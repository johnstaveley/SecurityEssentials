using OpenQA.Selenium;
using SecurityEssentials.Acceptance.Tests.Menus;
using System;

namespace SecurityEssentials.Acceptance.Tests.Pages
{
    public class RecoverPasswordSuccessPage : BasePage
	{
		public MenuBar MenuBar { get;  }

		public RecoverPasswordSuccessPage(IWebDriver webDriver, Uri baseUri)
			: base(webDriver, baseUri, PageTitles.RECOVER_PASSWORD_SUCCESS)
		{
			MenuBar = new MenuBar(webDriver, baseUri);
		}

	}

}
