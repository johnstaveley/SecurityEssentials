using System.ComponentModel.DataAnnotations;

namespace SecurityEssentials.Models
{
    public class RegisterExternalLogOnModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        public string ExternalLogOnData { get; set; }
    }
}