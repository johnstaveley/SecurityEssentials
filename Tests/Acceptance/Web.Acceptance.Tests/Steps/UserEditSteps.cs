using NUnit.Framework;
using SecurityEssentials.Acceptance.Tests.Extensions;
using SecurityEssentials.Acceptance.Tests.Model;
using SecurityEssentials.Acceptance.Tests.Pages;
using System;
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
			Assert.IsTrue(page.GetStatusMessage().Contains(message), $"A confirmation message '{message}' should have been displayed");
		}

		[Then(@"The following user edit information is displayed:")]
		public void ThenTheFollowingUserEditInformationIsDisplayed(Table table)
		{
			var page = ScenarioContext.Current.GetPage<UserEditPage>();
			var user = table.CreateInstance<UserViewModel>();
			foreach (var row in table.Rows)
			{
				switch (row[0].ToLower().Replace(" ", ""))
				{
					case "approved":
						Assert.AreEqual(user.Approved, page.GetApproved());
						break;
					case "emailverified":
						Assert.AreEqual(user.EmailVerified, page.GetEmailVerified());
						break;
					case "enabled":
						Assert.AreEqual(user.Enabled, page.GetEnabled());
						break;
					case "firstname":
						Assert.AreEqual(user.FirstName, page.GetFirstName());
						break;
					case "hometelephonenumber":
						Assert.AreEqual(user.HomeTelephoneNumber, page.GetTelNoHome());
						break;
					case "lastname":
						Assert.AreEqual(user.LastName, page.GetLastName());
						break;
					case "mobiletelephonenumber":
						Assert.AreEqual(user.MobileTelephoneNumber, page.GetTelNoMobile());
						break;
					case "postcode":
						Assert.AreEqual(user.Postcode, page.GetPostcode());
						break;
					case "skypename":
						Assert.AreEqual(user.SkypeName, page.GetSkypeName());
						break;
					case "title":
						Assert.AreEqual(user.Title, page.GetTitle());
						break;
					case "town":
						Assert.AreEqual(user.Town, page.GetTown());
						break;
					case "username":
						Assert.AreEqual(user.UserName, page.GetUserName());
						break;
					case "worktelephonenumber":
						Assert.AreEqual(user.WorkTelephoneNumber, page.GetTelNoWork());
						break;

					default:
						throw new Exception($"Field {row[0]} not defined");
				}
			}
		}

		[Given(@"I click edit on the user with name '(.*)'")]
		[When(@"I click edit on the user with name '(.*)'")]
		public void WhenIClickEditOnTheUserWithName(string name)
		{
			var page = ScenarioContext.Current.GetPage<UserIndexPage>();
			page.ClickEditUserWithName(name);
		}

		[Given(@"The text indicating the user is a system administrator is not displayed")]
		[Then(@"The text indicating the user is a system administrator is not displayed")]
		public void ThenTheTextIndicatingTheUserIsASystemAdministratorIsDisplayed()
		{
			var page = ScenarioContext.Current.GetPage<UserEditPage>();
			Assert.That(page.GetIfTextIndicatingUserIsAnAdminExists, Is.False, "The text indicating the user is an admin was  displayed and it shouldn't have");
		}
		[Given(@"The text indicating the user is a system administrator is displayed")]
		[Then(@"The text indicating the user is a system administrator is displayed")]
		public void GivenTheTextIndicatingTheUserIsASystemAdministratorIsDisplayed()
		{
			var page = ScenarioContext.Current.GetPage<UserEditPage>();
			Assert.That(page.GetIfTextIndicatingUserIsAnAdminExists, Is.True, "The text indicating the user is an admin was not displayed");
		}
		[Given(@"I click Remove Administrator Privilege")]
		public void GivenIClickRemoveAdministratorPrivilege()
		{
			var page = ScenarioContext.Current.GetPage<UserEditPage>();
			page.ClickRemoveAdmin();
		}
		[Given(@"I click Make Administrator Privilege")]
		public void GivenIClickMakeAdministratorPrivilege()
		{
			var page = ScenarioContext.Current.GetPage<UserEditPage>();
			page.ClickMakeAdmin();
		}		
		[Then(@"I can edit the username")]
		public void ThenICanEditTheUsername()
		{
			var page = ScenarioContext.Current.GetPage<UserEditPage>();
			Assert.That(page.IsUserNameEditable(), Is.True, "Should be able to edit the username but was unable");
		}
		[Then(@"I cannot edit the username")]
		public void ThenICannotEditTheUsername()
		{
			var page = ScenarioContext.Current.GetPage<UserEditPage>();
			Assert.That(page.IsUserNameEditable(), Is.False, "Shouldn't be able to edit the username");
		}			
		[Then(@"I am shown the following users:")]
		[Given(@"I am shown the following users:")]
		public void GivenIAmShownTheFollowingUsers(Table table)
		{
			var page = ScenarioContext.Current.GetPage<UserIndexPage>();
			var usersDisplayed = page.GetUsersDisplayed();
			table.CompareToSet(usersDisplayed);
		}	
		[When(@"I click confirm remove admin")]
		public void WhenIClickConfirmRemoveAdmin()
		{
			var page = ScenarioContext.Current.GetPage<RemoveAdminPage>();
			page.ClickSubmit();
		}
		[When(@"I click confirm make admin")]
		public void WhenIClickConfirmMakeAdmin()
		{
			var page = ScenarioContext.Current.GetPage<MakeAdminPage>();
			page.ClickSubmit();
		}
		[Given(@"I am shown the following make admin details:")]
		public void GivenIAmShownTheFollowingMakeAdminDetails(Table table)
		{
			var page = ScenarioContext.Current.GetPage<MakeAdminPage>();
			var user = table.CreateInstance<UserViewModel>();
			Assert.AreEqual(user.UserName, page.GetUserName());
		}
		[Given(@"I am shown the following remove admin details:")]
		public void GivenIAmShownTheFollowingRemoveAdminDetails(Table table)
		{
			var page = ScenarioContext.Current.GetPage<RemoveAdminPage>();
			var user = table.CreateInstance<UserViewModel>();
			Assert.AreEqual(user.UserName, page.GetUserName());
		}		
		[Given(@"I click Delete User")]
		public void GivenIClickDeleteUser()
		{
			var page = ScenarioContext.Current.GetPage<UserEditPage>();
			page.ClickDeleteUser();
		}

		[Given(@"I click Reset Password")]
		public void GivenIClickResetPassword()
		{
			var page = ScenarioContext.Current.GetPage<UserEditPage>();
			page.ClickResetPassword();
		}
		[Given(@"I am shown the following reset password details:")]
		public void GivenIAmShownTheFollowingResetPasswordDetails(Table table)
		{
			var page = ScenarioContext.Current.GetPage<ResetPasswordPage>();
			var user = table.CreateInstance<UserViewModel>();
			Assert.AreEqual(user.UserName, page.GetUserName());
		}
		[When(@"I click confirm reset password")]
		public void WhenIClickConfirmResetPassword()
		{
			var page = ScenarioContext.Current.GetPage<ResetPasswordPage>();
			page.ClickSubmit();
		}
		[Given(@"I am shown the following delete user details:")]
		public void GivenIAmShownTheFollowingDeleteUserDetails(Table table)
		{
			var page = ScenarioContext.Current.GetPage<DeleteUserPage>();
			var user = table.CreateInstance<UserViewModel>();
			Assert.AreEqual(user.UserName, page.GetUserName());
		}
		[When(@"I click confirm delete user")]
		public void WhenIClickConfirmDeleteUser()
		{
			var page = ScenarioContext.Current.GetPage<DeleteUserPage>();
			page.ClickSubmit();
		}
	}
}
