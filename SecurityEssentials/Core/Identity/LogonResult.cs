namespace SecurityEssentials.Core.Identity
{
	public class LogonResult
	{

		public string UserName { get; set; }

		public bool Success { get; set; }

		public int FailedLogonAttemptCount { get; set; }

		/// <summary>
		/// Used by AppSensor to indicate if one of a batch of common usernames has been used
		/// </summary>
		public bool IsCommonUserName { get; set; }

	}
}