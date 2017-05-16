using System;
using OpenQA.Selenium;
using SecurityEssentials.Acceptance.Tests.Web.Menus;

namespace SecurityEssentials.Acceptance.Tests.Web.Pages
{
    public class ChangeEmailAddressSuccessPage : BasePage
    {
        public ChangeEmailAddressSuccessPage(IWebDriver webDriver, Uri baseUri)
            : base(webDriver, baseUri, PageTitles.CHANGE_EMAIL_ADDRESS_SUCCESS)
        {
            MenuBar = new MenuBar(webDriver, baseUri);
        }

        public MenuBar MenuBar { get; }
    }
}