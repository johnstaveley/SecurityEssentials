using TechTalk.SpecFlow;
using SecurityEssentials.Acceptance.Tests.Web.Pages;
using SecurityEssentials.Acceptance.Tests.Web.Menus;
using SecurityEssentials.Acceptance.Tests.Web.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SecurityEssentials.Acceptance.Tests.Web.Steps
{

	[Binding]
	public class NavigationSteps
	{
		[Given(@"I navigate to the website")]
		[Given(@"I navigate to the Home page")]
		public void WhenINavigateToTheWebsite()
		{
			var webDriver = FeatureContext.Current.GetWebDriver();
			var uri = FeatureContext.Current.GetBaseUri();
			var homePage = HomePage.NavigateToPage(webDriver, uri);
			ScenarioContext.Current.Set(homePage);
		}

		[Given(@"I am taken to the login page")]
        public void ThenIAmTakenToTheLoginPage()
        {
			var loginPage = new LoginPage(FeatureContext.Current.GetWebDriver(), FeatureContext.Current.GetBaseUri());
			ScenarioContext.Current.Set<BasePage>(loginPage);
			Assert.IsTrue(loginPage.IsCurrentPage);
		}

		[Given(@"I navigate to the '(.*)' page")]
		public void GivenINavigateToThePage(string pageName)
		{

			var webDriver = FeatureContext.Current.GetWebDriver();
			var uri = FeatureContext.Current.GetBaseUri();
			BasePage page = null;
			switch (pageName)
			{
				case "Home":
					page = ScenarioContext.Current.GetPage<HomePage>();
					break;
				case "Register":
					page = ScenarioContext.Current.GetPage<RegisterPage>();
					break;
				default:
					throw new NotImplementedException();
			}
			ScenarioContext.Current.Set(page);
		}


	}
}
