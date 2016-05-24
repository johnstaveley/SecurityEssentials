using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Web.Mvc;
using SecurityEssentials.Model;

namespace SecurityEssentials.ViewModel
{
    public class ChangeSecurityInformationViewModel
    {

        #region Declarations

        [Required, MaxLength(20)]
        public string Password { get; set; }

        public SelectList SecurityQuestions { get; set; }

        [Required, Display(Name = "New Security Question")]
        public int SecurityQuestionLookupItemId { get; set; }

		[Required, Display(Name = "New Security Answer"), MinLength(4), MaxLength(40)]
        public string SecurityAnswer { get; set; }

        [Required, Display(Name = "New Security Answer Confirmation"), MinLength(4), MaxLength(40)]
        public string SecurityAnswerConfirm { get; set; }

        public string ErrorMessage { get; set; }

        #endregion

        #region Constructor

        public ChangeSecurityInformationViewModel(List<LookupItem> securityQuestions, string errorMessage)
        {
            SecurityQuestions = new SelectList(securityQuestions, "Id", "Description");
            ErrorMessage = errorMessage;
        }

        public ChangeSecurityInformationViewModel()
        {

        }

        #endregion

    }
}