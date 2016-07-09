using System;
using OpenQA.Selenium;
using SecurityEssentials.Acceptance.Tests.Web.Menus;

namespace SecurityEssentials.Acceptance.Tests.Web.Pages
{
	public class AccountLogPage : BasePage
	{
		public MenuBar MenuBar { get; private set; }

		private IWebElement MostRecentMessage
		{
			get { return this.GetVisibleWebElement(By.Id("mostRecentMessage")); }
		}
		

		public AccountLogPage(IWebDriver webDriver, Uri baseUri)
			: base(webDriver, baseUri, PageTitles.ACCOUNT_LOG)
		{
			MenuBar = new MenuBar(webDriver, baseUri);
		}

		public string GetMostRecentMessage()
		{
			return MostRecentMessage.Text;
		}

	}

}
