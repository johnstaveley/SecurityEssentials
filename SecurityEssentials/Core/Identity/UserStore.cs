using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SecurityEssentials.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;

namespace SecurityEssentials.Core.Identity
{

    public class UserStore<TUser> : IUserStore<User, int>, IUserPasswordStore<User, int>, IDisposable
    {

        #region Declarations

        public IPasswordHasher PasswordHasher { get; set; }
        public IIdentityValidator<string> PasswordValidator { get; set; }
        protected UserStore<IdentityUser> Store { get; private set; }
        private SEContext dbContext { get; set; }

        #endregion

        #region Constructor

        public UserStore(SEContext dbContext)
        {
            this.dbContext = dbContext;
        }

        #endregion

        #region IUserStore Implemented Methods

        public async Task CreateAsync(User user)
        {
            //user.Id = Guid.NewGuid();
            user.DateCreated = DateTime.Now;

            this.dbContext.User.Add(user);
            this.dbContext.Configuration.ValidateOnSaveEnabled = false;

            if (await this.dbContext.SaveChangesAsync().ConfigureAwait(false) == 0)
                throw new Exception("Error creating new user");
        }

        public async Task UpdateAsync(User user)
        {
            this.dbContext.User.Attach(user);
            this.dbContext.Entry(user).State = EntityState.Modified;

            await this.dbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task DeleteAsync(User user)
        {
            this.dbContext.User.Remove(user);

            await this.dbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<User> FindByIdAsync(int userId)
        {
            return await this.dbContext.User.SingleOrDefaultAsync(u => u.Id == userId).ConfigureAwait(false);
        }

        public async Task<User> FindByNameAsync(string userName)
        {
            return await this.dbContext.User.SingleOrDefaultAsync(u => !string.IsNullOrEmpty(u.UserName) && string.Compare(u.UserName, userName, StringComparison.InvariantCultureIgnoreCase) == 0).ConfigureAwait(false);
        }

        public async Task<int> GeneratePasswordResetTokenAsync(int userId)
        {
            var user = await this.FindByIdAsync(userId).ConfigureAwait(false);
            user.PasswordResetToken = Guid.NewGuid().ToString().Replace("-", "");
            user.PasswordResetExpiry = DateTime.Now.AddMinutes(15);
            return await this.dbContext.SaveChangesAsync().ConfigureAwait(false);
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
                if (this.dbContext != null)
                {
                    this.dbContext.Dispose();
                    this.dbContext = null;
                }
            }
        }

        #endregion

        #region Find

        public async Task<User> FindAsync(string userName, string password)
        {
            var user = await this.dbContext.User.SingleOrDefaultAsync(u => u.UserName == userName).ConfigureAwait(false);
            if (user != null)
            {
                var securedPassword = new SecuredPassword(Convert.FromBase64String(user.PasswordHash), Convert.FromBase64String(user.Salt));
                if (user.FailedLogonAttemptCount < 4)
                {
                    if (securedPassword.Verify(password))
                    {
                        user.FailedLogonAttemptCount = 0;
                        this.dbContext.SaveChanges();
                        return user;
                    }
                    else
                    {
                        user.FailedLogonAttemptCount += 1;
                        this.dbContext.SaveChanges();
                    }
                }
            }
            return null;
        }

        public async Task<User> FindByEmailAsync(string email)
        {
            var user = await this.dbContext.User.SingleOrDefaultAsync(u => u.Email == email).ConfigureAwait(false);
            if (user != null)
            {
                return user;
            }
            return null;
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
            if (user.PasswordResetToken != passwordResetToken || !user.PasswordResetExpiry.HasValue || user.PasswordResetExpiry < DateTime.Now)
            {
                return new IdentityResult("Your password reset token has expired or does not exist");
            }
            var securedPassword = new SecuredPassword(newPassword);
            if (securedPassword.Verify(newPassword))
            {
                user.PasswordHash = Convert.ToBase64String(securedPassword.Hash);
                user.Salt = Convert.ToBase64String(securedPassword.Salt);
                user.PasswordResetExpiry = null;
                user.PasswordResetToken = null;
                user.FailedLogonAttemptCount = 0;
            }
            await this.dbContext.SaveChangesAsync().ConfigureAwait(false);
            return new IdentityResult();
        }

        public async Task<int> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var user = await this.FindByIdAsync(userId).ConfigureAwait(false);
            var securedPassword = new SecuredPassword(currentPassword);
            if (securedPassword.Verify(currentPassword))
            {
                user.PasswordHash = Convert.ToBase64String(securedPassword.Hash);
                user.Salt = Convert.ToBase64String(securedPassword.Salt);
            }
            return await this.dbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        #endregion

    }
}