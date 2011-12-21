using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using MySql.Data.MySqlClient;
using SaasApp.DataModel;
using SaasApp.Utility;

namespace SaasApp.BusinessLayer
{
    public class RoleBL
    {
        public static string[] GetRolesForUser(string username, string account)
        {
            string[] roles = null;
            using (MySqlConnection sqlConnection = new MySqlConnection(UtilityHelper.GetConnectionString()))
            {
                sqlConnection.Open();
                roles = sqlConnection.Query<string>("Select Role from UserRole where Account=@Account and Username=@Username", new { Account = account, Username=username }).ToArray();
            }
            return roles;
        }

        public static bool IsUserInRole(string username, string account, string role)
        {
            bool result = false;
            using (MySqlConnection sqlConnection = new MySqlConnection(UtilityHelper.GetConnectionString()))
            {
                sqlConnection.Open();
                string roleDb = sqlConnection.Query<string>("Select Role from UserRole where Account=@Account and Username=@Username and Role=@Role", new { Account = account, Username = username, Role=role }).FirstOrDefault();
                if (!string.IsNullOrEmpty(roleDb))
                {
                    result = true;
                }
            }
            return result;
        }
    }
}