using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using SecurityEssentials.Acceptance.Tests.Web.Menus;
using TechTalk.SpecFlow;

namespace SecurityEssentials.Acceptance.Tests.Web.Pages
{
	public class ChangeEmailAddressPage : BasePage
	{
		public MenuBar MenuBar { get; private set; }

		private IWebElement Password
		{
			get { return this.GetVisibleWebElement(By.Id("Password")); }
		}
		
		private IWebElement NewUserName
		{
			get { return this.GetVisibleWebElement(By.Id("NewUserName")); }
		}

		private IWebElement ChangeButton
		{
			get { return this.GetVisibleWebElement(By.Id("submit")); }
		}

		public ChangeEmailAddressPage(IWebDriver webDriver, Uri baseUri)
			: base(webDriver, baseUri, PageTitles.CHANGE_EMAIL_ADDRESS)
		{
			MenuBar = new MenuBar(webDriver, baseUri);
		}

		public void EnterDetails(Table table)
		{

			foreach (var row in table.Rows)
			{
				switch (row[0].ToLower())
				{
					case "newusername":
					case "newemailaddress":
						NewUserName.SendKeys(row[1]);
						break;
					case "password":
						Password.SendKeys(row[1]);
						break;
					default:
						throw new Exception(string.Format("Field {0} not defined", row[0]));
				}
			}
		}

		public void ClickSubmit()
		{
			ChangeButton.Click();
		}		

	}

}
