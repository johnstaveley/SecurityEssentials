using System;
using OpenQA.Selenium;

namespace SecurityEssentials.Acceptance.Tests.Web.Menus
{
	public class AdminTab : BaseTab
	{
		public AdminTab(IWebDriver driver, Uri baseUri)
			: base(driver, baseUri, TabTitles.ADMIN, "admin", "admin")
		{
		}
	}
}
