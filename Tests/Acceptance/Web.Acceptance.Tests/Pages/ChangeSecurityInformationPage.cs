using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SecurityEssentials.Acceptance.Tests.Web.Menus;
using System;
using SecurityEssentials.Acceptance.Tests.Menus;
using TechTalk.SpecFlow;

namespace SecurityEssentials.Acceptance.Tests.Pages
{
	public class ChangeSecurityInformationPage : BasePage
	{

		public MenuBar MenuBar { get; }

		private IWebElement Password => GetVisibleWebElement(By.Id("Password"));

		private SelectElement SecurityQuestion => new SelectElement(GetVisibleWebElement(By.Id("SecurityQuestionLookupItemId")));

		private IWebElement SecurityAnswer => GetVisibleWebElement(By.Id("SecurityAnswer"));

		private IWebElement SecurityAnswerConfirm => GetVisibleWebElement(By.Id("SecurityAnswerConfirm"));

		private IWebElement SubmitButton => GetVisibleWebElement(By.Id("submit"));

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
						throw new Exception($"Field {row[0]} not defined");
				}
			}
		}

		public void ClickSubmit()
		{
			SubmitButton.Click();
		}

	}
}
