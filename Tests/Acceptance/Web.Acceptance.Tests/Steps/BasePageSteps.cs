using NUnit.Framework;
using SecurityEssentials.Acceptance.Tests.Extensions;
using SecurityEssentials.Acceptance.Tests.Pages;
using System;
using System.Linq;
using TechTalk.SpecFlow;

namespace SecurityEssentials.Acceptance.Tests.Steps
{
    [Binding]
	public class BasePageSteps : TechTalk.SpecFlow.Steps
	{

        private readonly FeatureContext _featureContext;
        private readonly ScenarioContext _scenarioContext;

        public BasePageSteps(FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            _featureContext = featureContext;
            _scenarioContext = scenarioContext;
        }

		[Then(@"an error message is shown '(.*)'")]
		public void ThenAnErrorMessageIsShown(string errorMessage)
		{
			var driver = _featureContext.GetWebDriver();
			Assert.IsTrue(driver.PageSource.Contains(errorMessage), $"Page should have contained error message '{errorMessage}'");
		}
        [Then(@"The following errors are displayed on the '(.*)' page:")]
        public void ThenTheFollowingErrorsAreDisplayedOnThePage(string pageName, Table table)
        {
            BasePage page = null;
            switch (pageName.ToLower())
            {
                case "login":
                    page = _scenarioContext.GetPage<LoginPage>();
                    break;
                case "register":
                    page = _scenarioContext.GetPage<RegisterPage>();
                    break;
                default:
                    throw new Exception($"Unknown page {pageName}");
            }
            var actualErrors = page.ErrorSummary;
            var expectedErrors = table.Rows.Select(a => a[0]).ToList();
            Assert.IsTrue(actualErrors.SequenceEqual(expectedErrors), "Expected FieldErrors are not present");
        }
	}
}
