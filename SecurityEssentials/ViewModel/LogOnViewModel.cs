using System.ComponentModel.DataAnnotations;

namespace SecurityEssentials.ViewModel
{
    public class LogOnViewModel
    {
        [Required]
        [Display(Name = "User name")]
		[StringLength(200, ErrorMessage = "The {0} must be at least {2} and less than {1} characters long.", MinimumLength = 7)]
		public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} and less than {1} characters long.", MinimumLength = 8)]
		[Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}