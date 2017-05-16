using System;
using OpenQA.Selenium;
using SecurityEssentials.Acceptance.Tests.Web.Menus;
using TechTalk.SpecFlow;

namespace SecurityEssentials.Acceptance.Tests.Web.Pages
{
    public class ChangePasswordPage : BasePage
    {
        public ChangePasswordPage(IWebDriver webDriver, Uri baseUri)
            : base(webDriver, baseUri, PageTitles.CHANGE_PASSWORD)
        {
            MenuBar = new MenuBar(webDriver, baseUri);
        }

        public MenuBar MenuBar { get; }

        private IWebElement CurrentPassword => GetVisibleWebElement(By.Id("OldPassword"));

        private IWebElement NewPassword => GetVisibleWebElement(By.Id("NewPassword"));

        private IWebElement ConfirmNewPassword => GetVisibleWebElement(By.Id("ConfirmPassword"));

        private IWebElement StatusMessage => GetVisibleWebElement(By.Id("statusMessage"));

        private IWebElement SubmitButton => GetVisibleWebElement(By.Id("submit"));

        public void EnterDetails(Table table)
        {
            foreach (var row in table.Rows)
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