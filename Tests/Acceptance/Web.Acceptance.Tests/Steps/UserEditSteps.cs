using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecurityEssentials.Acceptance.Tests.Extensions;
using SecurityEssentials.Acceptance.Tests.Model;
using SecurityEssentials.Acceptance.Tests.Pages;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SecurityEssentials.Acceptance.Tests.Steps
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

        [Scope(Scenario = "I can change my account information")]
        [Then(@"A confirmation message '(.*)' is shown")]
        public void ThenAConfirmationMessageIsShown(string message)
        {
            var page = ScenarioContext.Current.GetPage<UserEditPage>();
            Assert.IsTrue(page.GetStatusMessage().Contains(message));
        }

        [Then(@"The following user edit information is displayed:")]
        public void ThenTheFollowingUserEditInformationIsDisplayed(Table table)
        {
            var page = ScenarioContext.Current.GetPage<UserEditPage>();
            var user = table.CreateInstance<User>();
            Assert.AreEqual(user.UserName, page.GetUserName());
            Assert.AreEqual(user.Title, page.GetTitle());
            Assert.AreEqual(user.FirstName, page.GetFirstName());
            Assert.AreEqual(user.Surname, page.GetLastName());
            Assert.AreEqual(user.MobileTelephoneNumber, page.GetMobileTelephoneNumber());
            Assert.AreEqual(user.Approved, page.GetApproved());
            Assert.AreEqual(user.EmailVerified, page.GetEmailVerified());
            Assert.AreEqual(user.Enabled, page.GetEnabled());
        }

        [When(@"I click on user (.*)")]
        public void WhenIClickOn(int userId)
        {
            var page = ScenarioContext.Current.GetPage<UserIndexPage>();
            page.ClickEditUser(userId);
        }
    }
}