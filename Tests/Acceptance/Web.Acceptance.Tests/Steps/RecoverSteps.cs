using NUnit.Framework;
using SecurityEssentials.Acceptance.Tests.Extensions;
using SecurityEssentials.Acceptance.Tests.Pages;
using TechTalk.SpecFlow;

namespace SecurityEssentials.Acceptance.Tests.Steps
{
	[Binding]
	public class RecoverSteps
	{

		[Given(@"I enter the following recover data:")]
		public void GivenIEnterTheFollowingRecoverData(Table table)
		{
			var recoverPage = ScenarioContext.Current.GetPage<RecoverPage>();
			recoverPage.EnterDetails(table);
		}

		[When(@"I submit the recover form")]
		public void WhenISubmitTheRecoverForm()
		{
			var recoverPage = ScenarioContext.Current.GetPage<RecoverPage>();
			recoverPage.ClickSubmit();
		}

		[Given(@"I am taken to the password recovery page")]
		public void GivenIAmTakenToThePasswordRecoveryPage()
		{
			BasePage page = ScenarioContext.Current.GetPage<RecoverPage>();
			Assert.IsTrue(page.IsCurrentPage);
		}

	}
}
