using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SecurityEssentials.Core
{

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>TODO: This should be created as a singleton using your DI framework</remarks>
    public class AppConfiguration : IAppConfiguration
    {

		public bool AccountManagementCheckFailedLogonAttempts { get; private set; }
		public int AccountManagementMaximumFailedLogonAttempts { get; private set; }
		public bool AccountManagementRegisterAutoApprove { get; private set; }
        public string ApplicationName { get; private set; }
        public string DefaultFromEmailAddress { get; private set; }
        public string EncryptionPassword { get; private set; }
        public int EncryptionIterationCount { get; private set; }
		public bool HasEmailConfigured { get; private set; }
        public bool HasRecaptcha { get; private set; }
        public string WebsiteBaseUrl { get; private set; }
		

        public AppConfiguration()
        {
			AccountManagementCheckFailedLogonAttempts = Convert.ToBoolean(ConfigurationManager.AppSettings["AccountManagementCheckFailedLogonAttempts"].ToString());
			AccountManagementMaximumFailedLogonAttempts = Convert.ToInt32(ConfigurationManager.AppSettings["AccountManagementMaximumFailedLogonAttempts"].ToString());
			AccountManagementRegisterAutoApprove = Convert.ToBoolean(ConfigurationManager.AppSettings["AccountManagementRegisterAutoApprove"]);
            ApplicationName = ConfigurationManager.AppSettings["ApplicationName"];
            DefaultFromEmailAddress = ConfigurationManager.AppSettings["DefaultFromEmailAddress"];
            EncryptionPassword = ConfigurationManager.AppSettings["EncryptionPassword"];
            EncryptionIterationCount = Convert.ToInt32(ConfigurationManager.AppSettings["EncryptionIterationCount"]);
            HasRecaptcha = Convert.ToBoolean(ConfigurationManager.AppSettings["HasRecaptcha"]);
			HasEmailConfigured = Convert.ToBoolean(ConfigurationManager.AppSettings["HasEmailConfigured"]);
            WebsiteBaseUrl = ConfigurationManager.AppSettings["WebsiteBaseUrl"];
        }
    }
}