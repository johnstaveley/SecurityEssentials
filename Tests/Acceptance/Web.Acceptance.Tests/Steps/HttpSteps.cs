using NUnit.Framework;
using SecurityEssentials.Acceptance.Tests.Extensions;
using SecurityEssentials.Acceptance.Tests.Model;
using SecurityEssentials.Acceptance.Tests.Utility;
using System;
using System.Configuration;
using System.Linq;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SecurityEssentials.Acceptance.Tests.Steps
{

    [Binding]
	public class HttpSteps
	{


		[When(@"I call http get on the website")]
		public void WhenICallHttpGetOnTheWebsite()
		{
			var response = HttpWeb.Get(ConfigurationManager.AppSettings["WebServerUrl"]);
			var headers = Enumerable
				.Range(0, response.Headers.Count)
				.Select(v => Tuple.Create(response.Headers[v].Name, response.Headers[v].Value.ToString()));
			ScenarioContext.Current.SetHttpHeaders(headers);
		}

		[Then(@"the response headers will contain:")]
		public void ThenTheResponseHeadersWillContain(Table table)
		{
			var actualHeaders = ScenarioContext.Current.GetHttpHeaders().ToList();
			var expectedHeaders = table.CreateSet<HttpHeader>().ToList();			
			foreach (var expectedHeader in expectedHeaders)
			{
				Assert.IsTrue(actualHeaders.ToList().Any(a => a.Item1 == expectedHeader.Key), $"Headers do not contain the key '{expectedHeader.Key}'");
				var actualHeader = actualHeaders.First(a => a.Item1 == expectedHeader.Key);
				if (actualHeader.Item1 == "Content-Security-Policy" && ConfigurationManager.AppSettings["WebServerUrl"].StartsWith("https:")) {
					expectedHeader.Value = expectedHeader.Value.Replace("font-src 'self' https:", "font-src https:").Replace("*", "https:").Replace("'self'", "https:");
				}
				Assert.AreEqual(expectedHeader.Value, actualHeader.Item2, $"Header values do not match for key '{actualHeader.Item1}'");
            }
		}

		[Then(@"the response headers will not contain:")]
		public void ThenTheResponseHeadersWillNotContain(Table table)
		{
			var actualHeaders = ScenarioContext.Current.GetHttpHeaders().ToList();
			var excludedHeaders = table.CreateSet<HttpHeader>();
			foreach (var excludedHeader in excludedHeaders)
			{
				Assert.IsFalse(actualHeaders.Any(a => a.Item1 == excludedHeader.Key), $"Headers contain a header key '{excludedHeader.Key}' when it should not");
			}
		}
	}
}
