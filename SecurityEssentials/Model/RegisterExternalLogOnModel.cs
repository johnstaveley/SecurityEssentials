using System.ComponentModel.DataAnnotations;

namespace SecurityEssentials.Model
{
    public class RegisterExternalLogOnModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        public string ExternalLogOnData { get; set; }
    }
}