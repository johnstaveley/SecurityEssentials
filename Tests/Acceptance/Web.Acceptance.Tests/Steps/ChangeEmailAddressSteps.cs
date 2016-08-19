using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecurityEssentials.Acceptance.Tests.Web.Pages;
using TechTalk.SpecFlow;

namespace SecurityEssentials.Acceptance.Tests.Web.Extensions
{
	[Binding]
	public class ChangeEmailAddressSteps
	{

		[When(@"I navigate to the change email address link with token '(.*)'")]
		public void WhenINavigateToTheChangeEmailAddressLinkWithToken(string newUserNameToken)
		{
			var changeEmailAddressConfirmPage = ChangeEmailAddressConfirmPage.NavigateToPage(FeatureContext.Current.GetWebDriver(), FeatureContext.Current.GetBaseUri(), newUserNameToken);
		}


		[Given(@"I enter the following change email address data:")]
		public void GivenIEnterTheFollowingChangeEmailAddressData(Table table)
		{
			var changeEmailAddressPage = ScenarioContext.Current.GetPage<ChangeEmailAddressPage>();
			changeEmailAddressPage.EnterDetails(table);
		}


	}
}
