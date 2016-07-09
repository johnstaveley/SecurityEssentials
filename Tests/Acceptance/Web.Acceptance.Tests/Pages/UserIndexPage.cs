using System;
using OpenQA.Selenium;
using SecurityEssentials.Acceptance.Tests.Web.Menus;

namespace SecurityEssentials.Acceptance.Tests.Web.Pages
{
	public class UserIndexPage : BasePage
	{
		public MenuBar MenuBar { get; private set; }

		public UserIndexPage(IWebDriver webDriver, Uri baseUri)
			: base(webDriver, baseUri, PageTitles.USERS_INDEX)
		{
			MenuBar = new MenuBar(webDriver, baseUri);
		}

	}

}
