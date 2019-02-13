using NUnit.Framework;
using SecurityEssentials.Acceptance.Tests.Extensions;
using SecurityEssentials.Acceptance.Tests.Pages;
using System.Linq;
using TechTalk.SpecFlow;

namespace SecurityEssentials.Acceptance.Tests.Steps
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

		[Given(@"I click the login button")]
		[When(@"I click the login button")]
		[Then(@"I click the login button")]
		public void GivenIClickTheLoginButton()
		{
			var loginPage = ScenarioContext.Current.GetPage<LoginPage>();
			FeatureContext.Current.WaitForLoginAttempt();
			loginPage.ClickSubmit();
		}

		[Given(@"I click the login button as quickly as possible")]
		[When(@"I click the login button as quickly as possible")]
		public void GivenIClickTheLoginButtonAsQuicklyAsPossible()
		{
			var loginPage = ScenarioContext.Current.GetPage<LoginPage>();
			loginPage.ClickSubmit();
		}

		[Given(@"I enter the following login data:")]
		[Then(@"I enter the following login data:")]
		public void GivenIEnterTheFollowingLoginData(Table table)
		{
			var loginPage = ScenarioContext.Current.GetPage<LoginPage>();
			loginPage.EnterDetails(table);
		}

		[Then(@"The following errors are displayed:")]
		public void ThenTheFollowingErrorsAreDisplayed(Table table)
		{
			var loginPage = ScenarioContext.Current.GetPage<LoginPage>();
			var actualErrors = loginPage.ErrorSummary;
			var expectedErrors = table.Rows.Select(a => a[0]).ToList();
			Assert.IsTrue(actualErrors.SequenceEqual(expectedErrors), "Expected FieldErrors are not present");
		}

	}
}
