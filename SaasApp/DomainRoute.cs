using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SaasApp.Utility;

namespace System.Web.Routing
{
    public class DomainRoute : RouteBase
    {
        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            string subdomain = UtilityHelper.GetSubdomain(httpContext.Request.Headers["HOST"]);
            RouteData routeData = new RouteData(this, new MvcRouteHandler());

            if (!string.IsNullOrEmpty(subdomain))
            {
                routeData.Values.Add("account", subdomain);
                string filepath = httpContext.Request.FilePath;
                string[] parts = filepath.Split('/');
                switch (parts[1].ToLower())
                {
                    case "app":
                        routeData.Values.Add("controller", "App");
                        if (parts.Length > 2)
                        {
                            switch (parts[2].ToLower())
                            {
                                case "index":
                                    routeData.Values.Add("action", "Index");
                                    break;
                                default:
                                    routeData.Values.Add("action", "Index");
                                    break;
                            }
                        }
                        else
                        {
                            routeData.Values.Add("action", "Index");
                        }
                        break;
                    case "account":
                        routeData.Values.Add("controller", "Account");
                        if (parts.Length > 2)
                        {
                            switch (parts[2].ToLower())
                            {
                                case "login":
                                    routeData.Values.Add("action", "Login");
                                    break;
                                case "logout":
                                    routeData.Values.Add("action", "Logout");
                                    break;
                                case "settings":
                                    routeData.Values.Add("action", "Settings");
                                    break;
                                case "forgotpassword":
                                    routeData.Values.Add("action", "ForgotPassword");
                                    break;
                                case "listusers":
                                    routeData.Values.Add("action", "ListUsers");
                                    break;
                                case "createuser":
                                    routeData.Values.Add("action", "CreateUser");
                                    break;
                                case "edituser":
                                    routeData.Values.Add("action", "EditUser");
                                    if (parts.Length > 4)
                                    {
                                        routeData.Values.Add("username", parts[4]);
                                    }
                                    break;
                                case "deleteuser":
                                    routeData.Values.Add("action", "DeleteUser");
                                    if (parts.Length > 4)
                                    {
                                        routeData.Values.Add("username", parts[4]);
                                    }
                                    break;
                            }
                        }
                        break;
                    case "error":
                        routeData.Values.Add("controller", "Error");
                        routeData.Values.Add("action", "Home");
                        break;
                    default:
                        if (httpContext.Request.IsAuthenticated)
                        {
                            httpContext.Response.Redirect("/App/Index");
                        }
                        httpContext.Response.Redirect("/Account/Login");
                        break;
                }
            }
            else
            {
                string filepath = httpContext.Request.FilePath;
                if (filepath.ToLower().Equals("/account/login"))
                {
                    httpContext.Response.Redirect("/Home/Index");
                }
                string[] parts = filepath.Split('/');
                switch (parts[1].ToLower())
                {
                    case "home":
                        routeData.Values.Add("controller", "Home");
                        if (parts.Length > 2)
                        {
                            switch (parts[2].ToLower())
                            {
                                case "index":
                                    routeData.Values.Add("action", "Index");
                                    break;
                                case "plans":
                                    routeData.Values.Add("action", "Plans");
                                    break;
                                case "selectplan":
                                    routeData.Values.Add("action", "SelectPlan");
                                    if (parts.Length > 3)
                                    {
                                        routeData.Values.Add("planType", parts[3]);
                                    }
                                    else
                                    {
                                        routeData.Values.Add("planType", "1");
                                    }
                                    break;
                                case "selectaplan":
                                    routeData.Values.Add("action", "SelectAPlan");
                                    break;
                                case "accountcreated":
                                    routeData.Values.Add("action", "AccountCreated");
                                    if (parts.Length > 3)
                                    {
                                        routeData.Values.Add("accountName", parts[3]);
                                    }
                                    else
                                    {
                                        routeData.Values.Add("accountName", "");
                                    }
                                    break;
                                default:
                                    routeData.Values.Add("action", "Index");
                                    break;
                            }
                        }
                        else
                        {
                            routeData.Values.Add("action", "Index");
                        }
                        break;
                    default:
                        httpContext.Response.Redirect("/Home/Index");
                        break;
                }
            }
            return routeData;
        }

        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
            //Implement your formating Url formating here
            return null;
        } 
    }
}