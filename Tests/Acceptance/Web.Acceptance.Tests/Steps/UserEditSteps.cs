using SecurityEssentials.Acceptance.Tests.Web.Pages;
using TechTalk.SpecFlow;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SecurityEssentials.Acceptance.Tests.Web.Extensions
{
	[Binding]
	public class UserEditSteps
	{

		[Given(@"I enter the following change account information data:")]
		public void GivenIEnterTheFollowingChangeAccountInformationData(Table table)
		{
			var page = ScenarioContext.Current.GetPage<UserEditPage>();
			page.EnterDetails(table);
		}

		[When(@"I submit the manage account form")]
		public void WhenISubmitTheManageAccountForm()
		{
			var page = ScenarioContext.Current.GetPage<UserEditPage>();
			page.ClickSubmit();
		}

	}
}
