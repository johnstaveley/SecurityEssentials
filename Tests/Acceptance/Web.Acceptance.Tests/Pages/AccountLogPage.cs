using System;
using OpenQA.Selenium;
using SecurityEssentials.Acceptance.Tests.Menus;

namespace SecurityEssentials.Acceptance.Tests.Pages
{
    public class AccountLogPage : BasePage
    {
        public AccountLogPage(IWebDriver webDriver, Uri baseUri)
            : base(webDriver, baseUri, PageTitles.ACCOUNT_LOG)
        {
            MenuBar = new MenuBar(webDriver, baseUri);
        }

        public MenuBar MenuBar { get; }

        private IWebElement MostRecentMessage => GetVisibleWebElement(By.Id("mostRecentMessage"));

        public string GetMostRecentMessage()
        {
            return MostRecentMessage.Text;
        }
    }
}