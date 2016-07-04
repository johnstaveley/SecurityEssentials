using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecurityEssentials.Acceptance.Tests.Web.Pages;
using TechTalk.SpecFlow;

namespace SecurityEssentials.Acceptance.Tests.Web.Extensions
{
	[Binding]
	public class LandingPageSteps
	{
		[Then(@"the following last activity message is shown: '(.*)'")]
		public void ThenTheFollowingLastActivityMessageIsShown(string textToMatch)
		{
			var landingPage = ScenarioContext.Current.GetPage<LandingPage>();
			Assert.IsTrue(landingPage.GetLastAccountActivity().Contains(textToMatch));
		}


	}
}
