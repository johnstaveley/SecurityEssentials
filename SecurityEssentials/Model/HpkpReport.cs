using Newtonsoft.Json;

namespace SecurityEssentials.Model
{

    public class HpkpReport
	{
		/// <summary>
		/// 
		/// </summary>
		[JsonProperty(PropertyName = "date-time")]
		public string DateTime { get; set; }

		[JsonProperty(PropertyName = "hostname")]
		public string HostName { get; set; }

		[JsonProperty(PropertyName = "port")]
		public string Port { get; set; }

		[JsonProperty(PropertyName = "effective-expiration-date")]
		public string ExpirationDate { get; set; }

		[JsonProperty(PropertyName = "include-subdomains")]
		public string IncludeSubDomains { get; set; }

		[JsonProperty(PropertyName = "noted-hostname")]
		public string NotedHostName { get; set; }

		[JsonProperty(PropertyName = "served-certificate-chain")]
		public string[] ServedCertificateChain { get; set; }

		[JsonProperty(PropertyName = "validated-certificate-chain")]
		public string[] ValidatedCertificateChain { get; set; }

		[JsonProperty(PropertyName = "known-pins")]
		public string[] KnownPins { get; set; }

	}
}
