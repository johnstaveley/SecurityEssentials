using OpenQA.Selenium;
using SecurityEssentials.Acceptance.Tests.Menus;
using System;

namespace SecurityEssentials.Acceptance.Tests.Pages
{
    public class ResetPasswordPage : BasePage
	{
		public MenuBar MenuBar { get; }

		private IWebElement UserName => GetVisibleWebElement(By.Id("UserName"));

		private IWebElement ConfirmButton => GetVisibleWebElement(By.Id("submit"));
		public ResetPasswordPage(IWebDriver webDriver, Uri baseUri)
			: base(webDriver, baseUri, PageTitles.RESET_PASSWORD)
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
