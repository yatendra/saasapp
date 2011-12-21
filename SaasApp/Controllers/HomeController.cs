using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SaasApp.BusinessLayer;

namespace SaasApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string account)
        {
            ViewBag.Message = "Welcome to ASP.NET MVC!"+account;

            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Plans()
        {
            return View();
        }

        public ActionResult SelectPlan(int planType)
        {
            ViewData.Model = planType;
            return View();
        }

        public ActionResult SelectAPlan(string accountName, string friendlyName, string adminPassword, string adminPassword2, string adminEmail, string adminEmail2, int planType)
        {
            if (!AccountBL.AccountExists(accountName))
            {
                AccountBL.CreateAccount(accountName, friendlyName, adminPassword, adminEmail, planType);
                ViewBag.Account = accountName;
                return View("AccountCreated");
            }
            else
            {
                ViewData.Model = planType;
                ViewBag.ErrorMessage = "Account '"+accountName+"' already exists. Please enter another name.";
                ViewBag.FriendlyName = friendlyName;
                ViewBag.AdminEmail = adminEmail;
                return View("SelectPlan");
            }
        }

        public ActionResult AccountCreated(string accountName)
        {
            ViewData.Model = accountName;
            return View();
        }
    }
}
