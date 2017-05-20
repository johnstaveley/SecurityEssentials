using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecurityEssentials.Acceptance.Tests.Extensions;
using SecurityEssentials.Acceptance.Tests.Pages;
using TechTalk.SpecFlow;

namespace SecurityEssentials.Acceptance.Tests.Steps
{
    [Binding]
    public class AccountLogSteps
    {
        [Then(@"I am shown the message '(.*)'")]
        public void ThenIAmShownTheMessage(string message)
        {
            var page = ScenarioContext.Current.GetPage<AccountLogPage>();
            Assert.IsTrue(page.GetMostRecentMessage().Contains(message));
        }
    }
}