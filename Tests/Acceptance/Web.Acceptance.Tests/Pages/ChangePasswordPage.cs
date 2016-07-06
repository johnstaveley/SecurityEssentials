using System;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using SecurityEssentials.Acceptance.Tests.Web.Menus;
using TechTalk.SpecFlow;

namespace SecurityEssentials.Acceptance.Tests.Web.Pages
{
	public class ChangePasswordPage : BasePage
	{

		private IWebElement CurrentPassword
		{
			get { return this.GetVisibleWebElement(By.Id("OldPassword")); }
		}

		private IWebElement NewPassword
		{
			get { return this.GetVisibleWebElement(By.Id("NewPassword")); }
		}

		private IWebElement ConfirmNewPassword
		{
			get { return this.GetVisibleWebElement(By.Id("ConfirmPassword")); }
		}

		private IWebElement StatusMessage
		{
			get { return this.GetVisibleWebElement(By.Id("statusMessage")); }
		}

		private IWebElement SubmitButton
		{
			get { return this.GetVisibleWebElement(By.Id("submit")); }
		}

		public ChangePasswordPage(IWebDriver webDriver, Uri baseUri)
			: base(webDriver, baseUri, PageTitles.CHANGE_PASSWORD)
		{
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
						throw new Exception(string.Format("Field {0} not defined", row[0]));
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
