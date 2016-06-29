using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using SecurityEssentials.Acceptance.Tests.Web.Menus;

namespace SecurityEssentials.Acceptance.Tests.Web.Pages
{
	public class RegisterPage : BasePage
	{
		public MenuBar MenuBar { get; private set; }

		private IWebElement Submit
		{
			get { return this.GetVisibleWebElement(By.Id("submit")); }
		}

		public RegisterPage(IWebDriver webDriver, Uri baseUri)
			: base(webDriver, baseUri, PageTitles.HOME)
		{
			MenuBar = new MenuBar(webDriver, baseUri);
		}

		public void ClickSubmit()
		{
			Submit.Click();
		}

	}

}
