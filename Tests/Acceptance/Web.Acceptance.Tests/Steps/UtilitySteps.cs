using System.Threading;
using TechTalk.SpecFlow;

namespace SecurityEssentials.Acceptance.Tests.Steps
{
    [Binding]
    public class UtilitySteps
    {
        [Given(@"I wait (.*) seconds")]
        [When(@"I wait (.*) seconds")]
        public void GivenIWaitSeconds(int waitInSeconds)
        {
            Thread.Sleep(waitInSeconds * 1000);
        }
    }
}