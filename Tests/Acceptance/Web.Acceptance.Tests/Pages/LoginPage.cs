using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using SecurityEssentials.Acceptance.Tests.Web.Menus;

namespace SecurityEssentials.Acceptance.Tests.Web.Pages
{
	public class LoginPage : BasePage
	{

		public LoginPage(IWebDriver webDriver, Uri baseUri)
			: base(webDriver, baseUri, PageTitles.HOME)
		{
			
		}

	}

}
