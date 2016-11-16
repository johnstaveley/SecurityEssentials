using TechTalk.SpecFlow;
using SecurityEssentials.Acceptance.Tests.Web.Pages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace SecurityEssentials.Acceptance.Tests.Web.Extensions
{
	public static class ScenarioContextExtensions
	{

		public const string KEY_HTTP_HEADERS = "Http_Headers";

		public static IEnumerable<Tuple<string, string>> GetHttpHeaders(this ScenarioContext scenarioContext)
		{
			return scenarioContext.Get<IEnumerable<Tuple<string, string>>>(KEY_HTTP_HEADERS);
		}

		public static void SetHttpHeaders(this ScenarioContext scenarioContext, IEnumerable<Tuple<string, string>> value)
		{
			scenarioContext.Set<IEnumerable<Tuple<string, string>>>(value, KEY_HTTP_HEADERS);
		}

		public static T GetPage<T>(this ScenarioContext scenarioContext) where T : BasePage
		{
			T page = scenarioContext.Get<T>();
			Assert.IsTrue(page.IsCurrentPage, "Unable to load page");
			return page;
		}

		public static bool HasPage<T>(this ScenarioContext scenarioContext) where T : BasePage
		{
			T page;
			return scenarioContext.TryGetValue(out page);
		}

	}
}
