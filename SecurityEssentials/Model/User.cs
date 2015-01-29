using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SecurityEssentials.Model
{

    public class User : IUser<int>
    {

		public int Id { get; set; }
        /// <summary>
        /// Username used to log into the application
        /// </summary>
        [Required]
        public string UserName { get; set; }
        /// <summary>
        /// An enryption salt which is used to encrypt the password
        /// </summary>
        public string Salt { get; set; }
        /// <summary>
        /// A hashed version of the password, uses the salt
        /// </summary>
        public string PasswordHash { get; set; }
        /// <summary>
        /// The date the user was created
        /// </summary> 
        public DateTime DateCreated { get; set; }
        /// <summary>
        /// Whether the user can login or not
        /// </summary>
        public bool Enabled { get; set; } 
        /// <summary>
        /// Mr, Mrs etc
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// The number of failed logon attempts made to this user account
        /// </summary>
        public int FailedLogonAttemptCount { get; set; }
        [Display(Name = "First Name"), Required, MaxLength(100)]
        public string FirstName { get; set; }
        [Display(Name = "Last Name"), Required, MaxLength(100)]
        public string LastName { get; set; }

        /// <summary>
        /// The email address of the user
        /// </summary>
        [MaxLength(200)]
        public string Email { get; set; }

        [Display(Name = "Home Telephone number"), MaxLength(200)]
        public string TelNoHome { get; set; }

        [Display(Name = "Work Telephone number"), MaxLength(200)]
        public string TelNoWork { get; set; }

        [Display(Name = "Mobile Telephone number"), MaxLength(200)]
        public string TelNoMobile { get; set; }

        [MaxLength(200)]
        public string Town { get; set; }
        public string Postcode { get; set; }

        [Display(Name = "Skype Name")]
        public string SkypeName { get; set; }

        /// <summary>
        /// A question known to the user which can be used to reset the password
        /// </summary>
        public string SecurityQuestion { get; set; }
        /// <summary>
        /// The answer to the security question known to the user which can be used to reset the password
        /// </summary>
        public string SecurityAnswer { get; set; }
        /// <summary>
        /// A token which can be used to reset the password which is emailed to the user
        /// </summary>
        public string PasswordResetToken { get; set; }
        /// <summary>
        /// The expiry date and time for the token to reset the password
        /// </summary>
        public DateTime? PasswordResetExpiry { get; set; }

        // Reverse navigation
        public virtual ICollection<UserRole> UserRoles { get; set; }

        public User()
        {

            FailedLogonAttemptCount = 0;
            UserRoles = new List<UserRole>();
        }

		/// <summary>
		/// READONLY: FirstName concatenated with LastName
		/// </summary>
		[NotMapped]
		public string FullName
		{
			get
			{
				return string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0} {1}", this.FirstName, this.LastName).Trim();
			}
		}

    }
}
