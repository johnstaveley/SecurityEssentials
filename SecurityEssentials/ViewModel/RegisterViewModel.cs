using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using SecurityEssentials.Model;

namespace SecurityEssentials.ViewModel
{
    public class RegisterViewModel
    {
        #region Constructor

        public RegisterViewModel(string confirmPassword, bool hasRecaptcha, string password, User user,
            IEnumerable<LookupItem> securityQuestions)
        {
            ConfirmPassword = confirmPassword;
            HasRecaptcha = hasRecaptcha;
            Password = password;
            User = user;
            SecurityQuestions = new SelectList(securityQuestions, "Id", "Description");
        }

        #endregion

        #region Declarations

        [Required]
        [MinLength(8)]
        public string ConfirmPassword { get; set; }

        [Required]
        [MinLength(8)]
        public string Password { get; set; }

        public SelectList SecurityQuestions { get; set; }

        public User User { get; set; }

        public bool HasRecaptcha { get; set; }

        #endregion
    }
}