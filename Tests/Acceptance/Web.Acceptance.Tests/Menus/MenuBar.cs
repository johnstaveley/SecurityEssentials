using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;

namespace SecurityEssentials.Acceptance.Tests.Menus
{
	public class MenuBar
	{
		public HomeTab HomeTab { get; }
		public AdminTab AdminTab { get; }
		private IList<BaseTab> Tabs { get; }

		public MenuBar(IWebDriver driver, Uri baseUri)
		{
			HomeTab = new HomeTab(driver, baseUri);
			AdminTab = new AdminTab(driver, baseUri);

			Tabs = new List<BaseTab> {HomeTab, AdminTab};
		}		

		public BaseTab GetTab(string tabTitle)
		{
			return Tabs.SingleOrDefault(t => t.Title == tabTitle);
		}

	}
}
