using System;
using OpenQA.Selenium;
using SecurityEssentials.Acceptance.Tests.Web.Menus;

namespace SecurityEssentials.Acceptance.Tests.Web.Pages
{
	public class UserIndexPage : BasePage
	{
		public MenuBar MenuBar { get; private set; }

		private IWebElement EditUser(int id)
		{
			var elements = Driver.FindElements(By.XPath("//a[contains(text(),'Edit')]"));
			return elements[id];
		}

		public UserIndexPage(IWebDriver webDriver, Uri baseUri)
			: base(webDriver, baseUri, PageTitles.USERS_INDEX)
		{
			MenuBar = new MenuBar(webDriver, baseUri);
		}

		public void ClickEditUser(int userId)
		{
			EditUser(userId).Click();
		}
	}

}
