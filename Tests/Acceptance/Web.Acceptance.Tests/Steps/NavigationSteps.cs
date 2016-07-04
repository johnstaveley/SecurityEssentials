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

		[Then(@"I navigate to the '(.*)' page")]
		[Given(@"I navigate to the '(.*)' page")]
		public void GivenINavigateToThePage(string pageName)
		{

			var webDriver = FeatureContext.Current.GetWebDriver();
			var uri = FeatureContext.Current.GetBaseUri();
			switch (pageName.ToLower())
			{
				case "home":
					var homePage = new HomePage(webDriver, uri);
					Assert.IsTrue(homePage.IsCurrentPage);
					ScenarioContext.Current.Set(homePage);
					break;
				case "landing":
					var landingPage = new LandingPage(webDriver, uri);
					Assert.IsTrue(landingPage.IsCurrentPage);
					ScenarioContext.Current.Set(landingPage);
					break;
				case "login":
					var loginPage = new LoginPage(webDriver, uri);
					Assert.IsTrue(loginPage.IsCurrentPage);
					ScenarioContext.Current.Set(loginPage);
					break;
				case "register":
					var registerPage = new RegisterPage(webDriver, uri);
					Assert.IsTrue(registerPage.IsCurrentPage);
					ScenarioContext.Current.Set(registerPage);
break;
				default:
					throw new NotImplementedException();
			}
		}

		[Then(@"I am taken to the registration success page")]
		public void ThenIAmTakenToTheRegistrationSuccessPage()
		{
			BasePage page = ScenarioContext.Current.GetPage<RegisterSuccessPage>();
			Assert.IsTrue(page.IsCurrentPage);
		}


	}
}
