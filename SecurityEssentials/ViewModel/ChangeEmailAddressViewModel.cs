using System;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Linq;
using SecurityEssentials.Model;
using System.ComponentModel.DataAnnotations;

namespace SecurityEssentials.ViewModel
{

    public class ChangeEmailAddressViewModel
    {

		[Display(Name ="Current Email Address")]
		public string EmailAddress { get; set; }

		[Display(Name = "Enter New Email Address")]
		public string NewEmailAddress { get; set; }
		[Required]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "Re-Enter Password")]
		public string Password { get; set; }

		public DateTime? NewEmailAddressRequestExpiryDate { get; set; }

		public bool IsFormLocked { get; set; }

		public ChangeEmailAddressViewModel(string emailAddress, string newEmailAddress, DateTime? newUserNameRequestExpiryDate)
        {
            EmailAddress = emailAddress;
			NewEmailAddress = newEmailAddress;
			NewEmailAddressRequestExpiryDate = newUserNameRequestExpiryDate.HasValue ? (DateTime?) newUserNameRequestExpiryDate.Value.ToLocalTime() : null;
			IsFormLocked = newUserNameRequestExpiryDate.HasValue ? newUserNameRequestExpiryDate.Value > DateTime.UtcNow : false;

		}

		public ChangeEmailAddressViewModel()
		{

		}
    }
}