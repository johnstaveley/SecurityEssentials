using SecurityEssentials.Acceptance.Tests.Extensions;
using SecurityEssentials.Acceptance.Tests.Pages;
using TechTalk.SpecFlow;

namespace SecurityEssentials.Acceptance.Tests.Steps
{
    [Binding]
    public class RegisterPageSteps : TechTalk.SpecFlow.Steps
    {
        private readonly FeatureContext _featureContext;
        private readonly ScenarioContext _scenarioContext;

        public RegisterPageSteps(FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            _featureContext = featureContext;
            _scenarioContext = scenarioContext;
        }
        [When(@"I submit my registration details")]
        public void WhenISubmitMyRegistrationDetails()
        {
            var registerPage = _scenarioContext.GetPage<RegisterPage>();
            _featureContext.WaitForRegistrationAttempt();
            registerPage.ClickSubmit();
        }

        [Given(@"I enter the following registration details:")]
        public void GivenIEnterTheFollowingRegistrationDetails(Table table)
        {
            var registerPage = _scenarioContext.GetPage<RegisterPage>();
            registerPage.EnterDetails(table);
        }
    }
}
