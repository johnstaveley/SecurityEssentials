using NUnit.Framework;
using SecurityEssentials.Acceptance.Tests.Extensions;
using SecurityEssentials.Acceptance.Tests.Pages;
using System;
using TechTalk.SpecFlow;

namespace SecurityEssentials.Acceptance.Tests.Steps
{

    [Binding]
    public class NavigationSteps : TechTalk.SpecFlow.Steps
    {
        private readonly FeatureContext _featureContext;
        private readonly ScenarioContext _scenarioContext;

        public NavigationSteps(FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            _featureContext = featureContext;
            _scenarioContext = scenarioContext;
        }

        [Given(@"I navigate to the website")]
        [Given(@"I navigate to the Home page")]
        public void WhenINavigateToTheWebsite()
        {
            var webDriver = _featureContext.GetWebDriver();
            var uri = _featureContext.GetBaseUri();
            var homePage = HomePage.NavigateToPage(webDriver, uri);
            _scenarioContext.Set(homePage);
        }

        [Then(@"I am navigated to the '(.*)' page")]
        [Given(@"I am navigated to the '(.*)' page")]
        [When(@"I am navigated to the '(.*)' page")]
        public void GivenINavigateToThePage(string pageName)
        {

            var webDriver = _featureContext.GetWebDriver();
            var uri = _featureContext.GetBaseUri();
            switch (pageName.ToLower())
            {
                case "account log":
                    var accountLogPage = new AccountLogPage(webDriver, uri);
                    Assert.That(accountLogPage.IsCurrentPage, Is.True, $"Page {accountLogPage.GetType().Name} is not the current expected page");
                    _scenarioContext.Set(accountLogPage);
                    break;
                case "change email address":
                    var changeEmailAddressPage = new ChangeEmailAddressPage(webDriver, uri);
                    Assert.That(changeEmailAddressPage.IsCurrentPage, Is.True,
                        $"Page {changeEmailAddressPage.GetType().Name} is not the current expected page");
                    _scenarioContext.Set(changeEmailAddressPage);
                    break;
                case "change email address pending":
                    var changeEmailAddressPendingPage = new ChangeEmailAddressPendingPage(webDriver, uri);
                    Assert.That(changeEmailAddressPendingPage.IsCurrentPage, Is.True,
                        $"Page {changeEmailAddressPendingPage.GetType().Name} is not the current expected page");
                    _scenarioContext.Set(changeEmailAddressPendingPage);
                    break;
                case "change email address success":
                    var changeEmailAddressSuccessPage = new ChangeEmailAddressSuccessPage(webDriver, uri);
                    Assert.That(changeEmailAddressSuccessPage.IsCurrentPage, Is.True,
                        $"Page {changeEmailAddressSuccessPage.GetType().Name} is not the current expected page");
                    _scenarioContext.Set(changeEmailAddressSuccessPage);
                    break;
                case "change password":
                    var changePasswordPage = new ChangePasswordPage(webDriver, uri);
                    Assert.That(changePasswordPage.IsCurrentPage, Is.True,
                        $"Page {changePasswordPage.GetType().Name} is not the current expected page");
                    _scenarioContext.Set(changePasswordPage);
                    break;
                case "change password success":
                    var changePasswordSuccessPage = new ChangePasswordSuccessPage(webDriver, uri);
                    Assert.That(changePasswordSuccessPage.IsCurrentPage, Is.True,
                        $"Page {changePasswordSuccessPage.GetType().Name} is not the current expected page");
                    _scenarioContext.Set(changePasswordSuccessPage);
                    break;
                case "change security information":
                    var changeSecurityInformationPage = new ChangeSecurityInformationPage(webDriver, uri);
                    Assert.That(changeSecurityInformationPage.IsCurrentPage, Is.True,
                        $"Page {changeSecurityInformationPage.GetType().Name} is not the current expected page");
                    _scenarioContext.Set(changeSecurityInformationPage);
                    break;
                case "change security information success":
                    var changeSecurityInformationSuccessPage = new ChangeSecurityInformationSuccessPage(webDriver, uri);
                    Assert.That(changeSecurityInformationSuccessPage.IsCurrentPage, Is.True,
                        $"Page {changeSecurityInformationSuccessPage.GetType().Name} is not the current expected page");
                    _scenarioContext.Set(changeSecurityInformationSuccessPage);
                    break;
                case "delete user":
                    var deleteUserPage = new DeleteUserPage(webDriver, uri);
                    Assert.That(deleteUserPage.IsCurrentPage, Is.True,
                        $"Page {deleteUserPage.GetType().Name} is not the current expected page");
                    _scenarioContext.Set(deleteUserPage);
                    break;
                case "home":
                    var homePage = new HomePage(webDriver, uri);
                    Assert.That(homePage.IsCurrentPage, Is.True, $"Page {homePage.GetType().Name} is not the current expected page");
                    _scenarioContext.Set(homePage);
                    break;
                case "landing":
                    var landingPage = new LandingPage(webDriver, uri);
                    Assert.That(landingPage.IsCurrentPage, Is.True, $"Page {landingPage.GetType().Name} is not the current expected page");
                    _scenarioContext.Set(landingPage);
                    break;
                case "log":
                    var logPage = new LogPage(webDriver, uri);
                    Assert.That(logPage.IsCurrentPage, Is.True, $"Page {logPage.GetType().Name} is not the current expected page");
                    _scenarioContext.Set(logPage);
                    break;
                case "login":
                    var loginPage = new LoginPage(webDriver, uri);
                    Assert.That(loginPage.IsCurrentPage, Is.True, $"Page {loginPage.GetType().Name} is not the current expected page");
                    _scenarioContext.Set(loginPage);
                    break;
                case "make admin":
                    var makeAdminPage = new MakeAdminPage(webDriver, uri);
                    Assert.That(makeAdminPage.IsCurrentPage, Is.True, $"Page {makeAdminPage.GetType().Name} is not the current expected page");
                    _scenarioContext.Set(makeAdminPage);
                    break;
                case "manage users":
                    var userIndexPage = new UserIndexPage(webDriver, uri);
                    Assert.That(userIndexPage.IsCurrentPage, Is.True, $"Page {userIndexPage.GetType().Name} is not the current expected page");
                    _scenarioContext.Set(userIndexPage);
                    break;
                case "recover":
                    var recoverPage = new RecoverPage(webDriver, uri);
                    Assert.That(recoverPage.IsCurrentPage, Is.True, $"Page {recoverPage.GetType().Name} is not the current expected page");
                    _scenarioContext.Set(recoverPage);
                    break;
                case "recover password":
                    var recoverPasswordPage = new RecoverPasswordPage(webDriver, uri);
                    Assert.That(recoverPasswordPage.IsCurrentPage, Is.True,
                        $"Page {recoverPasswordPage.GetType().Name} is not the current expected page");
                    _scenarioContext.Set(recoverPasswordPage);
                    break;
                case "recover password success":
                    var recoverPasswordSuccessPage = new RecoverPasswordSuccessPage(webDriver, uri);
                    Assert.That(recoverPasswordSuccessPage.IsCurrentPage, Is.True,
                        $"Page {recoverPasswordSuccessPage.GetType().Name} is not the current expected page");
                    _scenarioContext.Set(recoverPasswordSuccessPage);
                    break;
                case "recover success":
                    var recoverSuccessPage = new RecoverSuccessPage(webDriver, uri);
                    Assert.That(recoverSuccessPage.IsCurrentPage, Is.True,
                        $"Page {recoverSuccessPage.GetType().Name} is not the current expected page");
                    _scenarioContext.Set(recoverSuccessPage);
                    break;
                case "register":
                    var registerPage = new RegisterPage(webDriver, uri);
                    Assert.That(registerPage.IsCurrentPage, Is.True, $"Page {registerPage.GetType().Name} is not the current expected page");
                    _scenarioContext.Set(registerPage);
                    break;
                case "register success":
                    var registerSuccessPage = new RegisterSuccessPage(webDriver, uri);
                    Assert.That(registerSuccessPage.IsCurrentPage, Is.True, $"Page {registerSuccessPage.GetType().Name} is not the current expected page");
                    _scenarioContext.Set(registerSuccessPage);
                    break;
                case "remove admin":
                    var removeAdminPage = new RemoveAdminPage(webDriver, uri);
                    Assert.That(removeAdminPage.IsCurrentPage, Is.True, $"Page {removeAdminPage.GetType().Name} is not the current expected page");
                    _scenarioContext.Set(removeAdminPage);
                    break;
                case "reset password":
                    var resetPasswordPage = new ResetPasswordPage(webDriver, uri);
                    Assert.That(resetPasswordPage.IsCurrentPage, Is.True, $"Page {resetPasswordPage.GetType().Name} is not the current expected page");
                    _scenarioContext.Set(resetPasswordPage);
                    break;
                case "user edit":
                    var userEditPage = new UserEditPage(webDriver, uri);
                    Assert.That(userEditPage.IsCurrentPage, Is.True, $"Page {userEditPage.GetType().Name} is not the current expected page");
                    _scenarioContext.Set(userEditPage);
                    break;
                default:
                    throw new NotImplementedException(pageName);
            }
        }

