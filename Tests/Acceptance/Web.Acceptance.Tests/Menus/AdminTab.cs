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

	}
}
