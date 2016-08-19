using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecurityEssentials.Acceptance.Tests.Web.Pages;
using TechTalk.SpecFlow;

namespace SecurityEssentials.Acceptance.Tests.Web.Extensions
{
	[Binding]
	public class ChangeEmailAddressSteps
	{

		[Given(@"I enter the following change email address data:")]
		public void GivenIEnterTheFollowingChangeEmailAddressData(Table table)
		{
			var changeEmailAddressPage = ScenarioContext.Current.GetPage<ChangeEmailAddressPage>();
			changeEmailAddressPage.EnterDetails(table);
		}


	}
}
