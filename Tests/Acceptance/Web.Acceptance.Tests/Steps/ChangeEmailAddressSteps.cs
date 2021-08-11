using SecurityEssentials.Acceptance.Tests.Extensions;
using SecurityEssentials.Acceptance.Tests.Pages;
using TechTalk.SpecFlow;

namespace SecurityEssentials.Acceptance.Tests.Steps
{
	[Binding]
	public class ChangeEmailAddressSteps : TechTalk.SpecFlow.Steps
	{

		private readonly FeatureContext _featureContext;
        private readonly ScenarioContext _scenarioContext;

        public ChangeEmailAddressSteps(FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            _featureContext = featureContext;
            _scenarioContext = scenarioContext;
        }

		[When(@"I submit the change email address form")]
		public void WhenISubmitTheChangeEmailAddressForm()
		{
			var changeEmailAddressPage = _scenarioContext.GetPage<ChangeEmailAddressPage>();
			changeEmailAddressPage.ClickSubmit();
		}

		[When(@"I navigate to the change email address link with token '(.*)'")]
		public void WhenINavigateToTheChangeEmailAddressLinkWithToken(string newUserNameToken)
		{
			ChangeEmailAddressConfirmPage.NavigateToPage(_featureContext.GetWebDriver(), _featureContext.GetBaseUri(), newUserNameToken);
		}

		[Given(@"I enter the following change email address data:")]
		public void GivenIEnterTheFollowingChangeEmailAddressData(Table table)
		{
			var changeEmailAddressPage = _scenarioContext.GetPage<ChangeEmailAddressPage>();
			changeEmailAddressPage.EnterDetails(table);
		}
	}
}
