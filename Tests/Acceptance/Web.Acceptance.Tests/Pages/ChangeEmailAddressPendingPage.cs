using System;
using OpenQA.Selenium;
using SecurityEssentials.Acceptance.Tests.Web.Menus;

namespace SecurityEssentials.Acceptance.Tests.Web.Pages
{
	public class ChangeEmailAddressPendingPage : BasePage
	{
		public MenuBar MenuBar { get; private set; }

		public ChangeEmailAddressPendingPage(IWebDriver webDriver, Uri baseUri)
			: base(webDriver, baseUri, PageTitles.CHANGE_EMAIL_ADDRESS_PENDING)
		{
			MenuBar = new MenuBar(webDriver, baseUri);
		}

	}

}
