using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using SecurityEssentials.Acceptance.Tests.Web.Menus;

namespace SecurityEssentials.Acceptance.Tests.Web.Pages
{
    public class HomePage : BasePage
    {
        public HomePage(IWebDriver webDriver, Uri baseUri)
            : base(webDriver, baseUri, PageTitles.HOME)
        {
            MenuBar = new MenuBar(webDriver, baseUri);
        }

        public MenuBar MenuBar { get; }

        private IWebElement Register => GetVisibleWebElement(By.Id("registerLink"));

        private IWebElement Login => GetVisibleWebElement(By.Id("loginLink"));

        public static HomePage NavigateToPage(IWebDriver webDriver, Uri baseUri)
        {
            var userUri = new Uri(baseUri, "Home/Index");
            webDriver.Navigate().GoToUrl(userUri);
            var homePage = new HomePage(webDriver, baseUri);
            PageFactory.InitElements(webDriver, homePage);
            return homePage;
        }

        public void ClickLogin()
        {
            Login.Click();
        }

        public void ClickRegister()
        {
            Register.Click();
        }
    }
}