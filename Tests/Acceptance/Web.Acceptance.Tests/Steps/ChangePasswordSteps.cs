using SecurityEssentials.Acceptance.Tests.Extensions;
using SecurityEssentials.Acceptance.Tests.Pages;
using TechTalk.SpecFlow;

namespace SecurityEssentials.Acceptance.Tests.Steps
{
    [Binding]
    public class ChangePasswordSteps
    {
        [Given(@"I enter the following change password data:")]
        public void GivenIEnterTheFollowingChangePasswordData(Table table)
        {
            var changePasswordPage = ScenarioContext.Current.GetPage<ChangePasswordPage>();
            changePasswordPage.EnterDetails(table);
        }

        [When(@"I submit the change password form")]
        public void WhenISubmitTheChangePassordForm()
        {
            var changePasswordPage = ScenarioContext.Current.GetPage<ChangePasswordPage>();
            changePasswordPage.ClickSubmit();
        }
    }
}