        [Then(@"I am taken to the registration success page")]
        public void ThenIAmTakenToTheRegistrationSuccessPage()
        {
            BasePage page = _scenarioContext.GetPage<RegisterSuccessPage>();
            Assert.That(page.IsCurrentPage, Is.True);
        }

        [Given(@"I select Admin -> Change Password from the menu")]
        public void GivenISelectAdmin_ChangePasswordFromTheMenu()
        {
            var homePage = new HomePage(_featureContext.GetWebDriver(), _featureContext.GetBaseUri());
            homePage.MenuBar.AdminTab.GotoChangePasswordPage();
        }

        [Then(@"I select Admin -> Change Security Information from the menu")]
        [Given(@"I select Admin -> Change Security Information from the menu")]
        public void GivenISelectAdmin_ChangeSecurityInformationFromTheMenu()
        {
            var homePage = new HomePage(_featureContext.GetWebDriver(), _featureContext.GetBaseUri());

            homePage.MenuBar.AdminTab.GotoChangeSecurityInformationPage();
        }

        [When(@"I select Admin -> Manage Account from the menu")]
        [Given(@"I select Admin -> Manage Account from the menu")]
        public void GivenISelectAdmin_ManageAccountFromTheMenu()
        {
            var homePage = new HomePage(_featureContext.GetWebDriver(), _featureContext.GetBaseUri());

            homePage.MenuBar.AdminTab.GotoManageAccountPage();
        }

