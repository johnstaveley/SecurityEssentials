using OpenQA.Selenium;
using SecurityEssentials.Acceptance.Tests.Web.Menus;
using System;
using SecurityEssentials.Acceptance.Tests.Menus;

namespace SecurityEssentials.Acceptance.Tests.Pages
{
	public class ChangeEmailAddressSuccessPage : BasePage
	{
		public MenuBar MenuBar { get; }

		public ChangeEmailAddressSuccessPage(IWebDriver webDriver, Uri baseUri)
			: base(webDriver, baseUri, PageTitles.CHANGE_EMAIL_ADDRESS_SUCCESS)
		{
			MenuBar = new MenuBar(webDriver, baseUri);
		}

	}

}
