using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SecurityEssentials.Model;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SecurityEssentials.Core.Identity
{

	public class UserStore<TUser> : IAppUserStore<User>
	{

		#region Declarations

		private string[] commonlyUsedUserNames = { "administrator", "admin", "test" };

		public IPasswordHasher PasswordHasher { get; set; }
		public IIdentityValidator<string> PasswordValidator { get; set; }
		protected UserStore<IdentityUser> Store { get; private set; }
		private ISEContext _context { get; set; }
		private IAppConfiguration _configuration { get; set; }

		#endregion

		#region Constructor

		public UserStore(ISEContext context, IAppConfiguration configuration)
		{
			if (configuration == null) throw new ArgumentNullException("configuration");
			if (context == null) throw new ArgumentNullException("context");

			_configuration = configuration;
			_context = context;
		}

		#endregion

		#region IUserStore Implemented Methods

		public async Task CreateAsync(User user)
		{

			user.DateCreated = DateTime.UtcNow;

			_context.User.Add(user);
			_context.SetConfigurationValidateOnSaveEnabled(false);

			if (await this._context.SaveChangesAsync().ConfigureAwait(false) == 0)
			{
				throw new Exception("Error creating new user");
			}

		}

		public async Task UpdateAsync(User user)
		{
			_context.User.Attach(user);
			_context.SetModified(user);

			await this._context.SaveChangesAsync().ConfigureAwait(false);
		}

		public async Task DeleteAsync(User user)
		{
			_context.User.Remove(user);

			await this._context.SaveChangesAsync().ConfigureAwait(false);
		}

		public async Task<User> FindByIdAsync(int userId)
		{
			return await this._context.User.SingleOrDefaultAsync(u => u.Id == userId).ConfigureAwait(false);
		}

		public async Task<User> FindByNameAsync(string userName)
		{
			return await this._context.User.SingleOrDefaultAsync(u => !string.IsNullOrEmpty(u.UserName) && string.Compare(u.UserName, userName, StringComparison.InvariantCultureIgnoreCase) == 0).ConfigureAwait(false);
		}

		#endregion

		#region IUserPasswordStore Implemented Methods

		public async Task<string> GetPasswordHashAsync(User user)
		{
			user = await this.FindByNameAsync(user.UserName).ConfigureAwait(false);
			return user.PasswordHash;
		}

		public async Task<bool> HasPasswordAsync(User user)
		{
			user = await this.FindByNameAsync(user.UserName).ConfigureAwait(false);
			return !string.IsNullOrEmpty(user.PasswordHash);
		}

		public async Task SetPasswordHashAsync(User user, string passwordHash)
		{
			user.PasswordHash = passwordHash;
			await this.UpdateAsync(user).ConfigureAwait(false);
		}

		#endregion

		#region IDisposable Implemented Methods

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
				if (this._context != null)
				{
					this._context.Dispose();
					this._context = null;
				}
			}
		}

		#endregion

		#region TryLogOnAsync

		/// <summary>
		/// Finds the user from the password, if the password is incorrect then increment the number of failed logon attempts
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public async Task<LogonResult> TryLogOnAsync(string userName, string password)
		{

			var user = await this._context.User.SingleOrDefaultAsync(u => u.UserName == userName && u.Enabled && u.Approved && u.EmailVerified).ConfigureAwait(false);
			var logonResult = new LogonResult();
			if (user == null)
			{
				// Check if the user exists and if not is one of a commonly used set of usernames
				var userNameExists = await this._context.User.SingleOrDefaultAsync(u => u.UserName == userName);
				if (userNameExists == null)
				{
					if (commonlyUsedUserNames.ToList().Contains(userName))
					{
						logonResult.IsCommonUserName = true;
					}
				}
			}
			else
			{
				var securePassword = new SecuredPassword(password, Convert.FromBase64String(user.PasswordHash), Convert.FromBase64String(user.Salt), user.HashStrategy);
				if (_configuration.AccountManagementCheckFailedLogonAttempts == false || user.FailedLogonAttemptCount < _configuration.AccountManagementMaximumFailedLogonAttempts)
				{
					if (securePassword.IsValid)
					{
						user.FailedLogonAttemptCount = 0;
						await _context.SaveChangesAsync();
						logonResult.Success = true;
						logonResult.UserName = user.UserName;
						return logonResult;
					}
					else
					{
						user.FailedLogonAttemptCount += 1;
						logonResult.FailedLogonAttemptCount = user.FailedLogonAttemptCount;
						user.UserLogs.Add(new UserLog() { Description = "Failed Logon attempt" });
						await _context.SaveChangesAsync();
					}
				}
			}
			return logonResult;
		}

		#endregion

		#region Create

		public async Task<ClaimsIdentity> CreateIdentityAsync(User user, string authenticationType)
		{
			user = await this.FindByNameAsync(user.UserName).ConfigureAwait(false);
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
				return new System.Security.Claims.ClaimsIdentity(claims, authenticationType);
			}

			return null;
		}

		#endregion

		#region Change

		public async Task<IdentityResult> ChangePasswordFromTokenAsync(int userId, string passwordResetToken, string newPassword)
		{
			var user = await this.FindByIdAsync(userId).ConfigureAwait(false);
			if (user.PasswordResetToken != passwordResetToken || !user.PasswordResetExpiry.HasValue || user.PasswordResetExpiry < DateTime.UtcNow)
			{
				return new IdentityResult("Your password reset token has expired or does not exist");
			}
			var securedPassword = new SecuredPassword(newPassword, _configuration.DefaultHashStrategy);
			user.HashStrategy = securedPassword.HashStrategy;
			user.PasswordHash = Convert.ToBase64String(securedPassword.Hash);
			user.PasswordLastChangedDate = DateTime.UtcNow;
			user.Salt = Convert.ToBase64String(securedPassword.Salt);
			user.PasswordResetExpiry = null;
			user.PasswordResetToken = null;
			user.FailedLogonAttemptCount = 0;
			user.UserLogs.Add(new UserLog() { Description = "Password changed using token" });

			await this._context.SaveChangesAsync().ConfigureAwait(false);
			return new IdentityResult();
		}

		public async Task<int> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
		{
			var user = await this.FindByIdAsync(userId).ConfigureAwait(false);
			var securePassword = new SecuredPassword(currentPassword, Convert.FromBase64String(user.PasswordHash), Convert.FromBase64String(user.Salt), user.HashStrategy);
			if (securePassword.IsValid)
			{
				var newPasswordHash = new SecuredPassword(currentPassword, _configuration.DefaultHashStrategy);
				user.PasswordHash = Convert.ToBase64String(newPasswordHash.Hash);
				user.PasswordLastChangedDate = DateTime.UtcNow;
				user.Salt = Convert.ToBase64String(newPasswordHash.Salt);
				user.HashStrategy = newPasswordHash.HashStrategy;
				user.PasswordResetExpiry = null;
				user.PasswordResetToken = null;
				user.FailedLogonAttemptCount = 0;
				user.UserLogs.Add(new UserLog() { Description = "Password changed" });
			}
			return await this._context.SaveChangesAsync().ConfigureAwait(false);
		}

		#endregion

	}
}