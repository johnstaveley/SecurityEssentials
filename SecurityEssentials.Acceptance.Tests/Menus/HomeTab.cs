using System;
using SecurityEssentials.Acceptance.Tests.Web.Pages;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;

namespace SecurityEssentials.Acceptance.Tests.Web.Menus
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
