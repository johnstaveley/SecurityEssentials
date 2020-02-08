using SecurityEssentials.Acceptance.Tests.Extensions;
using SecurityEssentials.Acceptance.Tests.Pages;
using TechTalk.SpecFlow;

namespace SecurityEssentials.Acceptance.Tests.Steps
{
	[Binding]
	public class RegisterPageSteps
	{
		[When(@"I submit my registration details")]
		public void WhenISubmitMyRegistrationDetails()
		{
			var registerPage = ScenarioContext.Current.GetPage<RegisterPage>();
            FeatureContext.Current.WaitForRegistrationAttempt();
			registerPage.ClickSubmit();
		}

		[Given(@"I enter the following registration details:")]
		public void GivenIEnterTheFollowingRegistrationDetails(Table table)
		{
			var registerPage = ScenarioContext.Current.GetPage<RegisterPage>();
			registerPage.EnterDetails(table);
		}
	}
}
