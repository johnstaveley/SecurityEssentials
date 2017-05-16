using System;
using OpenQA.Selenium;
using SecurityEssentials.Acceptance.Tests.Web.Menus;

namespace SecurityEssentials.Acceptance.Tests.Web.Pages
{
    public class RecoverPasswordSuccessPage : BasePage
    {
        public RecoverPasswordSuccessPage(IWebDriver webDriver, Uri baseUri)
            : base(webDriver, baseUri, PageTitles.RECOVER_PASSWORD_SUCCESS)
        {
            MenuBar = new MenuBar(webDriver, baseUri);
        }

        public MenuBar MenuBar { get; }
    }
}