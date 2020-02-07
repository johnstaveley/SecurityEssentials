using SecurityEssentials.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SecurityEssentials.Core.Identity
{
	public interface IUserManager
    {
	    Task<SeIdentityResult> CreateAsync(string userName, string firstName, string lastName, string password, string passwordConfirmation, int securityQuestionLookupItemId, string securityAnswer);
	    Task<int> LogOnAsync(string userName, bool isPersistent);
	    Task<LogonResult> TryLogOnAsync(string userName, string password);
	    Task<User> FindUserByIdAsync(int userId);
	    void SignOut();
	    Task<SeIdentityResult> ChangePasswordAsync(int userId, string oldPassword, string newPassword);
	    Task<SeIdentityResult> ChangePasswordFromTokenAsync(int userId, string token, string newPassword);
	    Task<SeIdentityResult> ResetPasswordAsync(int userId, string actioningUserName);
	    Task<SeIdentityResult> ValidatePassword(User user, string password, List<string> bannedWords);

	}
}
