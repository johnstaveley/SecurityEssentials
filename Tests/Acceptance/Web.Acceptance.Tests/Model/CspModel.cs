namespace SecurityEssentials.Acceptance.Tests.Model
{
	public class CspModel
	{
		public string BlockedUri { get; set; }
		public string DocumentUri { get; set; }
		public string LineNumber { get; set; }
		public string Referrer { get; set; }
		public string OriginalPolicy { get; set; }
		public string ScriptSample { get; set; }
		public string SourceFile { get; set; }
		public string ViolatedDirective { get; set; }
	}
}
