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
    public class AccountBL
    {
        public static bool AccountExists(string accountName)
        {
            bool accountExists = true;
            using (MySqlConnection sqlConnection = new MySqlConnection(UtilityHelper.GetConnectionString()))
            {
                sqlConnection.Open();
                Account accountObj = sqlConnection.Query<Account>("Select * from Account where Name=@AccountName", new { AccountName = accountName }).FirstOrDefault();
                if (accountObj==null)
                {
                    accountExists = false;
                }
            }
            return accountExists;
        }
        public static void CreateAccount(string accountName, string friendlyName, string adminPassword, string adminEmail, int planType)
        {
            using (var ts = new TransactionScope())
            {
                using (MySqlConnection sqlConnection = new MySqlConnection(UtilityHelper.GetConnectionString()))
                {
                    string salt = Guid.NewGuid().ToString();
                    sqlConnection.Open();
                    sqlConnection.Execute(@"insert into Account(Name,FriendlyName,IsActive,PlanType) values (@Name,@FriendlyName,@IsActive,@PlanType)", new { Name = accountName, FriendlyName = friendlyName, IsActive = true, PlanType = planType });
                    sqlConnection.Execute(@"insert into User(Username,Account,Password,Salt,IsActive,IsAdmin,Email) values (@Username,@Account,@Password,@Salt,@IsActive,@IsAdmin,@Email)", new { Username = "administrator", Account = accountName, Password = UtilityHelper.HashPassword(adminPassword, salt), Salt = salt, IsActive = true, IsAdmin = true, Email = adminEmail });
                }
                ts.Complete();
            }
        }
    }
}