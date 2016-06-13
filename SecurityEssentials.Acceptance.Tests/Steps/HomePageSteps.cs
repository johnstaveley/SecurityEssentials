using System;
using System.Globalization;
using SecurityEssentials.Acceptance.Tests.Web.Pages;
using TechTalk.SpecFlow;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using SecurityEssentials.Acceptance.Tests.Utility;

namespace SecurityEssentials.Acceptance.Tests.Web.Extensions
{
	[Binding]
	public class HomePageSteps
	{
		[Then(@"I am taken to the homepage")]
		[When(@"I am taken to the homepage")]
		public void ThenIAmTakenToTheHomepage()
		{
			var homePage = ScenarioContext.Current.GetPage<HomePage>();
			Assert.IsTrue(homePage.IsCurrentPage);
		}

		[Then(@"I can navigate to the '(.*)' page")]
		public void ThenICanNavigateToThePage(string targetPage)
		{
			var homePage = ScenarioContext.Current.GetPage<HomePage>();
			BasePage page = null;
			switch (targetPage)
			{
				//case "ChangePassword":
				//	page = homePage.MenuBar.AdminTab.GotoChangePassword();
					//break;
				default:
					throw new Exception(string.Format("Unknown page {0}", targetPage));
			}
			Task.Factory.StartNew(
				() => Repeater.DoOrTimeout(
					() => page.IsCurrentPage, new TimeSpan(0, 0, 10), new TimeSpan(0, 0, 1)))
				.Wait();

			Assert.IsTrue(page.IsCurrentPage);
			ScenarioContext.Current.Set(page);

		}

	}
}
