using SecurityEssentials.Acceptance.Tests.Extensions;
using SecurityEssentials.Acceptance.Tests.Pages;
using TechTalk.SpecFlow;

namespace SecurityEssentials.Acceptance.Tests.Steps
{
    [Binding]
    public class LoginPageSteps : TechTalk.SpecFlow.Steps
    {
        private readonly FeatureContext _featureContext;
        private readonly ScenarioContext _scenarioContext;

        public LoginPageSteps(FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            _featureContext = featureContext;
            _scenarioContext = scenarioContext;
        }
        [Given(@"I click recover password")]
        public void GivenIClickRecoverPassword()
        {
            var loginPage = _scenarioContext.GetPage<LoginPage>();
            loginPage.ClickRecoverPassword();
        }

        [Given(@"I click the login button")]
        [When(@"I click the login button")]
        [Then(@"I click the login button")]
        public void GivenIClickTheLoginButton()
        {
            var loginPage = _scenarioContext.GetPage<LoginPage>();
            _featureContext.WaitForLoginAttempt();
            loginPage.ClickSubmit();
        }

        [Given(@"I click the login button as quickly as possible")]
        [When(@"I click the login button as quickly as possible")]
        public void GivenIClickTheLoginButtonAsQuicklyAsPossible()
        {
            var loginPage = _scenarioContext.GetPage<LoginPage>();
            loginPage.ClickSubmit();
        }

        [Given(@"I enter the following login data:")]
        [Then(@"I enter the following login data:")]
        public void GivenIEnterTheFollowingLoginData(Table table)
        {
            var loginPage = _scenarioContext.GetPage<LoginPage>();
            loginPage.EnterDetails(table);
        }


    }
}
