using SecurityEssentials.Acceptance.Tests.Extensions;
using System.Configuration;
using System.Drawing;
using System.Threading;
using TechTalk.SpecFlow;

namespace SecurityEssentials.Acceptance.Tests.Steps
{
    [Binding]
    public class UtilitySteps : TechTalk.SpecFlow.Steps
    {
        private readonly FeatureContext _featureContext;
        private readonly ScenarioContext _scenarioContext;

        public UtilitySteps(FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            _featureContext = featureContext;
            _scenarioContext = scenarioContext;
        }
        [Given(@"I wait (.*) seconds")]
        [When(@"I wait (.*) seconds")]
        [Then(@"I wait (.*) seconds")]
        public void GivenIWaitSeconds(int waitInSeconds)
        {
            Thread.Sleep(waitInSeconds * 1000);
        }
        [Given(@"I maximise the browser window")]
        [When(@"I maximise the browser window")]
        public void GivenIMaximiseTheBrowserWindow()
        {
            var driver = _featureContext.GetWebDriver();
            var webBrowserType = ConfigurationManager.AppSettings["WebBrowserType"];
            if (webBrowserType == "Headless Chrome")
            {
                driver.Manage().Window.Size = new Size(1920, 1080);
            }
            else
            {
                driver.Manage().Window.Maximize();
            }
        }

    }
}
