using NUnit.Framework;
using SecurityEssentials.Acceptance.Tests.Extensions;
using SecurityEssentials.Acceptance.Tests.Pages;
using SecurityEssentials.Acceptance.Tests.Utility;
using System.Linq;
using TechTalk.SpecFlow;

namespace SecurityEssentials.Acceptance.Tests.Steps
{
    [Binding]
	public class ChangeSecurityInformationSteps : TechTalk.SpecFlow.Steps
	{

		private readonly FeatureContext _featureContext;
        private readonly ScenarioContext _scenarioContext;

        public ChangeSecurityInformationSteps(FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            _featureContext = featureContext;
            _scenarioContext = scenarioContext;
        }

		[Given(@"I enter the following change security information data:")]
		public void GivenIEnterTheFollowingChangeSecurityInformationData(Table table)
		{
			var changeSecurityInformationPage = _scenarioContext.GetPage<ChangeSecurityInformationPage>();
			changeSecurityInformationPage.EnterDetails(table);
		}

		[When(@"I submit the change security information form")]
		public void WhenISubmitTheChangeSecurityInformationForm()
		{
			var changeSecurityInformationPage = _scenarioContext.GetPage<ChangeSecurityInformationPage>();
			changeSecurityInformationPage.ClickSubmit();
		}

		[Given(@"I make a note of the security information and salt for user '(.*)'")]
		public void GivenIMakeANoteOfTheSecurityInformationAndSaltForUser(string userName)
		{
			var user = SeDatabase.GetUsers().Single(a => a.UserName == userName);
			_scenarioContext.SetHash(user.SecurityAnswer);
			_scenarioContext.SetSalt(user.SecurityAnswerSalt);
		}

		[Then(@"The security information for '(.*)' has changed")]
		public void ThenTheSecurityInformationForHasChanged(string userName)
		{
			var user = SeDatabase.GetUsers().Single(a => a.UserName == userName);
			var expectedHash = _scenarioContext.GetHash();
			var expectedSalt = _scenarioContext.GetSalt();
			Assert.That(string.IsNullOrEmpty(expectedHash), Is.False, "Hash has not previously been captured");
			Assert.That(string.IsNullOrEmpty(expectedSalt), Is.False, "Salt has not previously been captured");
			Assert.That(user.PasswordHash, Is.Not.EqualTo(expectedHash), "The hash was expected to have changed");
			Assert.That(user.PasswordSalt, Is.Not.EqualTo(expectedSalt), "The salt was expected to have changed");
		}
	}
}
