using System;
using System.Configuration;
using SecurityEssentials.Core.Identity;

namespace SecurityEssentials.Core
{
    /// <summary>
    /// </summary>
    /// <remarks>TODO: This should be created as a singleton using your DI framework</remarks>
    public class AppConfiguration : IAppConfiguration
    {
        public AppConfiguration()
        {
            AccountManagementCheckFailedLogonAttempts =
                Convert.ToBoolean(ConfigurationManager.AppSettings["AccountManagementCheckFailedLogonAttempts"]);
            AccountManagementMaximumFailedLogonAttempts =
                Convert.ToInt32(ConfigurationManager.AppSettings["AccountManagementMaximumFailedLogonAttempts"]);
            AccountManagementRegisterAutoApprove =
                Convert.ToBoolean(ConfigurationManager.AppSettings["AccountManagementRegisterAutoApprove"]);
            ApplicationName = ConfigurationManager.AppSettings["ApplicationName"];
            DefaultFromEmailAddress = ConfigurationManager.AppSettings["DefaultFromEmailAddress"];
            DefaultHashStrategy =
                (HashStrategyKind) Convert.ToInt32(ConfigurationManager.AppSettings["DefaultHashStrategy"]);
            EncryptionPassword = ConfigurationManager.AppSettings["EncryptionPassword"];
            EncryptionIterationCount = Convert.ToInt32(ConfigurationManager.AppSettings["EncryptionIterationCount"]);
            HasRecaptcha = Convert.ToBoolean(ConfigurationManager.AppSettings["HasRecaptcha"]);
            HasEmailConfigured = Convert.ToBoolean(ConfigurationManager.AppSettings["HasEmailConfigured"]);
            WebsiteBaseUrl = ConfigurationManager.AppSettings["WebsiteBaseUrl"];
        }

        public bool AccountManagementCheckFailedLogonAttempts { get; }
        public int AccountManagementMaximumFailedLogonAttempts { get; }
        public bool AccountManagementRegisterAutoApprove { get; }
        public string ApplicationName { get; }
        public string DefaultFromEmailAddress { get; }
        public string EncryptionPassword { get; }
        public int EncryptionIterationCount { get; }
        public bool HasEmailConfigured { get; }
        public bool HasRecaptcha { get; }
        public string WebsiteBaseUrl { get; }
        public HashStrategyKind DefaultHashStrategy { get; }
    }
}