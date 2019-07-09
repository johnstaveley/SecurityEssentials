using Newtonsoft.Json;
using System;

namespace SecurityEssentials.Model
{

    public class CtHolder
    {
        [JsonProperty(PropertyName = "expect-ct-report")]
        public CtReport CtReport { get; set; }
    }

    public class CtReport
	{
		/// <summary>
		/// When the failure happened
		/// </summary>
		[JsonProperty(PropertyName = "date-time")]
		public DateTime FailureDate { get; set; }
		[JsonProperty(PropertyName = "hostname")]
		public string HostName { get; set; }
		[JsonProperty(PropertyName = "port")]
		public int Port { get; set; }
        /// <summary>
        /// When the policy will expire
        /// </summary>
		[JsonProperty(PropertyName = "effective-expiration-date")]
		public DateTime EffectiveExpirationDate { get; set; }
        /// <summary>
        /// SCTs as delivered to client
        /// </summary>
        [JsonProperty(PropertyName = "scts")]
        public Sct[] Scts { get; set; }
        /// <summary>
        /// The certificate chain as it was served to the client
        /// </summary>
        [JsonProperty(PropertyName = "served-certificate-chain")]
        public string[] ServedCertificateChain { get; set; }
        /// <summary>
        /// The full certificate chain the client built
        /// </summary>
        [JsonProperty(PropertyName = "validated-certificate-chain")]
        public string[] ValidatedCertificateChain { get; set; }
        [JsonProperty(PropertyName = "source")]
        public string Source { get; set; }
    }

    public class Sct
    {
        [JsonProperty(PropertyName = "serialized_sct")]
        public string SerialisedSct { get; set; }
        /// <summary>
        /// How the client got the SCT. Either embedded, ocsp or tls-extension
        /// </summary>
        [JsonProperty(PropertyName = "source")]
        public string Source { get; set; }
        /// <summary>
        /// Valid or otherwise
        /// </summary>
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }
        /// <summary>
        /// Version number. Currently either 1 or 2
        /// </summary>
        [JsonProperty(PropertyName = "version")]
        public string Version { get; set; }
    }

}
