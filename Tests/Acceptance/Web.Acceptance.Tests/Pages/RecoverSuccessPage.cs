using System;
using OpenQA.Selenium;
using SecurityEssentials.Acceptance.Tests.Web.Menus;

namespace SecurityEssentials.Acceptance.Tests.Web.Pages
{
    public class RecoverSuccessPage : BasePage
    {
        public RecoverSuccessPage(IWebDriver webDriver, Uri baseUri)
            : base(webDriver, baseUri, PageTitles.RECOVER_SUCCESS)
        {
            MenuBar = new MenuBar(webDriver, baseUri);
        }

        public MenuBar MenuBar { get; }
    }
}