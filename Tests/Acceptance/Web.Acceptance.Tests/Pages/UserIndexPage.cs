using System;
using OpenQA.Selenium;
using SecurityEssentials.Acceptance.Tests.Menus;

namespace SecurityEssentials.Acceptance.Tests.Pages
{
    public class UserIndexPage : BasePage
    {
        public UserIndexPage(IWebDriver webDriver, Uri baseUri)
            : base(webDriver, baseUri, PageTitles.USERS_INDEX)
        {
            MenuBar = new MenuBar(webDriver, baseUri);
        }

        public MenuBar MenuBar { get; }

        private IWebElement EditUser(int id)
        {
            var elements = Driver.FindElements(By.XPath("//a[contains(text(),'Edit')]"));
            return elements[id];
        }

        public void ClickEditUser(int userId)
        {
            EditUser(userId).Click();
        }
    }
}