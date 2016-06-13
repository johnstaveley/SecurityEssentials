using TechTalk.SpecFlow;
using SecurityEssentials.Acceptance.Tests.Web.Pages;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SecurityEssentials.Acceptance.Tests.Web.Extensions
{
	public static class ScenarioContextExtensions
	{
		public static T GetPage<T>(this ScenarioContext scenarioContext) where T : BasePage
		{
			T page = scenarioContext.Get<T>();
			Assert.IsTrue(page.IsCurrentPage);
			return page;
		}

		public static bool HasPage<T>(this ScenarioContext scenarioContext) where T : BasePage
		{
			T page;
			return scenarioContext.TryGetValue(out page);
		}
	}
}
