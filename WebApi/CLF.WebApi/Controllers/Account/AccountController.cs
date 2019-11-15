using CLF.DataAccess.Account;
using CLF.Model.Account;
using CLF.Service.Account;
using CLF.Service.Core.Messages;
using CLF.Service.DTO.Account;
using CLF.Web.Framework.Identity;
using CLF.Web.Framework.Mvc;
using CLF.Web.Framework.Mvc.Filters;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Web;

namespace CLF.WebApi.Controllers.Account
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController : BaseApiController
    {
        private ApplicationSignInManager _applicationSignInManager;
        private ITokenService _tokenService;
        private IAccountService _accountService;
        private UserManager<AspNetUsers> _userManager;
        private IEmailSender _emailSender;

        public AccountController(ApplicationSignInManager applicationSignInManager, ITokenService tokenService, IAccountService accountService,
            UserManager<AspNetUsers> userManager, IEmailSender emailSender)
        {
            this._applicationSignInManager = applicationSignInManager;
            this._tokenService = tokenService;
            this._accountService = accountService;
            this._userManager = userManager;
            this._emailSender = emailSender;
        }

        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [ThrowIfException]
        public async Task<IActionResult> Register([FromBody]RegisterDTO model)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountService.CreateUserAsync(model);
                if (result.Key.Succeeded)
                {
                    AspNetUsers user = result.Value;
                    //发送验证邮件
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Action(nameof(AccountController.ConfirmEmail), "Account", new { email = user.Email, code = HttpUtility.UrlEncode(code) }, Request.Scheme, Request.Host.Host);
                    EmailMessage emailMessage = new EmailMessage
                    {
                        Subject = "注册激活",
                        Body = $"<a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>请单击激活账户</a>.",
                        To = new List<string> { model.Email }
                    };

                    //发送注册邮件
                    _emailSender.Send(emailMessage);

                    return ThrowJsonMessage(true, "恭喜您，注册成功!");
                }
                return ThrowJsonMessage(false, result.Key.Errors.First().Description);
            }
            return ThrowJsonMessage(false, GetModelStateErrorMessage());
        }

        [HttpGet]
        public ActionResult Login()
        {
            return Content("OK");
        }
        /// <summary>
        /// 用户登录成功后保存jwt的token，refreshToken
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login(string userName, string password)
        {
            var user = await _userManager.FindByNameAsync(userName);
            var checkPassword = await  _userManager.CheckPasswordAsync(user, password);
            if(!checkPassword)
                return ThrowJsonMessage(false, "用户名或密码错误");

            //生成token
            var token = _tokenService.GenerateAccessToken(userName);
            var refreshToken = _tokenService.GenerateRefreshToken();

            var securityToken = new AspNetUserSecurityTokenDTO
            {
                UserName = userName,
                RefreshToken = refreshToken
            };
            var isSave = _tokenService.AddToken(securityToken);
            if (isSave)
            {
                _tokenService.SetAccessTokenToCache(userName, token);//缓存token
                return new ObjectResult(new { success = true, token = token, refreshToken = refreshToken });
            }

            return BadRequest();
        }

        [AllowAnonymous]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> ConfirmEmail(string email, string code)
        {
            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(code))
            {
                var user = await _userManager.FindByEmailAsync(email);
                var result = await _userManager.ConfirmEmailAsync(user, HttpUtility.UrlDecode(code));
                if (result.Succeeded)
                {
                    return Ok();
                }
                return ThrowJsonMessage(false, "发送邮件失败");
            }
            return ThrowJsonMessage(false, $"{nameof(email)}或{nameof(code)}不能为空");
        }


        /// <summary>
        /// Migration生成或者更新数据库
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [ThrowIfException]
        public ActionResult InitAcccountDatabase()
        {
            DatabaseInitializer.Initialize();
            return Ok("数据库生成成功!");
        }
    }
}