using CLF.Common.Caching;
using CLF.Common.Configuration;
using CLF.Common.Exceptions;
using CLF.Common.Extensions;
using CLF.Common.Infrastructure;
using CLF.Common.SecurityHelper;
using CLF.DataAccess.Account;
using CLF.Domain.Core.EFRepository;
using CLF.Model.Account;
using CLF.Service.Core.Extensions;
using CLF.Service.DTO.Account;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CLF.Service.Account
{
    public class TokenService : ITokenService
    {
        private readonly JwtConfig jwtConfig = EngineContext.Current.Resolve<JwtConfig>();
        private readonly CommonRepository<AspNetUserSecurityToken> _securityTokenRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStaticCacheManager _staticCacheManager;
      
        public TokenService(CommonRepository<AspNetUserSecurityToken> securityTokenRepository,IHttpContextAccessor httpContextAccessor,IStaticCacheManager staticCacheManager)
        {
            this._securityTokenRepository = securityTokenRepository;
            this._httpContextAccessor = httpContextAccessor;
            this._staticCacheManager = staticCacheManager;
        }
        public string GenerateAccessToken(string userName)
        {
            if (jwtConfig == null)
                throw new BusinessPromptException("jwt认证配置错误。");

            var usersClaims = new[]
            {
                 new Claim(JwtRegisteredClaimNames.Nbf,$"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}") ,
                 new Claim (JwtRegisteredClaimNames.Exp,$"{new DateTimeOffset(DateTime.Now.AddMinutes(jwtConfig.ExpiredMinutes)).ToUnixTimeSeconds()}"),
                 new Claim(ClaimTypes.Name, userName)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.SecurityKey));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: jwtConfig.Issuer,
                audience: jwtConfig.Issuer,
                claims: usersClaims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddMinutes(jwtConfig.ExpiredMinutes),
                signingCredentials: signingCredentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,//过期也可以验证
                ValidateIssuerSigningKey = true,
                ValidAudience = jwtConfig.Issuer,
                ValidIssuer = jwtConfig.Issuer,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.SecurityKey))
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }

        public string GenerateRefreshToken() 
            => RandomProvider.GenerateRandom();

        public async Task<bool> ValidateAccessTokenWithCache()
        {
            if (string.IsNullOrEmpty(GetCurrentToken()))
                return true;

            var key = GetKey(GetCurrentToken());
            var cacheToken = await _staticCacheManager.GetAsync(key, () => Task.FromResult<string>(null),0);
            return !string.IsNullOrEmpty(cacheToken);
        }

        /// <summary>
        /// 用于登录时缓存用户的jwtToken，同时删除该用户缓存的旧jwtToken
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="token"></param>
        public void SetAccessTokenToCache(string userName,string token)
        {
            var userNameKey = GetKey(userName);
            var tokenKey = GetKey(token);

            var cacheOldToken =  _staticCacheManager.GetAsync(userNameKey, () => Task.FromResult<string>(null), 0).Result;
            if (!string.IsNullOrEmpty(cacheOldToken))
            {
                var oldTokenKey = GetKey(cacheOldToken);
                _staticCacheManager.Remove(oldTokenKey);
            }

            _staticCacheManager.Remove(userNameKey);

            _staticCacheManager.Set(userNameKey, token, jwtConfig.ExpiredMinutes);
            _staticCacheManager.Set(tokenKey, token, jwtConfig.ExpiredMinutes);
        }

        public bool AddToken(AspNetUserSecurityTokenDTO model)
        {
            var securityToken = model.Map<AspNetUserSecurityToken>();
            return _securityTokenRepository.Add(securityToken);
        }

        public bool ModifyToken(AspNetUserSecurityTokenDTO model)
        {
            Expression<Func<AspNetUserSecurityToken, bool>> expression = o => !o.IsDeleted;

            if (!string.IsNullOrEmpty(model.ClientId))
                expression = expression.And(o => o.ClientId == model.ClientId);

            if (!string.IsNullOrEmpty(model.UserName))
                expression = expression.And(o => o.UserName == model.UserName);

            var securityToken = _securityTokenRepository.Find(expression);
            securityToken.ToList().ForEach(o =>
            {
                if (!string.IsNullOrEmpty(model.RefreshToken))
                    o.RefreshToken = model.RefreshToken;
                o.IsRevoked = model.IsRevoked;
            });
            return _securityTokenRepository.Modify(securityToken, new string[] { "RefreshToken", "IsRevoked" }) > 0;
        }

        public AspNetUserSecurityTokenDTO GetAspNetUserSecurityToken(string userName, string refreshToken)
        {
            var data = _securityTokenRepository.FindByFilter(o => o.UserName == userName && o.RefreshToken == refreshToken
                    && !o.IsDeleted && !o.IsRevoked);
            return data.Map<AspNetUserSecurityTokenDTO>();
        }

        private string GetCurrentToken()
        {
            var authorizationHeader = _httpContextAccessor.HttpContext.Request.Headers["authorization"];
            return authorizationHeader == StringValues.Empty
                ? string.Empty
                : authorizationHeader.Single().Split(" ").Last();
        }

        private static string GetKey(string key)
            => string.Format(AccountServiceDefaults.JwtTokenCacheKey, key);
    }
}
