using NUnit.Framework;
using SecurityEssentials.Acceptance.Tests.Extensions;
using SecurityEssentials.Acceptance.Tests.Pages;
using TechTalk.SpecFlow;

namespace SecurityEssentials.Acceptance.Tests.Steps
{
    [Binding]
    public class RecoverSteps : TechTalk.SpecFlow.Steps
    {
        private readonly FeatureContext _featureContext;
        private readonly ScenarioContext _scenarioContext;

        public RecoverSteps(FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            _featureContext = featureContext;
            _scenarioContext = scenarioContext;
        }
        [Given(@"I enter the following recover data:")]
        public void GivenIEnterTheFollowingRecoverData(Table table)
        {
            var recoverPage = _scenarioContext.GetPage<RecoverPage>();
            recoverPage.EnterDetails(table);
        }

        [When(@"I submit the recover form")]
        public void WhenISubmitTheRecoverForm()
        {
            var recoverPage = _scenarioContext.GetPage<RecoverPage>();
            recoverPage.ClickSubmit();
        }

        [Given(@"I am taken to the password recovery page")]
        public void GivenIAmTakenToThePasswordRecoveryPage()
        {
            BasePage page = _scenarioContext.GetPage<RecoverPage>();
            Assert.That(page.IsCurrentPage);
        }

    }
}
