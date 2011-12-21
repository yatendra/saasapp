using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Mail;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using SaasApp.DataModel;
using SaasApp.Utility;
using SaasApp.BusinessLayer;

namespace SaasApp.Controllers
{
    public class AccountController : Controller
    {

        // **************************************
        // URL: /Account/LogOn
        // **************************************

        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(string username)
        {
            string subdomain = UtilityHelper.GetSubdomain(HttpContext.Request.Headers["HOST"]);            
            string password = Guid.NewGuid().ToString().Substring(0, 8);
            DataModel.User user = UserBL.ResetPassword(subdomain, username, password);
            HttpApplicationStateBase application=HttpContext.Application;
            string body = application[ConstantsUtil.ConfigForgotPasswordEmailBody].ToString().Replace("*password*", password);
            UtilityHelper.SendEmail(user.Email, application[ConstantsUtil.ConfigForgotPasswordEmailSubject].ToString(), body);
            ViewBag.Email = user.Email;
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string username, string password, bool? rememberMe)
        {
            string subdomain = UtilityHelper.GetSubdomain(HttpContext.Request.Headers["HOST"]);
            DataModel.User user = UserBL.GetValidatedUser(username, password, subdomain);
            if (user != null)
            {
                FormsAuthentication.SetAuthCookie(username, rememberMe.HasValue ? rememberMe.Value : false);
                Session[ConstantsUtil.SessionUser] = user;
                Session[ConstantsUtil.SessionRoles] = RoleBL.GetRolesForUser(username, subdomain);
                return RedirectToAction("Index", "App");
            }
            else
            {
                ViewBag.ErrorMessage = "Incorrect username/password";
                ViewBag.Username = username;
                return View();
            }
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "App");
        }

        public ActionResult ListUsers()
        {
            if (UtilityHelper.GetIsUserAdmin(Session))
            {
                string subdomain = UtilityHelper.GetSubdomain(HttpContext.Request.Headers["HOST"]);
                ViewBag.Users = UserBL.GetUsers(subdomain);
            }
            else
            {
                throw new Exception("Security exception");
            }
            return View();
        }

        public ActionResult Settings()
        {
            string subdomain = UtilityHelper.GetSubdomain(HttpContext.Request.Headers["HOST"]);
            string username = HttpContext.User.Identity.Name;
            DataModel.User userObj = UserBL.GetUser(subdomain, username);
            ViewBag.Username = username;
            ViewBag.Password = userObj.Password;
            ViewBag.Email = userObj.Email;
            return View();
        }

        public ActionResult DeleteUser(string account, string username)
        {            
            string subdomain = UtilityHelper.GetSubdomain(HttpContext.Request.Headers["HOST"]);
            if (subdomain.Equals(account) && UtilityHelper.GetIsUserAdmin(Session))
            {
                UserBL.DeleteUser(account, username);
            }
            else
            {
                throw new Exception("Security exception");
            }
            return RedirectToAction("ListUsers");
        }
        public ActionResult EditUser(string account, string username)
        {
            string subdomain = UtilityHelper.GetSubdomain(HttpContext.Request.Headers["HOST"]);
            if (subdomain.Equals(account) && UtilityHelper.GetIsUserAdmin(Session))
            {
                DataModel.User user = UserBL.GetUser(subdomain, username);
                ViewBag.Username = user.Username;
                ViewBag.Password = user.Password;
                ViewBag.Email = user.Email;
                ViewBag.IsAdmin = user.IsAdmin;
            }
            else
            {
                throw new Exception("Security exception");
            }
            return View();
        }
        [HttpPost]
        public ActionResult EditUser(string username, string password, string password2, string email, string email2, string isAdmin)
        {
            string subdomain = UtilityHelper.GetSubdomain(HttpContext.Request.Headers["HOST"]);
            if (UtilityHelper.GetIsUserAdmin(Session))
            {
                DataModel.User userObj = UserBL.GetUser(subdomain, username);
                bool? isAdminBool = null;
                if (!string.IsNullOrEmpty(isAdmin))
                {
                    if (isAdmin.Equals("on"))
                    {
                        isAdminBool = new bool?(true);
                    }
                }
                if (password.Equals(userObj.Password))
                {
                    //update without updating password
                    UserBL.UpdateUser(subdomain, username, email, true, isAdminBool);
                }
                else
                {
                    //update user along with password
                    UserBL.UpdateUser(subdomain, username, password, email, true, isAdminBool);
                }
            }
            else
            {
                throw new Exception("Security exception");
            }
            return RedirectToAction("ListUsers");
        }
        public ActionResult CreateUser()
        {
            if (!UtilityHelper.GetIsUserAdmin(Session))
            {
                throw new Exception("Security exception");
            }
            return View();
        }
        [HttpPost]
        public ActionResult CreateUser(string username, string password, string password2, string email, string email2, string isAdmin)
        {
            string subdomain = UtilityHelper.GetSubdomain(HttpContext.Request.Headers["HOST"]);
            if (UtilityHelper.GetIsUserAdmin(Session))
            {
                DataModel.User user = UserBL.GetUser(subdomain, username);
                bool? isAdminBool = null;
                if (!string.IsNullOrEmpty(isAdmin))
                {
                    if (isAdmin.Equals("on"))
                    {
                        isAdminBool = new bool?(true);
                    }
                }
                if (user!=null)
                {
                    if (user.IsActive)
                    {
                        ViewBag.Username = "";
                        ViewBag.Password = password;
                        ViewBag.Email = email;
                        ViewBag.ErrorMessage = HttpContext.Application[ConstantsUtil.ConfigUserExistsErrorMessage].ToString();
                        return View();
                    }
                    else
                    {
                        UserBL.UpdateUser(subdomain, username, email, true, isAdminBool);
                    }
                }
                else
                {
                    UserBL.CreateUser(username, subdomain, password, email,isAdminBool);
                }
            }
            else
            {
                throw new Exception("Security exception");
            } 
            return RedirectToAction("ListUsers");
        }
        [HttpPost]
        public ActionResult Settings(string username,string password,string password2,string email,string email2)
        {
            string subdomain = UtilityHelper.GetSubdomain(HttpContext.Request.Headers["HOST"]);
            string username1 = HttpContext.User.Identity.Name;
            if (username.Equals(username1))
            {
                DataModel.User userObj = UserBL.GetUser(subdomain, username1);
                if (password.Equals(userObj.Password))
                {
                    //update without updating password
                    UserBL.UpdateUser(subdomain, username, email, true, null);
                }
                else
                {
                    //update user along with password
                    UserBL.UpdateUser(subdomain, username, password, email, true, null);
                }
                return RedirectToAction("Index", "App");
            }
            return View();
        }
    }
}
