using System;
using OpenQA.Selenium;
using TechTalk.SpecFlow;
using OpenQA.Selenium.Support.UI;
using SecurityEssentials.Acceptance.Tests.Web.Menus;

namespace SecurityEssentials.Acceptance.Tests.Web.Pages
{
	public class ChangeSecurityInformationPage : BasePage
	{

		public MenuBar MenuBar { get; private set; }

		private IWebElement Password
		{
			get { return this.GetVisibleWebElement(By.Id("Password")); }
		}

		private SelectElement SecurityQuestion
		{
			get { return new SelectElement(this.GetVisibleWebElement(By.Id("SecurityQuestionLookupItemId"))); }
		}

		private IWebElement SecurityAnswer
		{
			get { return this.GetVisibleWebElement(By.Id("SecurityAnswer")); }
		}

		private IWebElement SecurityAnswerConfirm
		{
			get { return this.GetVisibleWebElement(By.Id("SecurityAnswerConfirm")); }
		}

		private IWebElement SubmitButton
		{
			get { return this.GetVisibleWebElement(By.Id("submit")); }
		}

		public ChangeSecurityInformationPage(IWebDriver webDriver, Uri baseUri)
			: base(webDriver, baseUri, PageTitles.CHANGE_SECURITY_INFORMATION)
		{
			MenuBar = new MenuBar(webDriver, baseUri);
		}

		public void EnterDetails(Table table)
		{

			foreach (var row in table.Rows)
			{
				switch (row[0].ToLower())
				{
					case "password":
						Password.SendKeys(row[1]);
						break;
					case "securityquestion":
						SecurityQuestion.SelectByText(row[1]);
						break;
					case "securityanswer":
						SecurityAnswer.SendKeys(row[1]);
						break;
					case "securityanswerconfirm":
						SecurityAnswerConfirm.SendKeys(row[1]);
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

	}
}
