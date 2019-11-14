using CLF.Model.Account;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CLF.Web.SSO.Infrastructure
{
    public class IdentityUserValidator : IResourceOwnerPasswordValidator
    {
        public UserManager<AspNetUsers> _userManager;

        public IdentityUserValidator(UserManager<AspNetUsers> userManager)
        {
            this._userManager = userManager;
        }

        /// <summary>
        /// http://localhost:5000/connect/token 调用此方法将会去数据库验证用户信息
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async  Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var user =await  _userManager.FindByNameAsync(context.UserName);
            if( await _userManager.CheckPasswordAsync(user,context.Password))
            {
                context.Result = new GrantValidationResult(subject: user.Id, authenticationMethod:"Password");
            }
            else
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "用户名或密码错误");
            }
        }
    }
}
