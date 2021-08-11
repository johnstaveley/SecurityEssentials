using NUnit.Framework;
using SecurityEssentials.Acceptance.Tests.Extensions;
using SecurityEssentials.Acceptance.Tests.Pages;
using SecurityEssentials.Acceptance.Tests.Utility;
using System.Linq;
using TechTalk.SpecFlow;

namespace SecurityEssentials.Acceptance.Tests.Steps
{
	[Binding]
	public class ChangePasswordSteps : TechTalk.SpecFlow.Steps
	{
		private readonly FeatureContext _featureContext;
        private readonly ScenarioContext _scenarioContext;

        public ChangePasswordSteps(FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            _featureContext = featureContext;
            _scenarioContext = scenarioContext;
        }

		[Given(@"I enter the following change password data:")]
		public void GivenIEnterTheFollowingChangePasswordData(Table table)
		{
			var changePasswordPage = _scenarioContext.GetPage<ChangePasswordPage>();
			changePasswordPage.EnterDetails(table);
		}

		[When(@"I submit the change password form")]
		public void WhenISubmitTheChangePassordForm()
		{
			var changePasswordPage = _scenarioContext.GetPage<ChangePasswordPage>();
			changePasswordPage.ClickSubmit();
		}


		[Given(@"I make a note of the password and salt for '(.*)'")]
		public void GivenIMakeANoteOfThePasswordAndSaltFor(string userName)
		{
			var user = SeDatabase.GetUsers().Single(a => a.UserName == userName);
			_scenarioContext.SetHash(user.PasswordHash);
			_scenarioContext.SetSalt(user.PasswordSalt);
		}

		[Then(@"The password for '(.*)' has changed")]
		public void ThenThePasswordForUserXHasChanged(string userName)
		{
			var user = SeDatabase.GetUsers().Single(a => a.UserName == userName);
			var expectedHash = _scenarioContext.GetHash();
			var expectedSalt = _scenarioContext.GetSalt();
			Assert.IsFalse(string.IsNullOrEmpty(expectedHash), "Hash has not previously been captured");
			Assert.IsFalse(string.IsNullOrEmpty(expectedSalt), "Salt has not previously been captured");
			Assert.That(user.PasswordHash, Is.Not.EqualTo(expectedHash), "The hash was expected to have changed");
			Assert.That(user.PasswordSalt, Is.Not.EqualTo(expectedSalt), "The salt was expected to have changed");
		}

	}
}
