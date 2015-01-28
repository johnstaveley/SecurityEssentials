using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SecurityEssentials.Model;
using SecurityEssentials.Core;

namespace SecurityEssentials.Core.Identity
{
    public interface IUserManager
    {
		Task<SEIdentityResult> CreateAsync(string userName, string firstName, string lastName, string password, string passwordConfirmation, string email);
        Task SignInAsync(string userName, bool isPersistent);
        Task<User> FindAsync(string userName, string password);
        Task<User> FindById(int userId);
        Task<User> FindByEmailAsync(string email);
        void SignOut();
        Task<SEIdentityResult> ChangePasswordAsync(int userId, string oldPassword, string newPassword);
        Task<SEIdentityResult> ChangePasswordFromTokenAsync(int userId, string oldPassword, string newPassword);
        Task<int> GeneratePasswordResetTokenAsync(int userId);
    }
}
