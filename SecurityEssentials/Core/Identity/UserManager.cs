using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using SecurityEssentials.Core.Constants;
using SecurityEssentials.Model;

namespace SecurityEssentials.Core.Identity
{
    public class AppUserManager : IUserManager, IDisposable
    {
        #region Properties

        private static IAuthenticationManager AuthenticationManager => HttpContext.Current.GetOwinContext()
            .Authentication;

        #endregion

        #region Create

        public async Task<SEIdentityResult> CreateAsync(string userName, string firstName, string lastName,
            string password, string passwordConfirmation, int securityQuestionLookupItemId, string securityAnswer)
        {
            var user = await _userStore.FindByNameAsync(userName);

            // Validate security question
            var securityQuestion = _context.LookupItem.FirstOrDefault(a => a.Id == securityQuestionLookupItemId &&
                            a.LookupTypeId == CONSTS.LookupTypeId.SecurityQuestion);
            if (securityQuestion == null)
                return new SEIdentityResult("Illegal security question");
            var result = ValidatePassword(password, new List<string> {firstName, lastName, securityAnswer});
            if (result.Succeeded)
            {
                if (user == null)
                {
                    user = new User();
                    user.UserName = userName;
                    var securedPassword = new SecuredPassword(password, _configuration.DefaultHashStrategy);
                    try
                    {
                        user.Approved = _configuration.AccountManagementRegisterAutoApprove;
                        user.EmailConfirmationToken = Guid.NewGuid().ToString().Replace("-", "");
                        user.EmailVerified = false;
                        user.Enabled = true;
                        user.FirstName = firstName;
                        user.LastName = lastName;
                        user.PasswordLastChangedDate = DateTime.UtcNow;
                        user.PasswordHash = Convert.ToBase64String(securedPassword.Hash);
                        user.Salt = Convert.ToBase64String(securedPassword.Salt);
                        user.SecurityQuestionLookupItemId = securityQuestionLookupItemId;
                        var encryptedSecurityAnswer = "";
                        _encryption.Encrypt(_configuration.EncryptionPassword, user.Salt,
                            _configuration.EncryptionIterationCount, securityAnswer, out encryptedSecurityAnswer);
                        user.SecurityAnswer = encryptedSecurityAnswer;
                        user.UserName = userName;
                        user.UserLogs.Add(new UserLog {Description = "Account Created"});
                        await _userStore.CreateAsync(user);
                    }
                    catch
                    {
                        return new SEIdentityResult(
                            "An error occurred creating the user, please contact the system administrator");
                    }

                    return new SEIdentityResult();
                }

                // TODO: Log duplicate account creation
                return new SEIdentityResult("Username already registered");
            }
            return result;
        }

        #endregion

        public async Task LogOnAsync(string userName, bool isPersistent)
        {
            var user = await _userStore.FindByNameAsync(userName);
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await _userStore.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties {IsPersistent = isPersistent}, identity);
            user.UserLogs.Add(new UserLog {Description = "User Logged On"});
            await _userStore.UpdateAsync(user);
        }

        public void SignOut()
        {
            try
            {
                var userName = AuthenticationManager.User.Identity.Name;
                var user = _context.User.FirstOrDefault(u => u.UserName == userName);
                user.UserLogs.Add(new UserLog {Description = "User Logged Off"});
                _context.SaveChanges();
            }
            catch
            {
            }
            finally
            {
                AuthenticationManager.SignOut();
            }
        }

        #region Change

        public async Task<SEIdentityResult> ChangePasswordAsync(int userId, string oldPassword, string newPassword)
        {
            var user = await FindUserByIdAsync(userId);
            var result = ValidatePassword(newPassword,
                new List<string> {user.FirstName, user.LastName, user.SecurityAnswer});
            if (!result.Succeeded) return result;
            await _userStore.ChangePasswordAsync(userId, oldPassword, newPassword);
            return new SEIdentityResult();
        }

        #endregion

        /// <summary>
        ///     The checks a password for minimum complexity, checks against a list of bad passwords and a list of banned words
        ///     (usually personal information)
        /// </summary>
        /// <param name="password"></param>
        /// <param name="bannedWords"></param>
        /// <returns></returns>
        public SEIdentityResult ValidatePassword(string password, List<string> bannedWords)
        {
            if (string.IsNullOrEmpty(password) || Regex.Matches(password, _passwordValidityRegex).Count == 0)
                return new SEIdentityResult(CONSTS.UserManagerMessages.PasswordValidityMessage);

            if (Regex.Matches(password, _passwordGoodEntropyRegex).Count == 0)
                return new SEIdentityResult(
                    "Your password cannot repeat the same character or digit more than 3 times consecutively, please choose another");

            var badPassword = _context.LookupItem.FirstOrDefault(l => l.LookupTypeId == CONSTS.LookupTypeId.BadPassword &&
                            string.Equals(l.Description, password, StringComparison.CurrentCultureIgnoreCase));
            if (badPassword != null)
                return new SEIdentityResult(
                    "Your password is on a list of easy to guess passwords, please choose another");

            foreach (var bannedWord in bannedWords)
                if (password.IndexOf(bannedWord, StringComparison.OrdinalIgnoreCase) >= 0)
                    return new SEIdentityResult("Your password cannot contain any of your personal information");
            return new SEIdentityResult();
        }

        #region Declarations

        private readonly IAppUserStore<User> _userStore;
        private readonly ISEContext _context;
        private readonly IAppConfiguration _configuration;
        private readonly IEncryption _encryption;

        private readonly string _passwordValidityRegex =
            @"^.*(?=.{8,100})(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z0-9]).*$";

        private readonly string _passwordGoodEntropyRegex = @"^(?!.*(.)\1{2})(.*?){3,29}$";

        #endregion

        #region Constructor

        public AppUserManager(IAppConfiguration configuration, ISEContext context, IEncryption encryption,
            IAppUserStore<User> userStore)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (encryption == null) throw new ArgumentNullException(nameof(encryption));
            if (userStore == null) throw new ArgumentNullException(nameof(userStore));

            _configuration = configuration;
            _context = context;
            _encryption = encryption;
            _userStore = userStore;
        }

        public AppUserManager() : this(new AppConfiguration(), new SEContext(), new Encryption(),
            new UserStore<User>(new SEContext(), new AppConfiguration()))
        {
            // TODO: Put in IoC container
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
            // free managed resources
            _context?.Dispose();
            _userStore?.Dispose();
        }

        #endregion

        #region Find

        public async Task<User> FindUserByIdAsync(int userId)
        {
            return await _userStore.FindByIdAsync(userId).ConfigureAwait(false);
        }

        public async Task<LogonResult> TryLogOnAsync(string userName, string password)
        {
            return await _userStore.TryLogOnAsync(userName, password).ConfigureAwait(false);
        }

        #endregion
    }
}