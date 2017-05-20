using System;
using OpenQA.Selenium;

namespace SecurityEssentials.Acceptance.Tests.Menus
{
    public class HomeTab : BaseTab
    {
        public HomeTab(IWebDriver driver, Uri baseUri)
            : base(driver, baseUri, TabTitles.HOME, "home", "home")
        {
        }

        //public XPage GotoPageX()
        //{
        //	ClickMenu();
        //	Click("x");
        //	var xPage = new XPage(_driver, _baseUri);
        //	PageFactory.InitElements(_driver, xPage);
        //	return xPage;
        //}
    }
}