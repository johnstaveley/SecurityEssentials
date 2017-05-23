using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using SecurityEssentials.Core.Constants;
using SecurityEssentials.Model;

namespace SecurityEssentials.Core.Identity
{

	public class UserStore<TUser> : IAppUserStore<User>
	{


		private readonly string[] _commonlyUsedUserNames = { "administrator", "admin", "test" };

		public IPasswordHasher PasswordHasher { get; set; }
		public IIdentityValidator<string> PasswordValidator { get; set; }
		private ISeContext _context { get; set; }
		private IAppConfiguration _configuration { get; }

		public UserStore(ISeContext context, IAppConfiguration configuration)
		{
			_configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		public async Task CreateAsync(User user)
		{
			user.CreatedDateUtc = DateTime.UtcNow;
			SetPasswordExpiryStrategy(user);
			_context.User.Add(user);
			_context.SetConfigurationValidateOnSaveEnabled(false);

			if (await _context.SaveChangesAsync().ConfigureAwait(false) == 0)
			{
				throw new Exception("Error creating new user");
			}
		}

		public async Task UpdateAsync(User user)
		{
			_context.User.Attach(user);
			_context.SetModified(user);

			await _context.SaveChangesAsync().ConfigureAwait(false);
		}

		public async Task DeleteAsync(User user)
		{
			_context.User.Remove(user);

			await _context.SaveChangesAsync().ConfigureAwait(false);
		}

		public async Task<User> FindByIdAsync(int userId)
		{
			return await _context.User
				.Include("PreviousPasswords")
				.SingleOrDefaultAsync(u => u.Id == userId);
		}

		public async Task<User> FindByNameAsync(string userName)
		{
			return await _context.User
				.Include("UserRoles")
				.Include("UserRoles.Role")
				.SingleOrDefaultAsync(u => !string.IsNullOrEmpty(u.UserName) && string.Compare(u.UserName, userName, StringComparison.InvariantCultureIgnoreCase) == 0);
		}

		public async Task<string> GetPasswordHashAsync(User user)
		{
			user = await FindByNameAsync(user.UserName);
			return user.PasswordHash;
		}

		public async Task<bool> HasPasswordAsync(User user)
		{
			user = await FindByNameAsync(user.UserName).ConfigureAwait(false);
			return !string.IsNullOrEmpty(user.PasswordHash);
		}

		public async Task SetPasswordHashAsync(User user, string passwordHash)
		{
			user.PasswordHash = passwordHash;
			await UpdateAsync(user).ConfigureAwait(false);
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
				if (_context != null)
				{
					_context.Dispose();
					_context = null;
				}
			}
		}

		/// <summary>
		/// Finds the user from the password, if the password is incorrect then increment the number of failed logon attempts
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public async Task<LogonResult> TryLogOnAsync(string userName, string password)
		{

			var user = await _context.User
				.SingleOrDefaultAsync(u => u.UserName == userName && u.Enabled && u.Approved && u.EmailVerified)
				.ConfigureAwait(false);
			var logonResult = new LogonResult();
			if (user == null)
			{
				// Check if the user exists and if not is one of a commonly used set of usernames
				var userNameExists = await _context.User.SingleOrDefaultAsync(u => u.UserName == userName);
				if (userNameExists == null)
				{
					if (_commonlyUsedUserNames.ToList().Contains(userName))
					{
						logonResult.IsCommonUserName = true;
					}
				}
			}
			else
			{
				var securePassword = new SecuredPassword(password, Convert.FromBase64String(user.PasswordHash), Convert.FromBase64String(user.PasswordSalt), user.HashStrategy);
				if (_configuration.AccountManagementCheckFailedLogonAttempts == false || user.FailedLogonAttemptCount < _configuration.AccountManagementMaximumFailedLogonAttempts)
				{
					if (securePassword.IsValid)
					{
						user.FailedLogonAttemptCount = 0;
						await _context.SaveChangesAsync();
						logonResult.MustChangePassword = user.PasswordExpiryDateUtc.HasValue &&
						                                 user.PasswordExpiryDateUtc.Value < DateTime.UtcNow;
						logonResult.Success = true;
						logonResult.UserName = user.UserName;
						return logonResult;
					}
					user.FailedLogonAttemptCount += 1;
					logonResult.FailedLogonAttemptCount = user.FailedLogonAttemptCount;
					user.UserLogs.Add(new UserLog { Description = "Failed Logon attempt" });
					await _context.SaveChangesAsync();
				}
			}
			return logonResult;
		}

		public async Task<ClaimsIdentity> CreateIdentityAsync(User user, string authenticationType)
		{
			user = await FindByNameAsync(user.UserName);
			if (user != null)
			{
				List<Claim> claims = new List<Claim>
				{
					new Claim(ClaimTypes.Name, user.UserName),
					new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
					new Claim(ClaimTypes.AuthenticationMethod, authenticationType)
				};
				foreach (var userRole in user.UserRoles)
				{
					claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Description));
				}
				return new ClaimsIdentity(claims, authenticationType);
			}

