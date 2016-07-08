using TechTalk.SpecFlow;
using SecurityEssentials.Acceptance.Tests.Web.Pages;
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

		[Then(@"I am navigated to the '(.*)' page")]
		[Given(@"I am navigated to the '(.*)' page")]
		public void GivenINavigateToThePage(string pageName)
		{

			var webDriver = FeatureContext.Current.GetWebDriver();
			var uri = FeatureContext.Current.GetBaseUri();
			switch (pageName.ToLower())
			{
				case "change password":
					var changePasswordPage = new ChangePasswordPage(webDriver, uri);
					Assert.IsTrue(changePasswordPage.IsCurrentPage);
					ScenarioContext.Current.Set(changePasswordPage);
					break;
				case "change security information":
					var changeSecurityInformationPage = new ChangeSecurityInformationPage(webDriver, uri);
					Assert.IsTrue(changeSecurityInformationPage.IsCurrentPage);
					ScenarioContext.Current.Set(changeSecurityInformationPage);
					break;
				case "change security information success":
					var changeSecurityInformationSuccessPage = new ChangeSecurityInformationSuccessPage(webDriver, uri);
					Assert.IsTrue(changeSecurityInformationSuccessPage.IsCurrentPage);
					ScenarioContext.Current.Set(changeSecurityInformationSuccessPage);
					break;
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
				case "recover":
					var recoverPage = new RecoverPage(webDriver, uri);
					Assert.IsTrue(recoverPage.IsCurrentPage);
					ScenarioContext.Current.Set(recoverPage);
					break;
				case "recover password":
					var recoverPasswordPage = new RecoverPasswordPage(webDriver, uri);
					Assert.IsTrue(recoverPasswordPage.IsCurrentPage);
					ScenarioContext.Current.Set(recoverPasswordPage);
					break;
				case "recover password success":
					var recoverPasswordSuccessPage = new RecoverPasswordSuccessPage(webDriver, uri);
					Assert.IsTrue(recoverPasswordSuccessPage.IsCurrentPage);
					ScenarioContext.Current.Set(recoverPasswordSuccessPage);
					break;
				case "recover success":
					var recoverSuccessPage = new RecoverSuccessPage(webDriver, uri);
					Assert.IsTrue(recoverSuccessPage.IsCurrentPage);
					ScenarioContext.Current.Set(recoverSuccessPage);
					break;
				case "register":
					var registerPage = new RegisterPage(webDriver, uri);
					Assert.IsTrue(registerPage.IsCurrentPage);
					ScenarioContext.Current.Set(registerPage);
break;
				case "register success":
					var registerSuccessPage = new RegisterSuccessPage(webDriver, uri);
					Assert.IsTrue(registerSuccessPage.IsCurrentPage);
					ScenarioContext.Current.Set(registerSuccessPage);
break;
				default:
					throw new NotImplementedException(pageName);
			}
		}

		[Then(@"I am taken to the registration success page")]
		public void ThenIAmTakenToTheRegistrationSuccessPage()
		{
			BasePage page = ScenarioContext.Current.GetPage<RegisterSuccessPage>();
			Assert.IsTrue(page.IsCurrentPage);
		}

		[Given(@"I select Admin -> Change Password from the menu")]
		public void GivenISelectAdmin_ChangePasswordFromTheMenu()
		{
			var homePage = new HomePage(FeatureContext.Current.GetWebDriver(), FeatureContext.Current.GetBaseUri());
			homePage.MenuBar.AdminTab.GotoChangePasswordPage();
		}

		[Given(@"I select Admin -> Change Security Information from the menu")]
		public void GivenISelectAdmin_ChangeSecurityInformationFromTheMenu()
		{
			var homePage = new HomePage(FeatureContext.Current.GetWebDriver(), FeatureContext.Current.GetBaseUri());
	
				homePage.MenuBar.AdminTab.GotoChangeSecurityInformationPage();
		}

	}
}