        [Given(@"I select Admin -> Change Email Address from the menu")]
        public void GivenISelectAdmin_ChangeEmailAddressFromTheMenu()
        {
            var homePage = new HomePage(_featureContext.GetWebDriver(), _featureContext.GetBaseUri());

            homePage.MenuBar.AdminTab.GotoChangeEmailAddressPage();
        }

        [When(@"I select Admin -> Account Log from the menu")]
        public void GivenISelectAdmin_AccountLogFromTheMenu()
        {
            var homePage = new HomePage(_featureContext.GetWebDriver(), _featureContext.GetBaseUri());
            homePage.MenuBar.AdminTab.GotoAccountLogPage();
        }
        [When(@"I select Admin -> Logs from the menu")]
        public void WhenISelectAdmin_LogsFromTheMenu()
        {
            var homePage = new HomePage(_featureContext.GetWebDriver(), _featureContext.GetBaseUri());
            homePage.MenuBar.AdminTab.GotoLogPage();
        }
        [Given(@"I select Admin -> Manage Users from the menu")]
        public void WhenISelectAdmin_ManageUsersFromTheMenu()
        {
            var homePage = new HomePage(_featureContext.GetWebDriver(), _featureContext.GetBaseUri());
            homePage.MenuBar.AdminTab.GotoManageUsersPage();
        }

    }
}
