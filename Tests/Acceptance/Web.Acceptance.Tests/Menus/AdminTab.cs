using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using SecurityEssentials.Acceptance.Tests.Pages;

namespace SecurityEssentials.Acceptance.Tests.Menus
{
    public class AdminTab : BaseTab
    {
        public AdminTab(IWebDriver driver, Uri baseUri)
            : base(driver, baseUri, TabTitles.ADMIN, "admin", "admin")
        {
        }

        public ChangePasswordPage GotoChangePasswordPage()
        {
            ClickMenu();
            Click("changePassword");
            var changePasswordPage = new ChangePasswordPage(Driver, BaseUri);
            PageFactory.InitElements(Driver, changePasswordPage);
            return changePasswordPage;
        }

        public ChangeSecurityInformationPage GotoChangeSecurityInformationPage()
        {
            ClickMenu();
            Click("changeSecurityInformation");
            var changeSecurityInformationPage = new ChangeSecurityInformationPage(Driver, BaseUri);
            PageFactory.InitElements(Driver, changeSecurityInformationPage);
            return changeSecurityInformationPage;
        }

        public UserEditPage GotoManageAccountPage()
        {
            ClickMenu();
            Click("manageAccount");
            var page = new UserEditPage(Driver, BaseUri);
            PageFactory.InitElements(Driver, page);
            return page;
        }

        public AccountLogPage GotoChangeEmailAddressPage()
        {
            ClickMenu();
            Click("changeEmailAddress");
            var page = new AccountLogPage(Driver, BaseUri);
            PageFactory.InitElements(Driver, page);
            return page;
        }

        public AccountLogPage GotoAccountLogPage()
        {
            ClickMenu();
            Click("accountLog");
            var page = new AccountLogPage(Driver, BaseUri);
            PageFactory.InitElements(Driver, page);
            return page;
        }

        public UserIndexPage GotoManageUsersPage()
        {
            ClickMenu();
            Click("manageUsers");
            var page = new UserIndexPage(Driver, BaseUri);
            PageFactory.InitElements(Driver, page);
            return page;
        }
    }
}