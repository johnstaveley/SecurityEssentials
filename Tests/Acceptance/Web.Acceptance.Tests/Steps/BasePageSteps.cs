using NUnit.Framework;
using SecurityEssentials.Acceptance.Tests.Extensions;
using TechTalk.SpecFlow;

namespace SecurityEssentials.Acceptance.Tests.Web.Extensions
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

	}
}
