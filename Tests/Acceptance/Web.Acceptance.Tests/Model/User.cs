namespace SecurityEssentials.Acceptance.Tests.Model
{
	public class User
	{

		public string UserName { get; set; }
		public string Title { get; set; }
		public string FirstName { get; set; }
		public string Surname { get; set; }
		public string MobileTelephoneNumber { get; set; }
		public bool Approved { get; set; }
		public bool EmailVerified { get; set; }
		public bool Enabled { get; set; }
	}
}
