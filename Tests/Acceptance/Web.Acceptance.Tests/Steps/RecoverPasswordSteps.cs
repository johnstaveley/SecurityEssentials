using SecurityEssentials.Acceptance.Tests.Web.Pages;
using TechTalk.SpecFlow;

namespace SecurityEssentials.Acceptance.Tests.Web.Extensions
{
	[Binding]
	public class RecoverPasswordSteps
	{

		[Given(@"I enter the following recover password data:")]
		public void GivenIEnterTheFollowingRecoverData(Table table)
		{
			var recoverPasswordPage = ScenarioContext.Current.GetPage<RecoverPasswordPage>();
			recoverPasswordPage.EnterDetails(table);
		}

		[When(@"I submit the recover passord form")]
		public void WhenISubmitTheRecoverPassordForm()
		{
			var recoverPasswordPage = ScenarioContext.Current.GetPage<RecoverPasswordPage>();
			recoverPasswordPage.ClickSubmit();
		}

		[Given(@"I navigate to the password reset link with token '(.*)'")]
		public void GivenINavigateToPasswordResetLinkWithToken(string passwordResetToken)
		{
			var recoverPasswordPage = RecoverPasswordPage.NavigateToPage(FeatureContext.Current.GetWebDriver(), FeatureContext.Current.GetBaseUri(), passwordResetToken);
			
		}


	}
}
