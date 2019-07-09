using OpenQA.Selenium;
using SecurityEssentials.Acceptance.Tests.Menus;
using System;

namespace SecurityEssentials.Acceptance.Tests.Pages
{
    public class LandingPage : BasePage
	{
		public MenuBar MenuBar { get;  }

		private IWebElement LastAccountActivity => GetVisibleWebElement(By.Id("LastAccountActivity"));
		public LandingPage(IWebDriver webDriver, Uri baseUri)
			: base(webDriver, baseUri, PageTitles.LANDING)
		{
			MenuBar = new MenuBar(webDriver, baseUri);
		}

		public string GetLastAccountActivity()
		{
			return LastAccountActivity.Text;
		}
		
	}

}
