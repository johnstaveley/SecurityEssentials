using SecurityEssentials.Core.Constants;

namespace SecurityEssentials.Core
{

	/// <summary>
	/// Class used to log additional information relating to a request
	/// </summary>
	public class Requester
	{
		/// <summary>
		/// Source IP Address of the request
		/// </summary>
		public string IpAddress { get; set; }
        /// <summary>
        /// Any user name of the user if they are logged on
        /// </summary>
		public string LoggedOnUser { get; set; }
		/// <summary>
		/// Any user id of the user if they are logged on
		/// </summary>
		public int? LoggedOnUserId { get; set; }

		public AppSensorDetectionPointKind? AppSensorDetectionPoint { get; set; }
        /// <summary>
		/// A hash of the user's session id so unauthenticated requests can be tracked
		/// </summary>
		public string SessionHash { get; set; }

        public override string ToString()
        {
            var description = $"IpAddress: {IpAddress}, ";
            if (!string.IsNullOrEmpty(LoggedOnUser))
            {
                description += $"LoggedOnUser: {LoggedOnUser}, ";
            }
            if (LoggedOnUserId.HasValue)
            {
                description += $"LoggedOnUserId: {LoggedOnUserId}, ";
            }
            if (!string.IsNullOrEmpty(SessionHash))
            {
                description += $"Session Hash: {SessionHash}, ";
            }
            return description.Trim(' ').Trim(',');
        }
	}
}