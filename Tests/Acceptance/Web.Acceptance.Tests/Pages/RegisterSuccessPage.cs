using System;
using OpenQA.Selenium;
using SecurityEssentials.Acceptance.Tests.Web.Menus;

namespace SecurityEssentials.Acceptance.Tests.Web.Pages
{
	public class RegisterSuccessPage : BasePage
	{
		public MenuBar MenuBar { get; private set; }

		public RegisterSuccessPage(IWebDriver webDriver, Uri baseUri)
			: base(webDriver, baseUri, PageTitles.REGISTER_SUCCESS)
		{
			MenuBar = new MenuBar(webDriver, baseUri);
		}

                          
	}                     
                          
}
