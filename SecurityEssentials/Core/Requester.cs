using SecurityEssentials.Core.Constants;

namespace SecurityEssentials.Core
{

	/// <summary>
	/// Class used to log additional information relating to a request in serilog
	/// </summary>
	public class Requester
	{

		public string IpAddress { get; set; }

		public string LoggedOnUser { get; set; }

		public int? LoggedOnUserId { get; set; }

		public AppSensorDetectionPointKind? AppSensorDetectionPoint { get; set; }

		public string SessionId { get; set; }

	}
}