using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using SecurityEssentials.Acceptance.Tests.Menus;
using TechTalk.SpecFlow;

namespace SecurityEssentials.Acceptance.Tests.Pages
{
    public class RecoverPasswordPage : BasePage
    {
        public RecoverPasswordPage(IWebDriver webDriver, Uri baseUri)
            : base(webDriver, baseUri, PageTitles.RECOVER_PASSWORD)
        {
            MenuBar = new MenuBar(webDriver, baseUri);
        }

        public MenuBar MenuBar { get; }

        private IWebElement Password => GetVisibleWebElement(By.Id("Password"));

        private IWebElement ConfirmPassword => GetVisibleWebElement(By.Id("ConfirmPassword"));

        private IWebElement SecurityAnswer => GetVisibleWebElement(By.Id("SecurityAnswer"));

        private IWebElement RecoverButton => GetVisibleWebElement(By.Id("submit"));

        public void EnterDetails(Table table)
        {
            foreach (var row in table.Rows)
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
                        throw new Exception($"Field {row[0]} not defined");
                }
        }

        public void ClickSubmit()
        {
            RecoverButton.Click();
        }

        public static HomePage NavigateToPage(IWebDriver webDriver, Uri baseUri, string passwordResetToken)
        {
            var userUri = new Uri(baseUri,
                $"Account/RecoverPassword?PasswordResetToken={passwordResetToken}");
            webDriver.Navigate().GoToUrl(userUri);
            var homePage = new HomePage(webDriver, baseUri);
            PageFactory.InitElements(webDriver, homePage);
            return homePage;
        }
    }
}