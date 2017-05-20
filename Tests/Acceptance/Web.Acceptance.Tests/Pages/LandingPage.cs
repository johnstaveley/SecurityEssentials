using System;
using OpenQA.Selenium;
using SecurityEssentials.Acceptance.Tests.Menus;

namespace SecurityEssentials.Acceptance.Tests.Pages
{
    public class LandingPage : BasePage
    {
        public LandingPage(IWebDriver webDriver, Uri baseUri)
            : base(webDriver, baseUri, PageTitles.LANDING)
        {
            MenuBar = new MenuBar(webDriver, baseUri);
        }

        public MenuBar MenuBar { get; }

        private IWebElement LastAccountActivity => GetVisibleWebElement(By.Id("LastAccountActivity"));

        public string GetLastAccountActivity()
        {
            return LastAccountActivity.Text;
        }
    }
}