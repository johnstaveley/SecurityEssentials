using Microsoft.AspNet.Identity;
using SecurityEssentials.Core.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecurityEssentials.Model
{

	public class User : IUser<int>
	{

		public int Id { get; set; }

		/// <summary>
		/// The email address of the user and used as the username to login to the application
		/// </summary>
		[MaxLength(200), Required, MinLength(7), Display(Name = "Email Address"),
		 RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,4})+)$", ErrorMessage = "The {0} does not appear to be valid"),
		 Index(IsUnique = true)]
		public string UserName { get; set; }

		/// <summary>
		/// A salt which is used to hash the password
		/// </summary>
		[MaxLength(500)]
		public string PasswordSalt { get; set; }

		/// <summary>
		/// A hashed version of the password, uses the salt
		/// </summary>
		[MaxLength(500)]
		public string PasswordHash { get; set; }

		/// <summary>
		/// The algorithm to use to create the hash
		/// </summary>
		public HashStrategyKind HashStrategy { get; set; }

		/// <summary>
		/// The date the password was last changed (UTC)
		/// </summary>
		[Display(Name = "Password Last Changed Date")]
		public DateTime PasswordLastChangedDateUtc { get; set; }

		/// <summary>
		/// The date the user was created (UTC)
		/// </summary> 
		[Display(Name = "Date Created")]
		public DateTime CreatedDateUtc { get; set; }

		/// <summary>
		/// Whether the user can login or not i.e. has been locked out for whatever reason
		/// </summary>
		public bool Enabled { get; set; }

		/// <summary>
		/// Whether a user you has registered online is approved or not, this can be a manual or automatic process
		/// </summary>
		public bool Approved { get; set; }

		/// <summary>
		/// Whether the email address has been verified or not
		/// </summary>
		[Display(Name = "Email Verified")]
		public bool EmailVerified { get; set; }

		/// <summary>
		/// Mr, Mrs etc
		/// </summary>
		[MaxLength(20)]
		public string Title { get; set; }

		/// <summary>
		/// The number of failed logon attempts made to this user account
		/// </summary>
		public int FailedLogonAttemptCount { get; set; }

		[Display(Name = "First Name"), Required, MaxLength(100)]
		public string FirstName { get; set; }

		[Display(Name = "Last Name"), Required, MaxLength(100)]
		public string LastName { get; set; }

		[Display(Name = "Home Telephone number"), MaxLength(200)]
		public string TelNoHome { get; set; }

		[Display(Name = "Work Telephone number"), MaxLength(200)]
		public string TelNoWork { get; set; }

		[Display(Name = "Mobile Telephone number"), MaxLength(200)]
		public string TelNoMobile { get; set; }

		[MaxLength(200)]
		public string Town { get; set; }

		[MaxLength(20)]
		public string Postcode { get; set; }

		[Display(Name = "Skype Name"), MaxLength(100)]
		public string SkypeName { get; set; }

		/// <summary>
		/// A question known to the user which can be used to reset the password
		/// </summary>
		[Required, Display(Name = "Security Question"),
		 Range(1, 10000, ErrorMessage = "The Security Question field is required.")]
		public int SecurityQuestionLookupItemId { get; set; }

		/// <summary>
		/// A hash salt which is used to encrypt the security answer
		/// </summary>
		[MaxLength(500)]
		public string SecurityAnswerSalt { get; set; }

		/// <summary>
		/// The encrypted answer to the security question known to the user which can be used to reset the password
		/// </summary>
		[Required, Display(Name = "Security Answer (Case Sensitive)"), MinLength(4), MaxLength(40)]
		public string SecurityAnswer { get; set; }

		/// <summary>
		/// A token which can be used to confirm the email address is valid
		/// </summary>
		[MaxLength(500)]
		public string EmailConfirmationToken { get; set; }

		/// <summary>
		/// Initiated by User, The expiry date and time for the token to reset the password (UTC)
		/// </summary>
		public DateTime? PasswordResetExpiryDateUtc { get; set; }

		/// <summary>
		/// Initiated by User, A token which can be used to reset the password which is emailed to the user
		/// </summary>
		[MaxLength(500)]
		public string PasswordResetToken { get; set; }

		/// <summary>
		/// An optional date time indicating when the user must next change their password
		/// </summary>
		public DateTime? PasswordExpiryDateUtc { get; set; }

		/// <summary>
		/// Any new email address change request 
		/// </summary>
		[MaxLength(200), MinLength(7), Display(Name = "New Email Address"), RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,4})+)$", ErrorMessage = "This does not appear to be a valid email address")]
		public string NewEmailAddress { get; set; }

		/// <summary>
		/// A token which can be used to change the email address/user name which is emailed to the user
		/// </summary>
		[MaxLength(500)]
		public string NewEmailAddressToken { get; set; }

		/// <summary>
		/// The expiry date and time for the token to change the email address (UTC)
		/// </summary>
		public DateTime? NewEmailAddressRequestExpiryDateUtc { get; set; }

		// Foreign Key
		public virtual LookupItem SecurityQuestionLookupItem { get; set; }

		// Reverse navigation
		public virtual ICollection<PreviousPassword> PreviousPasswords { get; set; }
		public virtual ICollection<UserRole> UserRoles { get; set; }
		public virtual ICollection<UserLog> UserLogs { get; set; }

		public User()
		{
			Approved = false;
			CreatedDateUtc = DateTime.UtcNow;
			FailedLogonAttemptCount = 0;
			PreviousPasswords = new List<PreviousPassword>();
			UserLogs = new List<UserLog>();
			UserRoles = new List<UserRole>();
		}

		/// <summary>
		/// READONLY: FirstName concatenated with LastName
		/// </summary>
		[NotMapped]
		public string FullName => string
			.Format(System.Globalization.CultureInfo.CurrentCulture, "{0} {1}", FirstName, LastName).Trim();

		/// <summary>
		/// Whether the user can be deleted or not
		/// </summary>
		/// <remarks>TODO: If user has changed auditable application data then this should return false</remarks>
		[NotMapped]
		public bool CanBeDeleted => true;
	}
}