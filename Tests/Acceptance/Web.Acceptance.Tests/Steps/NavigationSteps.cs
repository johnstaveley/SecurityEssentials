using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecurityEssentials.Acceptance.Tests.Web.Extensions;
using SecurityEssentials.Acceptance.Tests.Web.Pages;
using TechTalk.SpecFlow;

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
                    Assert.IsTrue(accountLogPage.IsCurrentPage,
                        $"Page {accountLogPage.GetType().Name} is not the current expected page");
                    ScenarioContext.Current.Set(accountLogPage);
                    break;
                case "change email address":
                    var changeEmailAddressPage = new ChangeEmailAddressPage(webDriver, uri);
                    Assert.IsTrue(changeEmailAddressPage.IsCurrentPage,
                        $"Page {changeEmailAddressPage.GetType().Name} is not the current expected page");
                    ScenarioContext.Current.Set(changeEmailAddressPage);
                    break;
                case "change email address pending":
                    var changeEmailAddressPendingPage = new ChangeEmailAddressPendingPage(webDriver, uri);
                    Assert.IsTrue(changeEmailAddressPendingPage.IsCurrentPage,
                        $"Page {changeEmailAddressPendingPage.GetType().Name} is not the current expected page");
                    ScenarioContext.Current.Set(changeEmailAddressPendingPage);
                    break;
                case "change email address success":
                    var changeEmailAddressSuccessPage = new ChangeEmailAddressSuccessPage(webDriver, uri);
                    Assert.IsTrue(changeEmailAddressSuccessPage.IsCurrentPage,
                        $"Page {changeEmailAddressSuccessPage.GetType().Name} is not the current expected page");
                    ScenarioContext.Current.Set(changeEmailAddressSuccessPage);
                    break;
                case "change password":
                    var changePasswordPage = new ChangePasswordPage(webDriver, uri);
                    Assert.IsTrue(changePasswordPage.IsCurrentPage,
                        $"Page {changePasswordPage.GetType().Name} is not the current expected page");
                    ScenarioContext.Current.Set(changePasswordPage);
                    break;
                case "change password success":
                    var changePasswordSuccessPage = new ChangePasswordSuccessPage(webDriver, uri);
                    Assert.IsTrue(changePasswordSuccessPage.IsCurrentPage,
                        $"Page {changePasswordSuccessPage.GetType().Name} is not the current expected page");
                    ScenarioContext.Current.Set(changePasswordSuccessPage);
                    break;
                case "change security information":
                    var changeSecurityInformationPage = new ChangeSecurityInformationPage(webDriver, uri);
                    Assert.IsTrue(changeSecurityInformationPage.IsCurrentPage,
                        $"Page {changeSecurityInformationPage.GetType().Name} is not the current expected page");
                    ScenarioContext.Current.Set(changeSecurityInformationPage);
                    break;
                case "change security information success":
                    var changeSecurityInformationSuccessPage = new ChangeSecurityInformationSuccessPage(webDriver, uri);
                    Assert.IsTrue(changeSecurityInformationSuccessPage.IsCurrentPage,
                        $"Page {changeSecurityInformationSuccessPage.GetType().Name} is not the current expected page");
                    ScenarioContext.Current.Set(changeSecurityInformationSuccessPage);
                    break;
                case "home":
                    var homePage = new HomePage(webDriver, uri);
                    Assert.IsTrue(homePage.IsCurrentPage,
                        $"Page {homePage.GetType().Name} is not the current expected page");
                    ScenarioContext.Current.Set(homePage);
                    break;
                case "landing":
                    var landingPage = new LandingPage(webDriver, uri);
                    Assert.IsTrue(landingPage.IsCurrentPage,
                        $"Page {landingPage.GetType().Name} is not the current expected page");
                    ScenarioContext.Current.Set(landingPage);
                    break;
                case "login":
                    var loginPage = new LoginPage(webDriver, uri);
                    Assert.IsTrue(loginPage.IsCurrentPage,
                        $"Page {loginPage.GetType().Name} is not the current expected page");
                    ScenarioContext.Current.Set(loginPage);
                    break;
                case "manage users":
                    var userIndexPage = new UserIndexPage(webDriver, uri);
                    Assert.IsTrue(userIndexPage.IsCurrentPage,
                        $"Page {userIndexPage.GetType().Name} is not the current expected page");
                    ScenarioContext.Current.Set(userIndexPage);
                    break;
                case "recover":
                    var recoverPage = new RecoverPage(webDriver, uri);
                    Assert.IsTrue(recoverPage.IsCurrentPage,
                        $"Page {recoverPage.GetType().Name} is not the current expected page");
                    ScenarioContext.Current.Set(recoverPage);
                    break;
                case "recover password":
                    var recoverPasswordPage = new RecoverPasswordPage(webDriver, uri);
                    Assert.IsTrue(recoverPasswordPage.IsCurrentPage,
                        $"Page {recoverPasswordPage.GetType().Name} is not the current expected page");
                    ScenarioContext.Current.Set(recoverPasswordPage);
                    break;
                case "recover password success":
                    var recoverPasswordSuccessPage = new RecoverPasswordSuccessPage(webDriver, uri);
                    Assert.IsTrue(recoverPasswordSuccessPage.IsCurrentPage,
                        $"Page {recoverPasswordSuccessPage.GetType().Name} is not the current expected page");
                    ScenarioContext.Current.Set(recoverPasswordSuccessPage);
                    break;
                case "recover success":
                    var recoverSuccessPage = new RecoverSuccessPage(webDriver, uri);
                    Assert.IsTrue(recoverSuccessPage.IsCurrentPage,
                        $"Page {recoverSuccessPage.GetType().Name} is not the current expected page");
                    ScenarioContext.Current.Set(recoverSuccessPage);
                    break;
                case "register":
                    var registerPage = new RegisterPage(webDriver, uri);
                    Assert.IsTrue(registerPage.IsCurrentPage,
                        $"Page {registerPage.GetType().Name} is not the current expected page");
                    ScenarioContext.Current.Set(registerPage);
                    break;
                case "register success":
                    var registerSuccessPage = new RegisterSuccessPage(webDriver, uri);
                    Assert.IsTrue(registerSuccessPage.IsCurrentPage,
                        $"Page {registerSuccessPage.GetType().Name} is not the current expected page");
                    ScenarioContext.Current.Set(registerSuccessPage);
                    break;
                case "user edit":
                    var userEditPage = new UserEditPage(webDriver, uri);
                    Assert.IsTrue(userEditPage.IsCurrentPage,
                        $"Page {userEditPage.GetType().Name} is not the current expected page");
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

        [Given(@"I select Admin -> Change Email Address from the menu")]
        public void GivenISelectAdmin_ChangeEmailAddressFromTheMenu()
        {
            var homePage = new HomePage(FeatureContext.Current.GetWebDriver(), FeatureContext.Current.GetBaseUri());

            homePage.MenuBar.AdminTab.GotoChangeEmailAddressPage();
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