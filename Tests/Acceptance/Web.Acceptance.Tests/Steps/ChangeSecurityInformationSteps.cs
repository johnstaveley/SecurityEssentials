using System.Linq;
using NUnit.Framework;
using SecurityEssentials.Acceptance.Tests.Extensions;
using SecurityEssentials.Acceptance.Tests.Pages;
using SecurityEssentials.Acceptance.Tests.Utility;
using TechTalk.SpecFlow;

namespace SecurityEssentials.Acceptance.Tests.Steps
{
	[Binding]
	public class ChangeSecurityInformationSteps
	{

		[Given(@"I enter the following change security information data:")]
		public void GivenIEnterTheFollowingChangeSecurityInformationData(Table table)
		{
			var changeSecurityInformationPage = ScenarioContext.Current.GetPage<ChangeSecurityInformationPage>();
			changeSecurityInformationPage.EnterDetails(table);
		}

		[When(@"I submit the change security information form")]
		public void WhenISubmitTheChangeSecurityInformationForm()
		{
			var changeSecurityInformationPage = ScenarioContext.Current.GetPage<ChangeSecurityInformationPage>();
			changeSecurityInformationPage.ClickSubmit();
		}

		[Given(@"I make a note of the security information and salt for user '(.*)'")]
		public void GivenIMakeANoteOfTheSecurityInformationAndSaltForUser(string userName)
		{
			var user = SeDatabase.GetUsers().Single(a => a.UserName == userName);
			ScenarioContext.Current.SetHash(user.SecurityAnswer);
			ScenarioContext.Current.SetSalt(user.SecurityAnswerSalt);
		}

		[Then(@"The security information for '(.*)' has changed")]
		public void ThenTheSecurityInformationForHasChanged(string userName)
		{
			var user = SeDatabase.GetUsers().Single(a => a.UserName == userName);
			var expectedHash = ScenarioContext.Current.GetHash();
			var expectedSalt = ScenarioContext.Current.GetSalt();
			Assert.IsFalse(string.IsNullOrEmpty(expectedHash), "Hash has not previously been captured");
			Assert.IsFalse(string.IsNullOrEmpty(expectedSalt), "Salt has not previously been captured");
			Assert.That(user.PasswordHash, Is.Not.EqualTo(expectedHash), "The hash was expected to have changed");
			Assert.That(user.PasswordSalt, Is.Not.EqualTo(expectedSalt), "The salt was expected to have changed");
		}
	}
}
