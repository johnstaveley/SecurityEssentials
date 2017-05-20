using System;
using OpenQA.Selenium;
using SecurityEssentials.Acceptance.Tests.Menus;
using TechTalk.SpecFlow;

namespace SecurityEssentials.Acceptance.Tests.Pages
{
    public class RecoverPage : BasePage
    {
        public RecoverPage(IWebDriver webDriver, Uri baseUri)
            : base(webDriver, baseUri, PageTitles.RECOVER)
        {
            MenuBar = new MenuBar(webDriver, baseUri);
        }

        public MenuBar MenuBar { get; }

        private IWebElement UserName => GetVisibleWebElement(By.Id("UserName"));

        private IWebElement RecoverButton => GetVisibleWebElement(By.Id("recoverButton"));


        public void EnterDetails(Table table)
        {
            foreach (var row in table.Rows)
                switch (row[0].ToLower())
                {
                    case "username":
                        UserName.SendKeys(row[1]);
                        break;
                    default:
                        throw new Exception($"Field {row[0]} not defined");
                }
        }

        public void ClickSubmit()
        {
            RecoverButton.Click();
        }
    }
}