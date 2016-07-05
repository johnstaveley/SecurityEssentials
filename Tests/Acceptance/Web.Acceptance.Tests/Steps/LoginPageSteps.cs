using SecurityEssentials.Acceptance.Tests.Web.Pages;
using TechTalk.SpecFlow;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SecurityEssentials.Acceptance.Tests.Web.Extensions
{
	[Binding]
	public class LoginPageSteps
	{

        [Given(@"I click recover password")]
		public void GivenIClickRecoverPassword()
		{
			var loginPage = ScenarioContext.Current.GetPage<LoginPage>();
			loginPage.ClickRecoverPassword();
		}

		[When(@"I click the login button")]
		[Given(@"I click the login button")]
		public void GivenIClickTheLoginButton()
		{
			var loginPage = ScenarioContext.Current.GetPage<LoginPage>();
			loginPage.ClickSubmit();
		}

		[Given(@"I enter the following login data:")]
		public void GivenIEnterTheFollowingLoginData(Table table)
		{
			var loginPage = ScenarioContext.Current.GetPage<LoginPage>();
			loginPage.EnterDetails(table);
		}

		[Then(@"The following errors are displayed:")]
		public void ThenTheFollowingErrorsAreDisplayed(Table table)
		{
			var loginPage = ScenarioContext.Current.GetPage<LoginPage>();
			var actualErrors = loginPage.Errors;
			var expectedErrors = table.Rows.Select(a => a[0]).ToList();
			Assert.IsTrue(actualErrors.SequenceEqual(expectedErrors));
		}

	}
}
