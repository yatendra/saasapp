using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using log4net;

namespace SaasApp
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.Add(new DomainRoute());
            //routes.Add(new DomainRoute(
            //    "{controller}/{action}/{id}", // URL with parameters
            //    new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            //    ));
            routes.MapRoute(
                "Default", // Route name
               "{controller}/{action}/{id}", // URL with parameters
                new { controller = "App", action = "Index", id = UrlParameter.Optional, org = "" } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo((Server.MapPath("~/Web.config"))));
            AreaRegistration.RegisterAllAreas();
            RegisterRoutes(RouteTable.Routes);
            string[] keys = ConfigurationManager.AppSettings.AllKeys;
            System.Collections.Specialized.NameValueCollection appSettings = ConfigurationManager.AppSettings;
            for (int i = 0; i < ConfigurationManager.AppSettings.Count; i++)
            {
                string key = keys[i];
                Application[key] = appSettings[key];
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            // Get the exception object.
            Exception ex = Server.GetLastError();
            ILog logger = LogManager.GetLogger((ex.Source));
            logger.Error(ex.Message, ex); // where ex is the exception instance
        }
    }
}