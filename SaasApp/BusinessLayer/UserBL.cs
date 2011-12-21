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
    public class UserBL
    {
        public static List<DataModel.User> GetUsers(string accountName)
        {
            List<DataModel.User> users = null;
            using (MySqlConnection sqlConnection = new MySqlConnection(UtilityHelper.GetConnectionString()))
            {
                sqlConnection.Open();
                users = sqlConnection.Query<DataModel.User>("Select * from User where Account=@Account and IsActive=true", new { Account = accountName }).ToList();
            }
            return users;
        }
        public static void CreateUser(string username, string accountName, string password, string email, bool? isAdmin)
        {
            using (MySqlConnection sqlConnection = new MySqlConnection(UtilityHelper.GetConnectionString()))
            {
                string salt = Guid.NewGuid().ToString();
                sqlConnection.Open();
                sqlConnection.Execute(@"insert into User(Username,Account,Password,Salt,IsActive,IsAdmin,Email) values (@Username,@Account,@Password,@Salt,@IsActive,@IsAdmin,@Email)", new { Username = username, Account = accountName, Password = UtilityHelper.HashPassword(password, salt), Salt = salt, IsActive = true, IsAdmin = isAdmin, Email = email });
            }
        }
        public static DataModel.User GetUser(string account, string username)
        {
            DataModel.User userObj = null;
            using (MySqlConnection sqlConnection = new MySqlConnection(UtilityHelper.GetConnectionString()))
            {
                sqlConnection.Open();
                userObj = sqlConnection.Query<DataModel.User>("Select * from User where Username=@Username and Account=@Account", new { Username = username, Account = account }).FirstOrDefault();
            }
            return userObj;
        }
        public static void DeleteUser(string account, string username)
        {
            using (MySqlConnection sqlConnection = new MySqlConnection(UtilityHelper.GetConnectionString()))
            {
                sqlConnection.Open();
                sqlConnection.Execute(@"update User set IsActive=false where Username=@Username and Account=@Account", new { Username = username, Account = account });
            }
        }
        public static DataModel.User ResetPassword(string account, string username, string password)
        {
            DataModel.User userObj = null;
            using (MySqlConnection sqlConnection = new MySqlConnection(UtilityHelper.GetConnectionString()))
            {
                sqlConnection.Open();
                string salt = sqlConnection.Query<string>("select Salt from user where Username=@Username and Account=@Account", new { Username = username, Account = account }).FirstOrDefault();
                if (string.IsNullOrEmpty(salt))
                {
                    salt = Guid.NewGuid().ToString();
                }
                sqlConnection.Execute(@"update User set Password=@Password, Salt=@Salt where Username=@Username and Account=@Account", new { Username = username, Account = account, Salt = salt, Password = UtilityHelper.HashPassword(password, salt) });
                userObj = sqlConnection.Query<DataModel.User>("Select * from User where Username=@Username and Account=@Account", new { Username = username, Account = account }).FirstOrDefault();
            }
            return userObj;
        }
        public static void UpdateUser(string account, string username, string email, bool isActive, bool? isAdmin)
        {
            using (MySqlConnection sqlConnection = new MySqlConnection(UtilityHelper.GetConnectionString()))
            {
                sqlConnection.Open();
                string salt = sqlConnection.Query<string>("select Salt from user where Username=@Username and Account=@Account", new { Username = username, Account = account }).FirstOrDefault();
                if (string.IsNullOrEmpty(salt))
                {
                    salt = Guid.NewGuid().ToString();
                }
                sqlConnection.Execute(@"update User set Email=@Email, Salt=@Salt, IsActive=@IsActive, IsAdmin=@IsAdmin where Username=@Username and Account=@Account", new { Email = email, Username = username, Account = account, Salt = salt, IsActive = isActive, IsAdmin = isAdmin });
            }
        }
        public static void UpdateUser(string account, string username, string password, string email, bool isActive, bool? isAdmin)
        {
            using (MySqlConnection sqlConnection = new MySqlConnection(UtilityHelper.GetConnectionString()))
            {
                sqlConnection.Open();
                string salt = sqlConnection.Query<string>("select Salt from user where Username=@Username and Account=@Account", new { Username = username, Account = account }).FirstOrDefault();
                if (string.IsNullOrEmpty(salt))
                {
                    salt = Guid.NewGuid().ToString();
                }
                if (isAdmin.HasValue)
                {
                    sqlConnection.Execute(@"update User set Email=@Email, Password=@Password, Salt=@Salt, IsActive=@IsActive, IsAdmin=@IsAdmin where Username=@Username and Account=@Account", new { Email = email, Username = username, Password = UtilityHelper.HashPassword(password, salt), Account = account, Salt = salt, IsActive = isActive, IsAdmin = isAdmin });
                }
                else
                {
                    sqlConnection.Execute(@"update User set Email=@Email, Password=@Password, Salt=@Salt, IsActive=@IsActive where Username=@Username and Account=@Account", new { Email = email, Username = username, Password = UtilityHelper.HashPassword(password, salt), Account = account, Salt = salt, IsActive = isActive });
                }
            }
        }
        public static DataModel.User GetValidatedUser(string username, string password, string account)
        {
            using (MySqlConnection sqlConnection = new MySqlConnection(UtilityHelper.GetConnectionString()))
            {
                sqlConnection.Open();
                DataModel.User userObj = sqlConnection.Query<DataModel.User>("Select * from User where Username=@Username and Account=@Account", new { Username = username, Account = account }).FirstOrDefault();
                if (userObj != null)
                {
                    if (string.IsNullOrEmpty(userObj.Salt))
                    {
                        if (password.Equals(userObj.Password))
                        {
                            string salt = Guid.NewGuid().ToString();
                            sqlConnection.Execute(@"update User set Salt=@Salt, Password=@Password where Username=@Username and Account=@Account", new { Salt = salt, Password = UtilityHelper.HashPassword(userObj.Password, salt), Username = username, Account = account });
                            return userObj;
                        }
                    }
                    else
                    {
                        password=UtilityHelper.HashPassword(password,userObj.Salt);
                        if (password.Equals(userObj.Password))
                        {
                            return userObj;
                        }
                    }
                }
            }
            return null;
        }
    }
}