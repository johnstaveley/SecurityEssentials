using NUnit.Framework;
using SecurityEssentials.Acceptance.Tests.Extensions;
using SecurityEssentials.Acceptance.Tests.Pages;
using TechTalk.SpecFlow;

namespace SecurityEssentials.Acceptance.Tests.Steps
{

    [Binding]
    public class HomePageSteps : TechTalk.SpecFlow.Steps
    {
        private readonly FeatureContext _featureContext;
        private readonly ScenarioContext _scenarioContext;

        public HomePageSteps(FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            _featureContext = featureContext;
            _scenarioContext = scenarioContext;
        }
        [Given(@"I am taken to the homepage")]
        [When(@"I am taken to the homepage")]
        public void ThenIAmTakenToTheHomepage()
        {
            var homePage = _scenarioContext.GetPage<HomePage>();
            Assert.IsTrue(homePage.IsCurrentPage);
        }

        [Given(@"I click register in the title bar")]
        public void GivenIClickRegisterInTheTitleBar()
        {
            var homePage = _scenarioContext.GetPage<HomePage>();
            homePage.ClickRegister();
        }

        [When(@"I click the login link in the navigation bar")]
        [Given(@"I click the login link in the navigation bar")]
        public void GivenIClickLogin()
        {
            var homePage = _scenarioContext.GetPage<HomePage>();
            homePage.ClickLogin();
        }

        [Given(@"I delete all cookies from the cache")]
        public void GivenIDeleteAllCookiesFromTheCache()
        {
            var driver = _featureContext.GetWebDriver();
            driver.Manage().Cookies.DeleteAllCookies();
        }
    }
}
