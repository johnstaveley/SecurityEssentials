using System;
using OpenQA.Selenium;
using SecurityEssentials.Acceptance.Tests.Web.Menus;

namespace SecurityEssentials.Acceptance.Tests.Web.Pages
{
	public class LandingPage : BasePage
	{
		public MenuBar MenuBar { get; private set; }

		private IWebElement LastAccountActivity
		{
			get { return this.GetVisibleWebElement(By.Id("LastAccountActivity")); }
		}

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
