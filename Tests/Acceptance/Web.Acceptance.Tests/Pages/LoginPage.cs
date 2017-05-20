using System;
using OpenQA.Selenium;
using SecurityEssentials.Acceptance.Tests.Menus;
using TechTalk.SpecFlow;

namespace SecurityEssentials.Acceptance.Tests.Pages
{
    public class LoginPage : BasePage
    {
        public LoginPage(IWebDriver webDriver, Uri baseUri)
            : base(webDriver, baseUri, PageTitles.LOGIN)
        {
            MenuBar = new MenuBar(webDriver, baseUri);
        }

        public MenuBar MenuBar { get; }

        private IWebElement UserName => GetVisibleWebElement(By.Id("UserName"));

        private IWebElement Password => GetVisibleWebElement(By.Id("Password"));

        private IWebElement RecoverLink => GetVisibleWebElement(By.Id("Recover"));

        private IWebElement LoginButton => GetVisibleWebElement(By.Id("Login"));

        public void ClickSubmit()
        {
            LoginButton.Click();
        }

        public void ClickRecoverPassword()
        {
            RecoverLink.Click();
        }

        public void EnterDetails(Table table)
        {
            foreach (var row in table.Rows)
                switch (row[0].ToLower())
                {
                    case "username":
                        UserName.SendKeys(row[1]);
                        break;
                    case "password":
                        Password.SendKeys(row[1]);
                        break;
                    default:
                        throw new Exception($"Field {row[0]} not defined");
                }
        }
    }
}