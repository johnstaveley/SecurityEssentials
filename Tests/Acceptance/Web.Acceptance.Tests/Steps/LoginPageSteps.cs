using System;
using System.Globalization;
using SecurityEssentials.Acceptance.Tests.Web.Pages;
using TechTalk.SpecFlow;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using SecurityEssentials.Acceptance.Tests.Utility;

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
			loginPage.ClickRecoverPassword();
		}

		[Given(@"I enter the following login data:")]
		public void GivenIEnterTheFollowingLoginData(Table table)
		{
			var loginPage = ScenarioContext.Current.GetPage<LoginPage>();
			loginPage.EnterDetails(table);
		}


	}
}
