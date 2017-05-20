using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SecurityEssentials.Acceptance.Tests.Menus;
using TechTalk.SpecFlow;

namespace SecurityEssentials.Acceptance.Tests.Pages
{
    public class RegisterPage : BasePage
    {
        public RegisterPage(IWebDriver webDriver, Uri baseUri)
            : base(webDriver, baseUri, PageTitles.REGISTER)
        {
            MenuBar = new MenuBar(webDriver, baseUri);
        }

        public MenuBar MenuBar { get; }

        private IWebElement Submit => GetVisibleWebElement(By.Id("submit"));

        private IWebElement Username => GetVisibleWebElement(By.Id("User_UserName"));

        private IWebElement FirstName => GetVisibleWebElement(By.Id("User_FirstName"));

        private IWebElement LastName => GetVisibleWebElement(By.Id("User_LastName"));

        private SelectElement SecurityQuestion => new SelectElement(
            GetVisibleWebElement(By.Id("User_SecurityQuestionLookupItemId")));

        private IWebElement SecurityAnswer => GetVisibleWebElement(By.Id("User_SecurityAnswer"));

        private IWebElement Password => GetVisibleWebElement(By.Id("Password"));

        private IWebElement ConfirmPassword => GetVisibleWebElement(By.Id("ConfirmPassword"));

        public void ClickSubmit()
        {
            Submit.Click();
        }

        public void EnterDetails(Table table)
        {
            foreach (var row in table.Rows)
                switch (row[0].ToLower())
                {
                    case "username":
                        Username.SendKeys(row[1]);
                        break;
                    case "firstname":
                        FirstName.SendKeys(row[1]);
                        break;
                    case "lastname":
                        LastName.SendKeys(row[1]);
                        break;
                    case "securityquestion":
                        SecurityQuestion.SelectByText(row[1]);
                        break;
                    case "securityanswer":
                        SecurityAnswer.SendKeys(row[1]);
                        break;
                    case "password":
                        Password.SendKeys(row[1]);
                        break;
                    case "confirmpassword":
                        ConfirmPassword.SendKeys(row[1]);
                        break;
                    default:
                        throw new Exception($"Field {row[0]} not defined");
                }
        }
    }
}