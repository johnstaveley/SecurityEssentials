using SecurityEssentials.Acceptance.Tests.Extensions;
using SecurityEssentials.Acceptance.Tests.Pages;
using SecurityEssentials.Model;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SecurityEssentials.Acceptance.Tests.Steps
{
	[Binding]
	public class LogSteps
	{

		[Then(@"I am shown the following logs:")]
		public void ThenIAmShownTheFollowingLogs(Table table)
		{
			var page = ScenarioContext.Current.GetPage<LogPage>();
			var recordsDisplayed = page.GetLogsDisplayed();
			recordsDisplayed.Remove(new Log { Level = "Information", Message = "Application Started" });
			table.CompareToSet(recordsDisplayed);
		}		

	}
}
