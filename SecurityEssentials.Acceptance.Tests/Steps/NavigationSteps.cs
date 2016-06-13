using TechTalk.SpecFlow;
using SecurityEssentials.Acceptance.Tests.Web.Pages;
using SecurityEssentials.Acceptance.Tests.Web.Menus;
using SecurityEssentials.Acceptance.Tests.Web.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SecurityEssentials.Acceptance.Tests.Web.Steps
{

	[Binding]
	public class NavigationSteps
	{
		[Given(@"I navigate to the website")]
		[Given(@"I navigate to the Home page")]
		public void WhenINavigateToTheWebsite()
		{
			var webDriver = FeatureContext.Current.GetWebDriver();
			var uri = FeatureContext.Current.GetBaseUri();
			var homePage = HomePage.NavigateToPage(webDriver, uri);
			ScenarioContext.Current.Set(homePage);
		}

	}
}
