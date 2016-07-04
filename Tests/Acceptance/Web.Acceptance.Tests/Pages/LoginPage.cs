using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using SecurityEssentials.Acceptance.Tests.Web.Menus;
using TechTalk.SpecFlow;

namespace SecurityEssentials.Acceptance.Tests.Web.Pages
{
	public class LoginPage : BasePage
	{

		private IWebElement UserName
		{
			get { return this.GetVisibleWebElement(By.Id("UserName")); }
		}

		private IWebElement Password
		{
			get { return this.GetVisibleWebElement(By.Id("Password")); }
		}

		private IWebElement RecoverLink
		{
			get { return this.GetVisibleWebElement(By.Id("Recover")); }
		}

		private IWebElement LoginButton
		{
			get { return this.GetVisibleWebElement(By.Id("Login")); }
		}				

		public LoginPage(IWebDriver webDriver, Uri baseUri)
			: base(webDriver, baseUri, PageTitles.LOGIN)
		{
			
		}

		public void ClickSubmit()
		{
			LoginButton.Click();
		}

		public void ClickRecoverPassword()
		{
			RecoverLink.Click();
		}

		public void EnterDetails(Table table)
		{

			foreach (var row in table.Rows)
			{
				switch (row[0].ToLower())
				{
					case "username":
						UserName.SendKeys(row[1]);
						break;
					case "password":
						Password.SendKeys(row[1]);
						break;
					default:
						throw new Exception(string.Format("Field {0} not defined", row[0]));
				}
			}

		}                 

	}

}
