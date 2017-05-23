using NUnit.Framework;
using SecurityEssentials.Acceptance.Tests.Extensions;
using SecurityEssentials.Acceptance.Tests.Pages;
using TechTalk.SpecFlow;

namespace SecurityEssentials.Acceptance.Tests.Steps
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

		[When(@"I click the login link in the navigation bar")]
		[Given(@"I click the login link in the navigation bar")]
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
