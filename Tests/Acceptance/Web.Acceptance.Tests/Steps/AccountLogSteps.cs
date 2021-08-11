using SecurityEssentials.Acceptance.Tests.Extensions;
using SecurityEssentials.Acceptance.Tests.Pages;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SecurityEssentials.Acceptance.Tests.Steps
{
    [Binding]
	public class AccountLogSteps : TechTalk.SpecFlow.Steps
	{

		private readonly FeatureContext _featureContext;
        private readonly ScenarioContext _scenarioContext;

        public AccountLogSteps(FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            _featureContext = featureContext;
            _scenarioContext = scenarioContext;
        }

		[Then(@"I am shown the following user logs:")]
		public void ThenIAmShownTheFollowingUserLogs(Table table)
		{
			var page = _scenarioContext.GetPage<AccountLogPage>();
			var userLogsDisplayed = page.GetUserLogsDisplayed();
			table.CompareToSet(userLogsDisplayed);
		}


	}
}
