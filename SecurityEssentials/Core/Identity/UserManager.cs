using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using SecurityEssentials.Core.Constants;
using SecurityEssentials.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace SecurityEssentials.Core.Identity
{

	public class AppUserManager : IUserManager, IDisposable
	{

		private IAuthenticationManager _authenticationManager;
		private readonly IAppUserStore<User> _userStore;
		private readonly ISeContext _context;
		private readonly IAppConfiguration _configuration;
		private readonly IEncryption _encryption;
		private readonly IServices _services;
		private readonly IPwnedPasswordValidator _pwnedPasswordValidator;
		private readonly string _passwordValidityRegex = @"^.*(?=.{8,100})(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z0-9]).*$";
		private readonly string _passwordGoodEntropyRegex = @"^(?!.*(.)\1{2})(.*?){3,29}$";

		public IAuthenticationManager AuthenticationManager
		{
			get => _authenticationManager ?? (_authenticationManager = HttpContext.Current.GetOwinContext().Authentication);
			set => _authenticationManager = value;
		}

		public AppUserManager(IAppConfiguration configuration, ISeContext context, IEncryption encryption, IPwnedPasswordValidator pwnedPasswordValidator, IServices services, IAppUserStore<User> userStore)
		{
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
			_context = context ?? throw new ArgumentNullException(nameof(context));
			_encryption = encryption ?? throw new ArgumentNullException(nameof(encryption));
            _pwnedPasswordValidator = pwnedPasswordValidator ?? throw new ArgumentNullException(nameof(pwnedPasswordValidator));
			_services = services ?? throw new ArgumentNullException(nameof(services));
			_userStore = userStore ?? throw new ArgumentNullException(nameof(userStore));
		}

		public async Task<SeIdentityResult> CreateAsync(string userName, string firstName, string lastName, string password, string passwordConfirmation, int securityQuestionLookupItemId, string securityAnswer)
		{
			var user = await _userStore.FindByNameAsync(userName);

			// Validate security question
			var securityQuestion = _context.LookupItem.FirstOrDefault(a => a.Id == securityQuestionLookupItemId && a.LookupTypeId == Consts.LookupTypeId.SecurityQuestion);
			if (securityQuestion == null)
			{
				return new SeIdentityResult("Illegal security question");
			}
			var result = await ValidatePassword(new User(), password, new List<string> { firstName, lastName, securityAnswer });
			if (result.Succeeded)
			{

				if (user == null)
				{
					user = new User
					{
						UserName = userName
					};
					var securedPassword = new SecuredPassword(password, _configuration.DefaultHashStrategy);
					try
					{
						user.Approved = _configuration.AccountManagementRegisterAutoApprove;
						user.HashStrategy = securedPassword.HashStrategy;
						user.EmailConfirmationToken = Guid.NewGuid().ToString().Replace("-", "");
						user.EmailVerified = false;
						user.Enabled = true;
						user.FirstName = firstName;
						user.LastName = lastName;
						user.PasswordLastChangedDateUtc = DateTime.UtcNow;
						user.PasswordHash = Convert.ToBase64String(securedPassword.Hash);
						user.PasswordSalt = Convert.ToBase64String(securedPassword.Salt);
						user.SecurityQuestionLookupItemId = securityQuestionLookupItemId;
                        _encryption.Encrypt(_configuration.EncryptionPassword, _configuration.EncryptionIterationCount, securityAnswer, out var encryptedSecurityAnswerSalt, out var encryptedSecurityAnswer);
						user.SecurityAnswer = encryptedSecurityAnswer;
						user.SecurityAnswerSalt = encryptedSecurityAnswerSalt;
						user.UserName = userName;
						user.UserLogs.Add(new UserLog { Description = "Account Created" });
						await _userStore.CreateAsync(user);

					}
					catch
					{
						return new SeIdentityResult("An error occurred creating the user, please contact the system administrator");
					}

					return new SeIdentityResult();
				}

				// TODO: Log duplicate account creation
				return new SeIdentityResult("Username already registered");
			}
			else
			{
				return result;
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				// free managed resources
				_context?.Dispose();
				_userStore?.Dispose();
			}
		}

		public async Task<User> FindUserByIdAsync(int userId)
		{
			return await _userStore.FindByIdAsync(userId).ConfigureAwait(false);
		}

		public async Task<LogonResult> TryLogOnAsync(string userName, string password)
		{
			return await _userStore.TryLogOnAsync(userName, password).ConfigureAwait(false);
		}

		public async Task<int> LogOnAsync(string userName, bool isPersistent)
		{
			var user = await _userStore.FindByNameAsync(userName);
			AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
			var identity = await _userStore.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
			AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = isPersistent }, identity);
			user.UserLogs.Add(new UserLog { Description = "User Logged On" });
			await _userStore.UpdateAsync(user);
			return user.Id;
		}

		public void SignOut()
		{
			try
			{
				var userName = AuthenticationManager.User.Identity.Name;
				var user = _context.User.Single(u => u.UserName == userName);
				user.UserLogs.Add(new UserLog { Description = "User Logged Off" });
				_context.SaveChanges();
			}
			catch
			{
				// Ignore error
			}
			finally
			{
				AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
			}
		}

		public async Task<SeIdentityResult> ChangePasswordFromTokenAsync(int userId, string token, string newPassword)
		{
			var user = await FindUserByIdAsync(userId);
            _encryption.Decrypt(_configuration.EncryptionPassword, user.SecurityAnswerSalt, _configuration.EncryptionIterationCount, user.SecurityAnswer, out var decryptedSecurityAnswer);
			var bannedWords = new List<string> { user.FirstName, user.LastName, decryptedSecurityAnswer };
			var passwordValidationResult = await ValidatePassword(user, newPassword, bannedWords);
			if (passwordValidationResult.Succeeded)
			{
				var result = await _userStore.ChangePasswordFromTokenAsync(userId, token, newPassword);
				if (result.Succeeded)
				{
					return new SeIdentityResult();
				}
				return new SeIdentityResult(result.Errors);
			}
			return new SeIdentityResult(passwordValidationResult.Errors);
		}

		public async Task<SeIdentityResult> ChangePasswordAsync(int userId, string oldPassword, string newPassword)
		{
			var user = await FindUserByIdAsync(userId);
            _encryption.Decrypt(_configuration.EncryptionPassword, user.SecurityAnswerSalt, _configuration.EncryptionIterationCount, user.SecurityAnswer, out var decryptedSecurityAnswer);
			var bannedWords = new List<string> { user.FirstName, user.LastName, decryptedSecurityAnswer };
			var result = await ValidatePassword(user, newPassword, bannedWords);
			if (result.Succeeded)
			{
				var output = await _userStore.ChangePasswordAsync(userId, oldPassword, newPassword);
				if (output == 0)
				{
					return new SeIdentityResult(new List<string> { "An Error occurred trying to change the password" });
				}
				return new SeIdentityResult();
			}
			return result;
		}

		public async Task<SeIdentityResult> ResetPasswordAsync(int userId, string actioningUserName)
		{
			var user = await FindUserByIdAsync(userId);
			var newPassword = await GenerateSecurePassword(user);
			var result = await _userStore.ResetPasswordAsync(userId, newPassword, actioningUserName);
			if (result.Succeeded)
			{
				string emailSubject = $"{_configuration.ApplicationName} - Password Reset";
				string emailBody = EmailTemplates.PasswordResetBodyText(user.FirstName, user.LastName, _configuration.ApplicationName, newPassword);
				_services.SendEmail(_configuration.DefaultFromEmailAddress, new List<string> { user.UserName }, null, null, emailSubject, emailBody, true);

				return new SeIdentityResult();
			}
			return new SeIdentityResult(result.Errors);
		}

		/// <summary>
		/// The checks a password for minimum complexity, checks against a list of bad passwords and a list of banned words (usually personal information)
		/// </summary>
		/// <param name="user">The user of the password</param>
		/// <param name="password">The proposed new password</param>
		/// <param name="bannedWords">A list of words the password should not contain</param>
		/// <returns></returns>
		public async Task<SeIdentityResult> ValidatePassword(User user, string password, List<string> bannedWords)
		{
			if (string.IsNullOrEmpty(password) || Regex.Matches(password, _passwordValidityRegex).Count == 0)
			{
				return new SeIdentityResult(Consts.UserManagerMessages.PasswordValidityMessage);
			}
			if (Regex.Matches(password, _passwordGoodEntropyRegex).Count == 0)
			{
				return new SeIdentityResult("Your password cannot repeat the same character or digit more than 3 times consecutively, please choose another");
			}
			var badPassword = _context.LookupItem.FirstOrDefault(l => l.LookupTypeId == Consts.LookupTypeId.BadPassword && l.Description.ToLower() == password.ToLower());
			if (badPassword != null)
			{
				return new SeIdentityResult("Your password is on a list of easy to guess passwords, please choose another");
			}
			foreach (string bannedWord in bannedWords.Where(a => !string.IsNullOrWhiteSpace(a)))
			{
				if (password.IndexOf(bannedWord, StringComparison.OrdinalIgnoreCase) >= 0)
				{
					return new SeIdentityResult("Your password cannot contain any of your personal information");
				}
			}
			if (!string.IsNullOrEmpty(user.PasswordHash) && !string.IsNullOrEmpty(user.PasswordSalt))
			{
				// Check the user is not changing the password to the same one
				var hashOfPasswordUsingCurrentSettings = new SecuredPassword(password, Convert.FromBase64String(user.PasswordSalt), user.HashStrategy);
				if (Convert.ToBase64String(hashOfPasswordUsingCurrentSettings.Hash) == user.PasswordHash)
				{
					return new SeIdentityResult($"You cannot use any of your {_configuration.MaxNumberOfPreviousPasswords} previous passwords");
				}
			}
			if (user.PreviousPasswords.Count > 0)
			{
				foreach (var previousPassword in user.PreviousPasswords)
				{
					var hashOfPasswordUsingPreviousSettings = new SecuredPassword(password, Convert.FromBase64String(previousPassword.Salt), previousPassword.HashStrategy);
					if (Convert.ToBase64String(hashOfPasswordUsingPreviousSettings.Hash) == previousPassword.Hash)
					{
						return new SeIdentityResult($"You cannot use any of your {_configuration.MaxNumberOfPreviousPasswords} previous passwords");
					}
				}
			}
            var pwnedPassword = await _pwnedPasswordValidator.Validate(password);
            if (pwnedPassword.IsPwned)
            {
                return new SeIdentityResult("Your password has previously been found in a data breach, please choose another");
            }
			return new SeIdentityResult();
		}

		public async Task<string> GenerateSecurePassword(User user)
		{
			SeIdentityResult identityResult;
			int iterations = 0;
			string newPassword;
			do
			{
				iterations++;
				newPassword = System.Web.Security.Membership.GeneratePassword(10, 2);
				identityResult = await ValidatePassword(user, newPassword, new List<string>());
				if (iterations > 50) throw new Exception("Unable to complete operation generate secured password");
			} while (!identityResult.Succeeded);
			return newPassword;
		}
	}
}