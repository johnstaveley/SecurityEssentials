using SecurityEssentials.Core.Identity;
using System;
using System.Configuration;
using SecurityEssentials.Core.Constants;

namespace SecurityEssentials.Core
{

	/// <summary>
	/// 
	/// </summary>
	public class AppConfiguration : IAppConfiguration
    {

	    public bool AccountManagementCheckFailedLogonAttempts { get; }
	    public int AccountManagementMaximumFailedLogonAttempts { get; }
	    public bool AccountManagementRegisterAutoApprove { get; }
	    public string ApplicationName { get; }
	    public string DefaultCCEmailAddress { get; }
	    public string DefaultFromEmailAddress { get; }
	    public string EncryptionPassword { get; }
	    public int EncryptionIterationCount { get; }
	    public bool HasEmailConfigured { get; }
	    public string EmailHost { get; set; }
	    public int EmailHostPort { get; set; }
	    public string EmailHostUsername { get; set; }
	    public string EmailHostPassword { get; set; }
	    public bool EmailEnableSSL { get; set; }
	    public bool HasRecaptcha { get; }
	    public int MaxNumberOfPreviousPasswords { get; set; }
	    public PasswordExpiryStrategyKind PasswordExpiryStrategy { get; set; }
	    public string WebsiteBaseUrl { get; }
	    public HashStrategyKind DefaultHashStrategy { get; }

		public AppConfiguration()
        {
	        AccountManagementCheckFailedLogonAttempts = Convert.ToBoolean(ConfigurationManager.AppSettings["AccountManagementCheckFailedLogonAttempts"]);
	        AccountManagementMaximumFailedLogonAttempts = Convert.ToInt32(ConfigurationManager.AppSettings["AccountManagementMaximumFailedLogonAttempts"]);
	        AccountManagementRegisterAutoApprove = Convert.ToBoolean(ConfigurationManager.AppSettings["AccountManagementRegisterAutoApprove"]);
	        ApplicationName = ConfigurationManager.AppSettings["ApplicationName"];
	        DefaultCCEmailAddress = (string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["DefaultCCEmailAddress"]) ? null : ConfigurationManager.AppSettings["DefaultCCEmailAddress"]);
	        DefaultFromEmailAddress = ConfigurationManager.AppSettings["DefaultFromEmailAddress"];
	        DefaultHashStrategy = (HashStrategyKind)Convert.ToInt32(ConfigurationManager.AppSettings["DefaultHashStrategy"]);
	        EmailHost = ConfigurationManager.AppSettings["EmailHost"];
	        EmailHostPort = Convert.ToInt32(ConfigurationManager.AppSettings["EmailHostPort"]);
	        EmailHostUsername = ConfigurationManager.AppSettings["EmailHostUsername"];
	        EmailHostPassword = ConfigurationManager.AppSettings["EmailHostPassword"];
	        EmailEnableSSL = Convert.ToBoolean(ConfigurationManager.AppSettings["EmailEnableSSL"]);
	        EncryptionPassword = ConfigurationManager.AppSettings["EncryptionPassword"];
	        EncryptionIterationCount = Convert.ToInt32(ConfigurationManager.AppSettings["EncryptionIterationCount"]);
	        HasRecaptcha = Convert.ToBoolean(ConfigurationManager.AppSettings["HasRecaptcha"]);
	        HasEmailConfigured = Convert.ToBoolean(ConfigurationManager.AppSettings["HasEmailConfigured"]);
	        MaxNumberOfPreviousPasswords = Convert.ToInt32(ConfigurationManager.AppSettings["MaxNumberOfPreviousPasswords"]);
	        PasswordExpiryStrategy =
		        (PasswordExpiryStrategyKind)Convert.ToInt32(ConfigurationManager.AppSettings["PasswordExpiryStrategy"]);
	        WebsiteBaseUrl = ConfigurationManager.AppSettings["WebsiteBaseUrl"];
		}
    }
}