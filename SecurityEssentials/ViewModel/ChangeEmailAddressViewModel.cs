using System;
using System.ComponentModel.DataAnnotations;

namespace SecurityEssentials.ViewModel
{
    public class ChangeEmailAddressViewModel
    {
        public ChangeEmailAddressViewModel(string emailAddress, string newEmailAddress,
            DateTime? newUserNameRequestExpiryDate)
        {
            EmailAddress = emailAddress;
            NewEmailAddress = newEmailAddress;
            NewEmailAddressRequestExpiryDate = newUserNameRequestExpiryDate?.ToLocalTime();
            IsFormLocked = newUserNameRequestExpiryDate.HasValue &&
                           newUserNameRequestExpiryDate.Value > DateTime.UtcNow;
        }

        public ChangeEmailAddressViewModel()
        {
        }

        [Display(Name = "Current Email Address")]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and less than {1} characters long.",
            MinimumLength = 7)]
        public string EmailAddress { get; set; }

        [Display(Name = "Enter New Email Address")]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and less than {1} characters long.",
            MinimumLength = 7)]
        public string NewEmailAddress { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and less than {1} characters long.",
            MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Re-Enter Password")]
        public string Password { get; set; }

        public DateTime? NewEmailAddressRequestExpiryDate { get; set; }

        public bool IsFormLocked { get; set; }
    }
}