using CLF.Common.Configuration;
using CLF.Common.Infrastructure;
using CLF.Service.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CLF.Web.Framework.Middleware
{
    public class JwtAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ITokenService _tokenService;

        public JwtAuthenticationMiddleware(ITokenService tokenService, RequestDelegate next)
        {
            this._tokenService = tokenService;
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }
        public async Task Invoke(HttpContext context)
        {
            var config = EngineContext.Current.Resolve<AppConfig>();

            //未开启jwt认证，直接通过
            if (!config.JwtAuthenticationEnabled)
            {
                await _next(context);
                return;
            }

            //开启jwt认证，并且缓存中存在对应的token，则通过
            var valid = await _tokenService.ValidateAccessTokenWithCache();
            if (valid)
            {
                await _next(context);
                return;
            }

            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        }
    }
}
