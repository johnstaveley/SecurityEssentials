using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using TechTalk.SpecFlow;

namespace SecurityEssentials.Acceptance.Tests.Web.Pages
{
	public class RecoverPasswordPage : BasePage
	{

		private IWebElement Password
		{
			get { return this.GetVisibleWebElement(By.Id("Password")); }
		}

		private IWebElement ConfirmPassword
		{
			get { return this.GetVisibleWebElement(By.Id("ConfirmPassword")); }
		}

		private IWebElement SecurityAnswer
		{
			get { return this.GetVisibleWebElement(By.Id("SecurityAnswer")); }
		}

		private IWebElement RecoverButton
		{
			get { return this.GetVisibleWebElement(By.Id("submit")); }
		}

		public RecoverPasswordPage(IWebDriver webDriver, Uri baseUri)
			: base(webDriver, baseUri, PageTitles.RECOVER_PASSWORD)
		{
		}

		public void EnterDetails(Table table)
		{

			foreach (var row in table.Rows)
			{
				switch (row[0].ToLower())
				{
					case "confirm password":
						ConfirmPassword.SendKeys(row[1]);
						break;
					case "password":
						Password.SendKeys(row[1]);
						break;
					case "securityanswer":
						SecurityAnswer.SendKeys(row[1]);
						break;
					default:
						throw new Exception(string.Format("Field {0} not defined", row[0]));
				}
			}
		}

		public void ClickSubmit()
		{
			RecoverButton.Click();
		}

		public static HomePage NavigateToPage(IWebDriver webDriver, Uri baseUri, string passwordResetToken)
		{
			var userUri = new Uri(baseUri, string.Format("Account/RecoverPassword?PasswordResetToken={0}", passwordResetToken));
			webDriver.Navigate().GoToUrl(userUri);
			var homePage = new HomePage(webDriver, baseUri);
			PageFactory.InitElements(webDriver, homePage);
			return homePage;
		}

	}

}
