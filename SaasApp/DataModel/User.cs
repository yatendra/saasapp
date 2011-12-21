using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SaasApp.DataModel
{
    public class User
    {
        public String Username { get; set; }
        public String Account { get; set; }
        public String Password { get; set; }
        public String Salt { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }
        public String Email { get; set; }
        public String Phone { get; set; }
    }
}