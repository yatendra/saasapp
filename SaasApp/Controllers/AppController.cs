using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SaasApp.Controllers
{
    public class AppController : Controller
    {
        public ActionResult Index(string account)
        {
            return View();
        }
    }
}
