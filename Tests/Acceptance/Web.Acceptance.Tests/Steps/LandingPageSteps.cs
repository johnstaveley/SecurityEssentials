using NUnit.Framework;
using SecurityEssentials.Acceptance.Tests.Extensions;
using SecurityEssentials.Acceptance.Tests.Pages;
using TechTalk.SpecFlow;

namespace SecurityEssentials.Acceptance.Tests.Steps
{
    [Binding]
    public class LandingPageSteps : TechTalk.SpecFlow.Steps
    {
        private readonly FeatureContext _featureContext;
        private readonly ScenarioContext _scenarioContext;

        public LandingPageSteps(FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            _featureContext = featureContext;
            _scenarioContext = scenarioContext;
        }
        [Then(@"the following last activity message is shown: '(.*)'")]
        public void ThenTheFollowingLastActivityMessageIsShown(string textToMatch)
        {
            var landingPage = _scenarioContext.GetPage<LandingPage>();
            Assert.IsTrue(landingPage.GetLastAccountActivity().Contains(textToMatch), $"Expected text '{textToMatch}' but was '{landingPage.GetLastAccountActivity()}'");
        }
    }
}