			return null;
		}

		public async Task<IdentityResult> ChangePasswordFromTokenAsync(int userId, string passwordResetToken, string newPassword)
		{
			var user = await FindByIdAsync(userId).ConfigureAwait(false);
			if (user.PasswordResetToken != passwordResetToken || !user.PasswordResetExpiryDateUtc.HasValue || user.PasswordResetExpiryDateUtc < DateTime.UtcNow)
			{
				return new IdentityResult("Your password reset token has expired or does not exist");
			}
			var securedPassword = new SecuredPassword(newPassword, _configuration.DefaultHashStrategy);
			UpdatePreviousPasswordList(user);
			SetPasswordExpiryStrategy(user);
			user.HashStrategy = securedPassword.HashStrategy;
			user.PasswordHash = Convert.ToBase64String(securedPassword.Hash);
			user.PasswordLastChangedDateUtc = DateTime.UtcNow;
			user.PasswordSalt = Convert.ToBase64String(securedPassword.Salt);
			user.PasswordResetExpiryDateUtc = null;
			user.PasswordResetToken = null;
			user.FailedLogonAttemptCount = 0;
			user.UserLogs.Add(new UserLog { Description = "Password changed using token" });

			await _context.SaveChangesAsync().ConfigureAwait(false);
			return IdentityResult.Success;
		}

		public async Task<int> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
		{
			var user = await FindByIdAsync(userId).ConfigureAwait(false);
			var securePassword = new SecuredPassword(currentPassword, Convert.FromBase64String(user.PasswordHash), Convert.FromBase64String(user.PasswordSalt), user.HashStrategy);
			if (securePassword.IsValid)
			{
				var newPasswordHash = new SecuredPassword(newPassword, _configuration.DefaultHashStrategy);
				UpdatePreviousPasswordList(user);
				SetPasswordExpiryStrategy(user);
				user.PasswordHash = Convert.ToBase64String(newPasswordHash.Hash);
				user.PasswordLastChangedDateUtc = DateTime.UtcNow;
				user.PasswordSalt = Convert.ToBase64String(newPasswordHash.Salt);
				user.HashStrategy = newPasswordHash.HashStrategy;
				user.PasswordResetExpiryDateUtc = null;
				user.PasswordResetToken = null;
				user.FailedLogonAttemptCount = 0;
				user.UserLogs.Add(new UserLog() { Description = "Password changed" });
			}
			else
			{
				return 0;
			}
			return _context.SaveChanges();
		}

		public async Task<IdentityResult> ResetPasswordAsync(int userId, string newPassword, string actioningUserName)
		{
			var user = await FindByIdAsync(userId);
			SecuredPassword securedPassword = new SecuredPassword(newPassword, _configuration.DefaultHashStrategy);
			UpdatePreviousPasswordList(user);
			user.HashStrategy = securedPassword.HashStrategy;
			user.PasswordHash = Convert.ToBase64String(securedPassword.Hash);
			user.PasswordLastChangedDateUtc = DateTime.UtcNow;
			user.PasswordSalt = Convert.ToBase64String(securedPassword.Salt);
			user.PasswordResetExpiryDateUtc = null;
			user.PasswordResetToken = null;
			user.FailedLogonAttemptCount = 0;
			user.PasswordExpiryDateUtc = DateTime.UtcNow; // User must change their password on next logon
			user.UserLogs.Add(new UserLog() { Description = $"User had password reset sent out via email by {actioningUserName}" });
			await _context.SaveChangesAsync();
			return IdentityResult.Success;
		}

		/// <summary>
		/// Add last password to previous password list and remove earliest nth one if present
		/// </summary>
		/// <param name="user"></param>
		private void UpdatePreviousPasswordList(User user)
		{
			user.PreviousPasswords.Add(new PreviousPassword
			{
				HashStrategy = user.HashStrategy,
				Hash = user.PasswordHash,
				Salt = user.PasswordSalt,
				ActiveFromDateUtc = user.PasswordLastChangedDateUtc
			});
			var currentNumberOfPreviousPasswords = user.PreviousPasswords.Count;
			if (currentNumberOfPreviousPasswords >= _configuration.MaxNumberOfPreviousPasswords)
			{
				var earliestPassword = user.PreviousPasswords.OrderBy(a => a.ActiveFromDateUtc).Take(1).Single();
				_context.SetDeleted(earliestPassword);
			}

		}

		private void SetPasswordExpiryStrategy(User user)
		{
			var passwordExpiryStrategy = _configuration.PasswordExpiryStrategy;
			switch (passwordExpiryStrategy)
			{
				case (PasswordExpiryStrategyKind.DontRequireChanges):
					user.PasswordExpiryDateUtc = null;
					break;
				case (PasswordExpiryStrategyKind.ChangeEvery1Month):
					user.PasswordExpiryDateUtc = DateTime.UtcNow.AddMonths(1);
					break;
				default:
					throw new Exception($"Password Expiry Strategy {passwordExpiryStrategy} not found");
			}
		}

	}
}