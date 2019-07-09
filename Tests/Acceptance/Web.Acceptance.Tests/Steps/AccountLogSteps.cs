using SecurityEssentials.Acceptance.Tests.Extensions;
using SecurityEssentials.Acceptance.Tests.Pages;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SecurityEssentials.Acceptance.Tests.Steps
{
    [Binding]
	public class AccountLogSteps
	{

		[Then(@"I am shown the following user logs:")]
		public void ThenIAmShownTheFollowingUserLogs(Table table)
		{
			var page = ScenarioContext.Current.GetPage<AccountLogPage>();
			var userLogsDisplayed = page.GetUserLogsDisplayed();
			table.CompareToSet(userLogsDisplayed);
		}


	}
}
