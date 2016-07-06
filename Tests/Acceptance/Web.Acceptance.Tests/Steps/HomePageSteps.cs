using SecurityEssentials.Acceptance.Tests.Web.Pages;
using TechTalk.SpecFlow;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SecurityEssentials.Acceptance.Tests.Web.Extensions
{
	[Binding]
	public class HomePageSteps
	{
		[Given(@"I am taken to the homepage")]
		[When(@"I am taken to the homepage")]
		public void ThenIAmTakenToTheHomepage()
		{			
            var homePage = ScenarioContext.Current.GetPage<HomePage>();
			Assert.IsTrue(homePage.IsCurrentPage);
		}

        [Given(@"I click register in the title bar")]
		public void GivenIClickRegisterInTheTitleBar()
		{
			var homePage = ScenarioContext.Current.GetPage<HomePage>();
			homePage.ClickRegister();
		}

		[When(@"I click login")]
        [Given(@"I click login")]
		public void GivenIClickLogin()
		{
			var homePage = ScenarioContext.Current.GetPage<HomePage>();
			homePage.ClickLogin();

		}

		[Given(@"I delete all cookies from the cache")]
		public void GivenIDeleteAllCookiesFromTheCache()
		{
			var driver = FeatureContext.Current.GetWebDriver();
			driver.Manage().Cookies.DeleteAllCookies();
		}


	}
}
