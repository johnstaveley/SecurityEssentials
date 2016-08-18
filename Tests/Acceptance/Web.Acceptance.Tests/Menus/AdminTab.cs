using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using SecurityEssentials.Acceptance.Tests.Web.Pages;

namespace SecurityEssentials.Acceptance.Tests.Web.Menus
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
			var changePasswordPage = new ChangePasswordPage(_driver, _baseUri);
			PageFactory.InitElements(_driver, changePasswordPage);
			return changePasswordPage;
		}

		public ChangeSecurityInformationPage GotoChangeSecurityInformationPage()
		{
			ClickMenu();
			Click("changeSecurityInformation");
			var changeSecurityInformationPage = new ChangeSecurityInformationPage(_driver, _baseUri);
			PageFactory.InitElements(_driver, changeSecurityInformationPage);
			return changeSecurityInformationPage;
		}

		public UserEditPage GotoManageAccountPage()
		{
			ClickMenu();
			Click("manageAccount");
			var page = new UserEditPage(_driver, _baseUri);
			PageFactory.InitElements(_driver, page);
			return page;
		}

		public AccountLogPage GotoChangeEmailAddressPage()
		{
			ClickMenu();
			Click("changeEmailAddress");
			var page = new AccountLogPage(_driver, _baseUri);
			PageFactory.InitElements(_driver, page);
			return page;
		}

		public AccountLogPage GotoAccountLogPage()
		{
			ClickMenu();
			Click("accountLog");
			var page = new AccountLogPage(_driver, _baseUri);
			PageFactory.InitElements(_driver, page);
			return page;
		}

		public UserIndexPage GotoManageUsersPage()
		{
			ClickMenu();
			Click("manageUsers");
			var page = new UserIndexPage(_driver, _baseUri);
			PageFactory.InitElements(_driver, page);
			return page;

		}

	}
}
