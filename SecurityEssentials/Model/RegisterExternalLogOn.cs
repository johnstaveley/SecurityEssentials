using System.ComponentModel.DataAnnotations;

namespace SecurityEssentials.Model
{
    public class RegisterExternalLogOn
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        public string ExternalLogOnData { get; set; }
    }
}