using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CLF.Service.Account;
using CLF.Web.Framework.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CLF.WebApi.Controllers.Account
{
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PermissionController : BaseApiController
    {
        private readonly IAccountService _accountService;
        public PermissionController(IAccountService accountService)
        {
            this._accountService = accountService;
        }

        /// <summary>
        /// 查询权限列表
        /// </summary>
        /// <param name="start">起始页</param>
        /// <param name="length">每页记录条数</param>
        /// <param name="controllerName">控制器</param>
        /// <param name="actionName">actionName</param>
        /// <returns>权限列表</returns>
        [HttpGet]
        [Authorize]
        public ActionResult GetPermissions(string controllerName, string actionName, int start = 0, int length = 10)
        {
            var result = _accountService.FindPagenatedListWithCount(start, length, controllerName, actionName);
            return new ObjectResult(result);
        }

        [HttpGet]
        [Authorize]
        public ActionResult GetPermission()
        {
            return Ok("ok!");
        }
    }
}