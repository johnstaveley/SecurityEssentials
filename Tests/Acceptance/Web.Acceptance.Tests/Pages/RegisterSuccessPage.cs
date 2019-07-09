using OpenQA.Selenium;
using SecurityEssentials.Acceptance.Tests.Menus;
using System;

namespace SecurityEssentials.Acceptance.Tests.Pages
{
    public class RegisterSuccessPage : BasePage
	{
		public MenuBar MenuBar { get; }

		public RegisterSuccessPage(IWebDriver webDriver, Uri baseUri)
			: base(webDriver, baseUri, PageTitles.REGISTER_SUCCESS)
		{
			MenuBar = new MenuBar(webDriver, baseUri);
		}
	}

}
