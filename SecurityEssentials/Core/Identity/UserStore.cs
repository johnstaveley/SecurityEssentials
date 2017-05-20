using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using SecurityEssentials.Model;

namespace SecurityEssentials.Core.Identity
{
    public class UserStore : IAppUserStore<User>
    {
        #region Constructor

        public UserStore(ISEContext context, IAppConfiguration configuration, UserStore store)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            if (context == null) throw new ArgumentNullException(nameof(context));

            Configuration = configuration;
            Store = store;
            Context = context;
        }

        public UserStore(ISEContext context, IAppConfiguration configuration) : this(context, configuration, new UserStore())
        {

        }

        public UserStore()
        {
        }

        #endregion

        #region TryLogOnAsync

        /// <summary>
        ///     Finds the user from the password, if the password is incorrect then increment the number of failed logon attempts
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<LogonResult> TryLogOnAsync(string userName, string password)
        {
            var user = await Context.User
                .SingleOrDefaultAsync(u => u.UserName == userName && u.Enabled && u.Approved && u.EmailVerified)
                .ConfigureAwait(false);
            var logonResult = new LogonResult();
            if (user == null)
            {
                // Check if the user exists and if not is one of a commonly used set of usernames
                var userNameExists = await Context.User.SingleOrDefaultAsync(u => u.UserName == userName);
                if (userNameExists == null)
                    if (_commonlyUsedUserNames.ToList().Contains(userName))
                        logonResult.IsCommonUserName = true;
            }
            else
            {
                var securePassword = new SecuredPassword(password, Convert.FromBase64String(user.PasswordHash),
                    Convert.FromBase64String(user.Salt), user.HashStrategy);
                if (Configuration.AccountManagementCheckFailedLogonAttempts && user.FailedLogonAttemptCount >=
                    Configuration.AccountManagementMaximumFailedLogonAttempts) return logonResult;

                if (securePassword.IsValid)
                {
                    user.FailedLogonAttemptCount = 0;
                    await Context.SaveChangesAsync();
                    logonResult.Success = true;
                    logonResult.UserName = user.UserName;
                    return logonResult;
                }
                else
                {
                    user.FailedLogonAttemptCount += 1;
                    logonResult.FailedLogonAttemptCount = user.FailedLogonAttemptCount;
                    user.UserLogs.Add(new UserLog { Description = "Failed Logon attempt" });
                    await Context.SaveChangesAsync();
                }
            }
            return logonResult;
        }

        #endregion

        #region Create

        public async Task<ClaimsIdentity> CreateIdentityAsync(User user, string authenticationType)
        {
            user = await FindByNameAsync(user.UserName).ConfigureAwait(false);

            if (user == null) return null;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.AuthenticationMethod, authenticationType)
            };
            foreach (var userRole in user.UserRoles)
                claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Description));
            return new ClaimsIdentity(claims, authenticationType);
        }

        #endregion

        #region Declarations

        private readonly string[] _commonlyUsedUserNames = { "administrator", "admin", "test" };

        public IPasswordHasher PasswordHasher { get; set; }
        public IIdentityValidator<string> PasswordValidator { get; set; }
        protected UserStore Store { get; }
        private ISEContext Context { get; set; }
        private IAppConfiguration Configuration { get; }

        #endregion

        #region IUserStore Implemented Methods

        public async Task CreateAsync(User user)
        {
            user.DateCreated = DateTime.UtcNow;

            Context.User.Add(user);
            Context.SetConfigurationValidateOnSaveEnabled(false);

            if (await Context.SaveChangesAsync().ConfigureAwait(false) == 0)
                throw new Exception("Error creating new user");
        }

        public async Task UpdateAsync(User user)
        {
            Context.User.Attach(user);
            Context.SetModified(user);

            await Context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task DeleteAsync(User user)
        {
            Context.User.Remove(user);

            await Context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<User> FindByIdAsync(int userId)
        {
            return await Context.User.SingleOrDefaultAsync(u => u.Id == userId).ConfigureAwait(false);
        }

        public async Task<User> FindByNameAsync(string userName)
        {
            return await Context.User
                .SingleOrDefaultAsync(u => !string.IsNullOrEmpty(u.UserName) &&
                                           string.Compare(u.UserName, userName,
                                               StringComparison.InvariantCultureIgnoreCase) == 0).ConfigureAwait(false);
        }

        #endregion

        #region IUserPasswordStore Implemented Methods

        public async Task<string> GetPasswordHashAsync(User user)
        {
            user = await FindByNameAsync(user.UserName).ConfigureAwait(false);
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

        #endregion

        #region IDisposable Implemented Methods

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;


            if (Context == null) return;

            Context.Dispose();
            Context = null;
        }

        #endregion

        #region Change

        public async Task<IdentityResult> ChangePasswordFromTokenAsync(int userId, string passwordResetToken,
            string newPassword)
        {
            var user = await FindByIdAsync(userId).ConfigureAwait(false);
            if (user.PasswordResetToken != passwordResetToken || !user.PasswordResetExpiry.HasValue ||
                user.PasswordResetExpiry < DateTime.UtcNow)
                return new IdentityResult("Your password reset token has expired or does not exist");
            var securedPassword = new SecuredPassword(newPassword, Configuration.DefaultHashStrategy);
            user.HashStrategy = securedPassword.HashStrategy;
            user.PasswordHash = Convert.ToBase64String(securedPassword.Hash);
            user.PasswordLastChangedDate = DateTime.UtcNow;
            user.Salt = Convert.ToBase64String(securedPassword.Salt);
            user.PasswordResetExpiry = null;
            user.PasswordResetToken = null;
            user.FailedLogonAttemptCount = 0;
            user.UserLogs.Add(new UserLog { Description = "Password changed using token" });

            await Context.SaveChangesAsync().ConfigureAwait(false);
            return new IdentityResult();
        }

        public async Task<int> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var user = await FindByIdAsync(userId).ConfigureAwait(false);
            var securePassword = new SecuredPassword(currentPassword, Convert.FromBase64String(user.PasswordHash),
                Convert.FromBase64String(user.Salt), user.HashStrategy);
            if (securePassword.IsValid)
            {
                var newPasswordHash = new SecuredPassword(currentPassword, Configuration.DefaultHashStrategy);
                user.PasswordHash = Convert.ToBase64String(newPasswordHash.Hash);
                user.PasswordLastChangedDate = DateTime.UtcNow;
                user.Salt = Convert.ToBase64String(newPasswordHash.Salt);
                user.HashStrategy = newPasswordHash.HashStrategy;
                user.PasswordResetExpiry = null;
                user.PasswordResetToken = null;
                user.FailedLogonAttemptCount = 0;
                user.UserLogs.Add(new UserLog { Description = "Password changed" });
            }
            return await Context.SaveChangesAsync().ConfigureAwait(false);
        }

        #endregion
    }
}