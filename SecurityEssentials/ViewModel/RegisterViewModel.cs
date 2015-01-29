using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Web.Mvc;
using SecurityEssentials.Model;

namespace SecurityEssentials.ViewModel
{
    public class RegisterViewModel
    {

        #region Declarations

		[Required, MinLength(8)]
		public string ConfirmPassword { get; set; }

		[Required, MinLength(8)]
		public string Password { get; set; }

		public SelectList SecurityQuestions { get; set; }

		public User User { get; set; }

        #endregion

        #region Constructor

        public RegisterViewModel(string confirmPassword, string password, User user, List<LookupItem> securityQuestions)            
        {
			ConfirmPassword = confirmPassword;
			Password = password;
			User = user;
			SecurityQuestions = new SelectList(securityQuestions, "Id", "Description");
        }

        #endregion

    }
}