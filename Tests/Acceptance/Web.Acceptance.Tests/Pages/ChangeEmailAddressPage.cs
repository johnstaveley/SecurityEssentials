using System;
using OpenQA.Selenium;
using SecurityEssentials.Acceptance.Tests.Menus;
using TechTalk.SpecFlow;

namespace SecurityEssentials.Acceptance.Tests.Pages
{
	public class ChangeEmailAddressPage : BasePage
	{
		public MenuBar MenuBar { get; }

		private IWebElement Password => GetVisibleWebElement(By.Id("Password"));

		private IWebElement NewUserName => GetVisibleWebElement(By.Id("NewEmailAddress"));

		private IWebElement ChangeButton => GetVisibleWebElement(By.Id("submit"));

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
						throw new Exception($"Field {row[0]} not defined");
				}
			}
		}

		public void ClickSubmit()
		{
			ChangeButton.Click();
		}

	}

}
