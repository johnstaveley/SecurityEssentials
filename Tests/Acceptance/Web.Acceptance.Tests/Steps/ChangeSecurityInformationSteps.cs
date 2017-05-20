using SecurityEssentials.Acceptance.Tests.Extensions;
using SecurityEssentials.Acceptance.Tests.Pages;
using TechTalk.SpecFlow;

namespace SecurityEssentials.Acceptance.Tests.Steps
{
    [Binding]
    public class ChangeSecurityInformationSteps
    {
        [Given(@"I enter the following change security information data:")]
        public void GivenIEnterTheFollowingChangeSecurityInformationData(Table table)
        {
            var changeSecurityInformationPage = ScenarioContext.Current.GetPage<ChangeSecurityInformationPage>();
            changeSecurityInformationPage.EnterDetails(table);
        }

        [When(@"I submit the change security information form")]
        public void WhenISubmitTheChangeSecurityInformationForm()
        {
            var changeSecurityInformationPage = ScenarioContext.Current.GetPage<ChangeSecurityInformationPage>();
            changeSecurityInformationPage.ClickSubmit();
        }
    }
}