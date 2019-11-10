using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CLF.Web.Framework.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace CLF.Web.Mvc.Controllers
{
    public class UserController : BaseController
    {
        public ActionResult Index()
        {
            return Json(0);
        }
    }
}