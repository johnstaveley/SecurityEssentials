namespace SecurityEssentials.Core.Constants
{
	public static class Consts
	{
		public static class LookupTypeId
		{
			public static readonly int BadPassword = 1;
			public static readonly int SecurityQuestion = 2;
		}
		public static class Roles
		{
			public static readonly int Admin = 1;
		}

		public static class UserManagerMessages
		{
			public static readonly string PasswordValidityMessage = "Your password must consist of 8 characters, digits or special characters and must contain at least 1 uppercase, 1 lowercase and 1 numeric value";
		}
	}
}