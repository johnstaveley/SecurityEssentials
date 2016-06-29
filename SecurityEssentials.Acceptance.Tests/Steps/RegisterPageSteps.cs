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
	public class RegisterPageSteps
	{

		[Given(@"I click register")]
		public void GivenIClickRegister()
		{
			var registerPage = ScenarioContext.Current.GetPage<RegisterPage>();
			registerPage.ClickSubmit();
		}

	}
}
