using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecurityEssentials.Acceptance.Tests.Extensions;
using SecurityEssentials.Acceptance.Tests.Pages;
using TechTalk.SpecFlow;

namespace SecurityEssentials.Acceptance.Tests.Steps
{
    [Binding]
    public class LandingPageSteps
    {
        [Then(@"the following last activity message is shown: '(.*)'")]
        public void ThenTheFollowingLastActivityMessageIsShown(string textToMatch)
        {
            var landingPage = ScenarioContext.Current.GetPage<LandingPage>();
            Assert.IsTrue(landingPage.GetLastAccountActivity().Contains(textToMatch));
        }
    }
}