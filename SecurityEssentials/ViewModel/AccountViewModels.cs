using System;
using System.ComponentModel.DataAnnotations;

namespace SecurityEssentials.ViewModel
{

    public class Recover
    {
        [Required]
        [Display(Name = "Email Address")]
        public string UserName { get; set; }

    }

    public class RecoverPassword
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Display(Name = "Security question")]
        public string SecurityQuestion { get; set; }

        [Required]
        [Display(Name = "Answer to security question")]
        [MaxLength(20, ErrorMessage="The length of the security answer is too long")]
        public string SecurityAnswer { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Enter new Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Required]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public int Id { get; set; }

        public string PasswordResetToken { get; set; }
    }

}
