using OpenQA.Selenium;
using SecurityEssentials.Acceptance.Tests.Web.Menus;
using System;
using SecurityEssentials.Acceptance.Tests.Menus;

namespace SecurityEssentials.Acceptance.Tests.Pages
{
	public class ChangePasswordSuccessPage : BasePage
	{
		public MenuBar MenuBar { get; private set; }

		public ChangePasswordSuccessPage(IWebDriver webDriver, Uri baseUri)
			: base(webDriver, baseUri, PageTitles.CHANGE_PASSWORD_SUCCESS)
		{
			MenuBar = new MenuBar(webDriver, baseUri);
		}

	}

}
