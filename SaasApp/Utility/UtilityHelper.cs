using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using SaasApp.BusinessLayer;

namespace SaasApp.Utility
{
    public class UtilityHelper
    {
        private static System.Collections.Specialized.NameValueCollection appSettings = null;
        private UtilityHelper()
        {
            appSettings = ConfigurationManager.AppSettings;
        }
        public static bool GetIsUserAdmin(HttpSessionStateBase session)
        {
            DataModel.User user = (DataModel.User)session[ConstantsUtil.SessionUser];
            return user.IsAdmin;
        }
        public static string GetUsername(HttpSessionStateBase session)
        {
            if (session[ConstantsUtil.SessionUser] != null)
            {
                return ((SaasApp.DataModel.User)session[ConstantsUtil.SessionUser]).Username;
            }
            return string.Empty;
        }
        public static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings[ConstantsUtil.ConfigConnectionString].ConnectionString;
        }
        public static string GetSubdomain(string domain)
        {
            string baserUrl = ConfigurationManager.AppSettings[ConstantsUtil.ConfigBaseUrl];
            if (domain.IndexOf(baserUrl) > 0)
            {
                int index = domain.IndexOf(".");
                if (index < 0)
                {
                    return null;
                }
                return domain.Substring(0, index).ToLower();                
            }
            else
            {
                return null;
            }
        }
        public static string HashPassword(string password, string salt)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password + salt);
            HashAlgorithm hash = new SHA1Managed();
            byte[] hashedBytes = hash.ComputeHash(passwordBytes);
            return Convert.ToBase64String(hashedBytes);
        }
        public static void SendEmail(string toEmail, string subject, string body)
        {
            HttpApplicationState application = HttpContext.Current.Application;
            MailMessage mail = new MailMessage();
            mail.To.Add(toEmail);
            mail.From = new MailAddress(application[ConstantsUtil.ConfigFromEmail].ToString());
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = application[ConstantsUtil.ConfigSMTPServer].ToString(); //Or Your SMTP Server Address
            smtp.Port = Int32.Parse(application[ConstantsUtil.ConfigSMTPPort].ToString());
            smtp.Credentials = new System.Net.NetworkCredential(application[ConstantsUtil.ConfigSMTPUser].ToString(), application[ConstantsUtil.ConfigSMTPPassword].ToString());
            smtp.EnableSsl = true;
            smtp.SendCompleted += new SendCompletedEventHandler(smtp_SendCompleted);
            smtp.SendAsync(mail, null);
        }
        private static void smtp_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
        }
    }
}