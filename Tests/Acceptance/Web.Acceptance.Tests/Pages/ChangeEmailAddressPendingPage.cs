using OpenQA.Selenium;
using System;
using SecurityEssentials.Acceptance.Tests.Menus;

namespace SecurityEssentials.Acceptance.Tests.Pages
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
