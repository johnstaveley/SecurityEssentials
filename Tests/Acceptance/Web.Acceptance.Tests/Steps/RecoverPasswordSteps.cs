using SecurityEssentials.Acceptance.Tests.Extensions;
using SecurityEssentials.Acceptance.Tests.Pages;
using TechTalk.SpecFlow;

namespace SecurityEssentials.Acceptance.Tests.Steps
{
    [Binding]
    public class RecoverPasswordSteps : TechTalk.SpecFlow.Steps
    {
        private readonly FeatureContext _featureContext;
        private readonly ScenarioContext _scenarioContext;

        public RecoverPasswordSteps(FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            _featureContext = featureContext;
            _scenarioContext = scenarioContext;
        }
        [Given(@"I enter the following recover password data:")]
        public void GivenIEnterTheFollowingRecoverData(Table table)
        {
            var recoverPasswordPage = _scenarioContext.GetPage<RecoverPasswordPage>();
            recoverPasswordPage.EnterDetails(table);
        }

        [When(@"I submit the recover passord form")]
        public void WhenISubmitTheRecoverPassordForm()
        {
            var recoverPasswordPage = _scenarioContext.GetPage<RecoverPasswordPage>();
            recoverPasswordPage.ClickSubmit();
        }

        [Given(@"I navigate to the password reset link with token '(.*)'")]
        public void GivenINavigateToPasswordResetLinkWithToken(string passwordResetToken)
        {
            RecoverPasswordPage.NavigateToPage(_featureContext.GetWebDriver(), _featureContext.GetBaseUri(), passwordResetToken);

        }
    }
}
