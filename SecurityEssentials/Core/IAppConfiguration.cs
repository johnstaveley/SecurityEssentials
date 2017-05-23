using SecurityEssentials.Core.Constants;
using SecurityEssentials.Core.Identity;

namespace SecurityEssentials.Core
{
	public interface IAppConfiguration
    {
	    bool AccountManagementCheckFailedLogonAttempts { get; }
	    int AccountManagementMaximumFailedLogonAttempts { get; }
	    bool AccountManagementRegisterAutoApprove { get; }
	    string ApplicationName { get; }
	    string DefaultCCEmailAddress { get; }
	    string DefaultFromEmailAddress { get; }
	    HashStrategyKind DefaultHashStrategy { get; }
	    int EncryptionIterationCount { get; }
	    string EncryptionPassword { get; }
	    bool HasEmailConfigured { get; }
	    bool HasRecaptcha { get; }
	    string WebsiteBaseUrl { get; }
	    string EmailHost { get; set; }
	    int EmailHostPort { get; set; }
	    string EmailHostUsername { get; set; }
	    string EmailHostPassword { get; set; }
	    bool EmailEnableSSL { get; set; }
	    int MaxNumberOfPreviousPasswords { get; set; }
	    PasswordExpiryStrategyKind PasswordExpiryStrategy { get; set; }
	}
}