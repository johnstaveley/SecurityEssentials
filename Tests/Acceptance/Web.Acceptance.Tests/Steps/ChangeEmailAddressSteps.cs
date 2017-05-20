using SecurityEssentials.Acceptance.Tests.Extensions;
using SecurityEssentials.Acceptance.Tests.Pages;
using TechTalk.SpecFlow;

namespace SecurityEssentials.Acceptance.Tests.Steps
{
    [Binding]
    public class ChangeEmailAddressSteps
    {
        [When(@"I submit the change email address form")]
        public void WhenISubmitTheChangeEmailAddressForm()
        {
            var changeEmailAddressPage = ScenarioContext.Current.GetPage<ChangeEmailAddressPage>();
            changeEmailAddressPage.ClickSubmit();
        }

        [When(@"I navigate to the change email address link with token '(.*)'")]
        public void WhenINavigateToTheChangeEmailAddressLinkWithToken(string newUserNameToken)
        {
            var changeEmailAddressConfirmPage =
                ChangeEmailAddressConfirmPage.NavigateToPage(FeatureContext.Current.GetWebDriver(),
                    FeatureContext.Current.GetBaseUri(), newUserNameToken);
        }


        [Given(@"I enter the following change email address data:")]
        public void GivenIEnterTheFollowingChangeEmailAddressData(Table table)
        {
            var changeEmailAddressPage = ScenarioContext.Current.GetPage<ChangeEmailAddressPage>();
            changeEmailAddressPage.EnterDetails(table);
        }
    }
}