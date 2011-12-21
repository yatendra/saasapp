using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SaasApp.DataModel
{
    public class Account
    {
        public String Name { get; set; }
        public String FriendlyName { get; set; }
        public bool IsActive { get; set; }
        public int PlanType { get; set; }
    }
}