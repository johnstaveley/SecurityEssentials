using NUnit.Framework;
using SecurityEssentials.Acceptance.Tests.Extensions;
using SecurityEssentials.Acceptance.Tests.Pages;
using System;
using System.Linq;
using TechTalk.SpecFlow;

namespace SecurityEssentials.Acceptance.Tests.Steps
{
    [Binding]
	public class BasePageSteps
	{

		[Then(@"an error message is shown '(.*)'")]
		public void ThenAnErrorMessageIsShown(string errorMessage)
		{
			var driver = FeatureContext.Current.GetWebDriver();
			Assert.IsTrue(driver.PageSource.Contains(errorMessage), $"Page should have contained error message '{errorMessage}'");
		}
        [Then(@"The following errors are displayed on the '(.*)' page:")]
        public void ThenTheFollowingErrorsAreDisplayedOnThePage(string pageName, Table table)
        {
            BasePage page = null;
            switch (pageName.ToLower())
            {
                case "login":
                    page = ScenarioContext.Current.GetPage<LoginPage>();
                    break;
                case "register":
                    page = ScenarioContext.Current.GetPage<RegisterPage>();
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
