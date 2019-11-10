using CLF.Service.Core;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace CLF.Web.Framework.Mvc
{
    public class BaseController : Controller
    {
        protected JsonResult GetServiceJsonResult<T>(Func<T> func)
        {
            var result = ServiceWrapper.Invoke(
                "CLF.Web.Mvc.Controllers.BaseController",
                "GetServiceJsonResult",
                func);

            return Json(result);
        }

        protected string GetModelStateErrorMessage()
        {
            string message = string.Empty;
            foreach (var value in ModelState.Values.Where(o => o.Errors.Any()))
            {
                if (!string.IsNullOrEmpty(value.Errors.FirstOrDefault().ErrorMessage))
                {
                    message = value.Errors.FirstOrDefault().ErrorMessage;
                    break;
                }
            }
            return message;
        }

        protected virtual JsonResult ThrowJsonMessage(bool success, string message = null)
        {
            return Json(new ServiceResult<bool>(success ? ServiceResultType.Success : ServiceResultType.Error, message));
        }

        protected ClaimsIdentity Identity
        {
            get
            {
                var principal = User as ClaimsPrincipal;
                if(principal!=null)
                {
                    return principal.Identity as ClaimsIdentity;
                }
                return null;
            }
        }


        protected string CurrentUserId
        {
            get
            {
                if (Identity != null&&Identity.FindFirst(ClaimTypes.NameIdentifier)!=null)
                {
                    return Identity.FindFirst(ClaimTypes.NameIdentifier).Value;
                }
                return string.Empty;
            }
        }

        protected string CurrentUserName
        {
            get
            {
                if (Identity != null && Identity.FindFirst(ClaimTypes.Name) != null)
                {
                    return Identity.FindFirst(ClaimTypes.Name).Value;
                }
                return string.Empty;
            }
        }

        protected string CurrentUserPhone
        {
            get
            {
                if (Identity != null && Identity.FindFirst(ClaimTypes.MobilePhone) != null)
                {
                    return Identity.FindFirst(ClaimTypes.MobilePhone).Value;
                }
                return string.Empty;
            }
        }

        protected string CurrentUserEmail
        {
            get
            {
                if (Identity != null && Identity.FindFirst(ClaimTypes.Email) != null)
                {
                    return Identity.FindFirst(ClaimTypes.Email).Value;
                }
                return string.Empty;
            }
        }
    }
}
