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
				case "account log":
					var accountLogPage = new AccountLogPage(webDriver, uri);
					Assert.IsTrue(accountLogPage.IsCurrentPage);
					ScenarioContext.Current.Set(accountLogPage);
					break;
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
				case "manage users":
					var userIndexPage = new UserIndexPage(webDriver, uri);
					Assert.IsTrue(userIndexPage.IsCurrentPage);
					ScenarioContext.Current.Set(userIndexPage);
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
				case "user edit":
					var userEditPage = new UserEditPage(webDriver, uri);
					Assert.IsTrue(userEditPage.IsCurrentPage);
					ScenarioContext.Current.Set(userEditPage);
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

		[Given(@"I select Admin -> Manage Account from the menu")]
		public void GivenISelectAdmin_ManageAccountFromTheMenu()
		{
			var homePage = new HomePage(FeatureContext.Current.GetWebDriver(), FeatureContext.Current.GetBaseUri());

			homePage.MenuBar.AdminTab.GotoManageAccountPage();
		}

		[When(@"I select Admin -> Account Log from the menu")]
		public void GivenISelectAdmin_AccountLogFromTheMenu()
		{
			var homePage = new HomePage(FeatureContext.Current.GetWebDriver(), FeatureContext.Current.GetBaseUri());

			homePage.MenuBar.AdminTab.GotoAccountLogPage();
		}

		[Given(@"I select Admin -> Manage Users from the menu")]
		public void WhenISelectAdmin_ManageUsersFromTheMenu()
		{
			var homePage = new HomePage(FeatureContext.Current.GetWebDriver(), FeatureContext.Current.GetBaseUri());

			homePage.MenuBar.AdminTab.GotoManageUsersPage();
		}


	}
}
