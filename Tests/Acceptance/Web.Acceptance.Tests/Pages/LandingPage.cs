using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using SecurityEssentials.Acceptance.Tests.Web.Menus;
using TechTalk.SpecFlow;

namespace SecurityEssentials.Acceptance.Tests.Web.Pages
{
	public class LandingPage : BasePage
	{

		private IWebElement LastAccountActivity
		{
			get { return this.GetVisibleWebElement(By.Id("LastAccountActivity")); }
		}

		public LandingPage(IWebDriver webDriver, Uri baseUri)
			: base(webDriver, baseUri, PageTitles.LANDING)
		{
			
		}

		public string GetLastAccountActivity()
		{
			return LastAccountActivity.Text;
		}

	}

}
