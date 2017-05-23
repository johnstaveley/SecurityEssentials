using System;
using OpenQA.Selenium;
using SecurityEssentials.Acceptance.Tests.Menus;
using SecurityEssentials.Acceptance.Tests.Web.Menus;

namespace SecurityEssentials.Acceptance.Tests.Pages
{
	public class RecoverSuccessPage : BasePage
	{
		public MenuBar MenuBar { get; }

		public RecoverSuccessPage(IWebDriver webDriver, Uri baseUri)
			: base(webDriver, baseUri, PageTitles.RECOVER_SUCCESS)
		{
			MenuBar = new MenuBar(webDriver, baseUri);
		}

	}

}
