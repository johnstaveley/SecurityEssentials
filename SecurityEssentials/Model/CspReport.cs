using Newtonsoft.Json;

namespace SecurityEssentials.Model
{
	public class CspHolder
	{
		[JsonProperty(PropertyName = "csp-report")]
		public CspReport CspReport { get; set; }
	}

	public class CspReport
	{
		/// <summary>
		/// The URI of the document in which the violation occurred.
		/// </summary>
		[JsonProperty(PropertyName = "document-uri")]
		public string DocumentUri { get; set; }

		/// <summary>
		/// The referrer of the document in which the violation occurred.
		/// </summary>
		[JsonProperty(PropertyName = "referrer")]
		public string Referrer { get; set; }

		/// <summary>
		/// The URI of the resource that was blocked from loading by the Content Security Policy. If the blocked URI is from a different origin than the document-uri, then the blocked URI is truncated to contain just the scheme, host, and port.
		/// </summary>
		[JsonProperty(PropertyName = "blocked-uri")]
		public string BlockedUri { get; set; }

		[JsonProperty(PropertyName = "line-number")]
		public string LineNumber { get; set; }

		/// <summary>
		/// The name of the policy section that was violated 
		/// </summary>
		[JsonProperty(PropertyName = "violated-directive")]
		public string ViolatedDirective { get; set; }

		/// <summary>
		/// The original policy as specified by the Content-Security-Policy HTTP header
		/// </summary>
		[JsonProperty(PropertyName = "original-policy")]
		public string OriginalPolicy { get; set; }

		[JsonProperty(PropertyName = "script-sample")]
		public string ScriptSample { get; set; }

		[JsonProperty(PropertyName = "source-file")]
		public string SourceFile { get; set; }
	}
}
