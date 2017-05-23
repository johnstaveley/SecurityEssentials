namespace SecurityEssentials.Acceptance.Tests.Model
{
	public class UserToCreate
	{

		public string UserName { get; set; }
		public string Title { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string MobileTelephoneNumber { get; set; }
		public string HomeTelephoneNumber { get; set; }
		public string WorkTelephoneNumber { get; set; }
		public string SkypeName { get; set; }
		public string Town { get; set; }
		public string Postcode { get; set; }
		public bool Approved { get; set; }
		public bool EmailVerified { get; set; }
		public bool Enabled { get; set; }
		public string Password { get; set; }
		public string SecurityQuestion { get; set; }
		public string SecurityAnswer { get; set; }
		public string PasswordResetToken { get; set; }
		public string PasswordResetExpiry { get; set; }
		public string NewEmailAddress { get; set; }
		public string NewEmailAddressToken { get; set; }
		public string NewEmailAddressRequestExpiryDate { get; set; }
		public bool IsAdmin { get; set; }
		public string PasswordExpiryDate { get; set; }
	}
}
