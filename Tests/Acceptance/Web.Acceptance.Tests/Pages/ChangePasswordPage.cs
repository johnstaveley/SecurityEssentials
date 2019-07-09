using System;
using OpenQA.Selenium;
using SecurityEssentials.Acceptance.Tests.Menus;
using TechTalk.SpecFlow;

namespace SecurityEssentials.Acceptance.Tests.Pages
{
	public class ChangePasswordPage : BasePage
	{
		public MenuBar MenuBar { get; }

		private IWebElement CurrentPassword => GetVisibleWebElement(By.Id("OldPassword"));

		private IWebElement NewPassword => GetVisibleWebElement(By.Id("NewPassword"));

		private IWebElement ConfirmNewPassword => GetVisibleWebElement(By.Id("ConfirmPassword"));

		private IWebElement StatusMessage => GetVisibleWebElement(By.Id("statusMessage"));

		private IWebElement SubmitButton => GetVisibleWebElement(By.Id("submit"));

		public ChangePasswordPage(IWebDriver webDriver, Uri baseUri)
			: base(webDriver, baseUri, PageTitles.CHANGE_PASSWORD)
		{
			MenuBar = new MenuBar(webDriver, baseUri);
		}

		public void EnterDetails(Table table)
		{

			foreach (var row in table.Rows)
			{
				switch (row[0].ToLower())
				{
					case "currentpassword":
						CurrentPassword.SendKeys(row[1]);
						break;
					case "newpassword":
						NewPassword.SendKeys(row[1]);
						break;
					case "confirmnewpassword":
						ConfirmNewPassword.SendKeys(row[1]);
						break;
					default:
						throw new Exception($"Field {row[0]} not defined");
				}
			}
		}

		public void ClickSubmit()
		{
			SubmitButton.Click();
		}

		public string GetStatusMessage()
		{
			return StatusMessage.Text;
		}

	}

}
