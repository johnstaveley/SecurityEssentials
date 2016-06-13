using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace SecurityEssentials.Acceptance.Tests.Web.Menus
{
	public class MenuBar
	{
		public HomeTab HomeTab { get; private set; }
		public AdminTab AdminTab { get; private set; }
		private IList<BaseTab> Tabs { get; set; }

		public MenuBar(IWebDriver driver, Uri baseUri)
		{
			HomeTab = new HomeTab(driver, baseUri);
			AdminTab = new AdminTab(driver, baseUri);

			Tabs = new List<BaseTab>();
			Tabs.Add(HomeTab);
			Tabs.Add(AdminTab);
		}		

		public BaseTab GetTab(string tabTitle)
		{
			return Tabs.SingleOrDefault(t => t.Title == tabTitle);
		}

	}
}
