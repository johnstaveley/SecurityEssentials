using OpenQA.Selenium;
using SecurityEssentials.Acceptance.Tests.Web.Menus;
using System;
using SecurityEssentials.Acceptance.Tests.Menus;

namespace SecurityEssentials.Acceptance.Tests.Pages
{
	public class MakeAdminPage : BasePage
	{
		public MenuBar MenuBar { get; }

		private IWebElement UserName => GetVisibleWebElement(By.Id("UserName"));

		private IWebElement ConfirmButton => GetVisibleWebElement(By.Id("submit"));
		public MakeAdminPage(IWebDriver webDriver, Uri baseUri)
			: base(webDriver, baseUri, PageTitles.MAKE_ADMIN)
		{
			MenuBar = new MenuBar(webDriver, baseUri);
		}

		public void ClickSubmit()
		{
			ConfirmButton.Click();
		}

		public string GetUserName()
		{
			return UserName.Text;
		}
	}
}
