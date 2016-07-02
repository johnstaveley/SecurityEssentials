using System;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using SecurityEssentials.Acceptance.Tests.Web.Menus;
using TechTalk.SpecFlow;

namespace SecurityEssentials.Acceptance.Tests.Web.Pages
{
	public class RecoverPasswordPage : BasePage
	{

		private IWebElement UserName
		{
			get { return this.GetVisibleWebElement(By.Id("UserName")); }
		}

		private IWebElement RecoverButton
		{
			get { return this.GetVisibleWebElement(By.Id("recoverButton")); }
		}

		public RecoverPasswordPage(IWebDriver webDriver, Uri baseUri)
			: base(webDriver, baseUri, PageTitles.RECOVER_PASSWORD)
		{
		}

                          
	}                     
                          
}
