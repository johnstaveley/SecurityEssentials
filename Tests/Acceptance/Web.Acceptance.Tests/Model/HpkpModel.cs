using System;

namespace SecurityEssentials.Acceptance.Tests.Model
{
	public class HpkpModel
	{
		public DateTime DateTime { get; set; }

		public string HostName { get; set; }

		public string Port { get; set; }

		public DateTime ExpirationDate { get; set; }

		public Boolean IncludeSubDomains { get; set; }

		public string NotedHostName { get; set; }

		public string ServedCertificateChainDelimited { get; set; }

		public string ValidatedCertificateChainDelimited { get; set; }

		public string KnownPinsDelimited { get; set; }
	}
}
