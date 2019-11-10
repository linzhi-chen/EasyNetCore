using CLF.Common.Configuration;
using CLF.Common.Exceptions;
using CLF.Common.Infrastructure;
using CLF.Service.Account;
using CLF.Service.DTO.Account;
using CLF.Web.Framework.Mvc;
using CLF.Web.Framework.Mvc.Filters;
using IdentityModel.Client;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CLF.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OAuthController : BaseApiController
    {
        public static HttpClient _httpClient = new HttpClient();
        private ITokenService _tokenService;
        public OAuthController(ITokenService tokenService)
        {
            this._tokenService = tokenService;
        }

        /// <summary>
        /// jwt刷新token
        /// </summary>
        /// <param name="token"></param>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        [HttpPost]
        [ThrowIfException]
        public IActionResult RefreshToken(string token, string refreshToken)
        {
            if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(refreshToken))
            {
                var principal = _tokenService.GetPrincipalFromExpiredToken(token);
                var username = principal.Identity.Name;

                var aspNetUserSecurityToken = _tokenService.GetAspNetUserSecurityToken(username, refreshToken);
                if (aspNetUserSecurityToken == null)
                    return ThrowJsonMessage(false, $"{nameof(refreshToken)}不存在");

                var newToken = _tokenService.GenerateAccessToken(username);
                var newRefreshToken = _tokenService.GenerateRefreshToken();

                aspNetUserSecurityToken.RefreshToken = newRefreshToken;
                var result = _tokenService.ModifyToken(aspNetUserSecurityToken);
                if (result)
                {
                    _tokenService.SetAccessTokenToCache(username, newToken); //缓存Token
                    return new ObjectResult(new { success = true, token = newToken, refreshToken = newRefreshToken });
                }
                return ThrowJsonMessage(false, $"更新{nameof(refreshToken)}失败");
            }
            return ThrowJsonMessage(false, $"{nameof(token)}或{nameof(refreshToken)}不能为空");
        }

        /// <summary>
        /// 废除jwt refreshToken
        /// </summary>
        /// <returns></returns>
        [ThrowIfException, Authorize]
        [HttpPost]
        public IActionResult RevokeToken()
        {
            AspNetUserSecurityTokenDTO model = new AspNetUserSecurityTokenDTO
            {
                UserName = User.Identity.Name,
                IsRevoked = true
            };
            var result = _tokenService.ModifyToken(model);
            return ThrowJsonMessage(result);
        }

        /// <summary>
        /// 获取OAuth2.0认证 AccessToken
        /// </summary>
        /// <param name="type">授权模式（client，password，code）</param>
        /// <param name="clientId">客户端ID</param>
        /// <param name="clientSecret">客户端密码</param>
        /// <param name="userName">用户名</param>
        /// <param name="password">用户密码</param>
        /// <param name="code">授权码</param>
        /// <param name="scope">三种模式都可以不提供该参数</param>
        /// <returns></returns>
        [HttpGet]
        [ThrowIfException]
        public async Task<ActionResult> GetOAuthToken(string type, string clientId, string clientSecret, string userName, string password,  string code, string scope)
        {
            var config = EngineContext.Current.Resolve<OAuthConfig>();
            if (config == null)
                throw new BusinessException("配置错误");

            type = type ?? "client";
            var disco = await _httpClient.GetDiscoveryDocumentAsync(config.Authority);
            if (disco.IsError)
                return ThrowJsonMessage(false, disco.Error);

            TokenResponse token = null;
            switch (type)
            {
                case "client":
                    token = await _httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest()
                    {
                        Address = disco.TokenEndpoint,
                        ClientId = $"{nameof(GrantTypes.ClientCredentials)}_{clientId}",
                        ClientSecret = clientSecret,
                        Scope = scope
                    });
                    break;
                case "password":
                    token = await _httpClient.RequestPasswordTokenAsync(new PasswordTokenRequest()
                    {
                        Address = disco.TokenEndpoint,
                        ClientId = $"{nameof(GrantTypes.ResourceOwnerPassword)}_{clientId}",
                        ClientSecret = clientSecret,
                        UserName = userName,
                        Password = password,
                        Scope = scope
                    });
                    break;
                case "code":
                    token = await _httpClient.RequestAuthorizationCodeTokenAsync(new AuthorizationCodeTokenRequest()
                    {
                        Address = disco.TokenEndpoint,
                        ClientId = $"{nameof(GrantTypes.Code)}_{clientId}",
                        ClientSecret = clientSecret,
                        Code = code,
                        RedirectUri = config.RedirectUris
                    });
                    break;
            }

            if (token == null)
                return ThrowJsonMessage(false, "获取token失败");

            if (token.IsError)
                return ThrowJsonMessage(false, token.Error);

            return new JsonResult(new { access_token = token.AccessToken });
        }
    }
}