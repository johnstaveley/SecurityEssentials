using NUnit.Framework;
using SecurityEssentials.Acceptance.Tests.Model;
using SecurityEssentials.Acceptance.Tests.Utility;
using SecurityEssentials.Core;
using SecurityEssentials.Core.Constants;
using SecurityEssentials.Core.Identity;
using SecurityEssentials.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using User = SecurityEssentials.Model.User;

namespace SecurityEssentials.Acceptance.Tests.Steps
{
    [Binding]
	public class DatabaseSteps
	{

		[Given(@"the following users are setup in the database:")]
		public void GivenTheFollowingUsersAreSetupInTheDatabase(Table table)
		{
			var usersToCreate = table.CreateSet<UserToCreate>().ToList();
			var users = new List<User>();
			var hashStrategy = (HashStrategyKind)Convert.ToInt32(ConfigurationManager.AppSettings["DefaultHashStrategy"]);
			var encryptor = new Encryption();
			var adminRole = SeDatabase.GetRoleByName("Admin");

			foreach (var userToCreate in usersToCreate)
			{
				string encryptedSecurityAnswer;
				string encryptedSecurityAnswerSalt;
				var securePassword = new SecuredPassword(userToCreate.Password, hashStrategy);
				var securityQuestion = SeDatabase.GetLookupItemsByLookupType(Consts.LookupTypeId.SecurityQuestion).Single(a => a.Description == userToCreate.SecurityQuestion);
				encryptor.Encrypt(ConfigurationManager.AppSettings["EncryptionPassword"], Convert.ToInt32(ConfigurationManager.AppSettings["EncryptionIterationCount"]), userToCreate.SecurityAnswer, out encryptedSecurityAnswerSalt, out encryptedSecurityAnswer);

				var user = new User
				{
					UserName = userToCreate.UserName,
					FirstName = userToCreate.FirstName,
					LastName = userToCreate.LastName,
					TelNoWork = userToCreate.WorkTelephoneNumber,
					TelNoHome = userToCreate.HomeTelephoneNumber,
					TelNoMobile = userToCreate.MobileTelephoneNumber,
					Title = userToCreate.Title,
					Town = userToCreate.Town,
					Postcode = userToCreate.Postcode,
					SkypeName = userToCreate.SkypeName,
					HashStrategy = hashStrategy,
					PasswordHash = Convert.ToBase64String(securePassword.Hash),
					PasswordSalt = Convert.ToBase64String(securePassword.Salt),
					SecurityQuestionLookupItemId = securityQuestion.Id,
					SecurityAnswer = encryptedSecurityAnswer,
					SecurityAnswerSalt = encryptedSecurityAnswerSalt,
					Approved = true,
					EmailVerified = true,
					Enabled = true,
					PasswordLastChangedDateUtc = DateTime.UtcNow,
					PasswordResetToken = string.IsNullOrWhiteSpace(userToCreate.PasswordResetToken) ? null : userToCreate.PasswordResetToken,
					PasswordResetExpiryDateUtc = userToCreate.PasswordResetExpiry == "[One day from now]" ? (DateTime?)DateTime.UtcNow.AddDays(1) : null,
					NewEmailAddress = string.IsNullOrWhiteSpace(userToCreate.NewEmailAddress) ? null : userToCreate.NewEmailAddress,
					NewEmailAddressToken = string.IsNullOrWhiteSpace(userToCreate.NewEmailAddressToken) ? null : userToCreate.NewEmailAddressToken,
					NewEmailAddressRequestExpiryDateUtc = userToCreate.NewEmailAddressRequestExpiryDate == "[One day from now]" ? (DateTime?)DateTime.UtcNow.AddDays(1) : null,
					PasswordExpiryDateUtc = userToCreate.PasswordExpiryDate == "[Expired]" ? (DateTime?)DateTime.Now.AddDays(-1) : null
				};
				if (userToCreate.IsAdmin)
				{
					user.UserRoles = new List<UserRole> { new UserRole { RoleId = adminRole.Id, UserId = 1 } };
				}
				users.Add(user);
			}
			SeDatabase.SetUsers(users);

		}		
		[Given(@"the following lookup items are set up in the database:")]
		public void GivenTheFollowingLookupItemsAreSetUpInTheDatabase(Table table)
		{
			var itemsToCreate = table.CreateSet<LookupItem>().ToList();
			SeDatabase.SetLookupItems(itemsToCreate);
		}
	
		[Given(@"the following lookup types are set up in the database:")]
		public void GivenTheFollowingLookupTypesAreSetUpInTheDatabase(Table table)
		{
			var itemsToCreate = table.CreateSet<LookupType>().ToList();
			SeDatabase.SetLookupTypes(itemsToCreate);
		}	
		[Given(@"I have the following logs in the system:")]
		public void GivenIHaveTheFollowingLogsInTheSystem(Table table)
		{
			var dataToCreate = table.CreateSet<Log>().ToList();
			dataToCreate.ForEach(a => a.TimeStamp = DateTime.Now);
			SeDatabase.SetLogs(dataToCreate);
		}		
		[Then(@"I have the following users in the system:")]
		public void ThenIHaveTheFollowingUsersInTheSystem(Table table)
		{
			var users = SeDatabase.GetUsers();
			if (table.Header.Contains("SecurityQuestionLookupItemId"))
			{
				foreach (var tableRow in table.Rows)
				{
					if (tableRow.ContainsKey("SecurityQuestionLookupItemId"))
					{
						if (!string.IsNullOrWhiteSpace(tableRow["SecurityQuestionLookupItemId"]))
						{
							var securityQuestionLookupItem =
								SeDatabase.GetLookupItemByLookupTypeAndDescription(Consts.LookupTypeId.SecurityQuestion, tableRow["SecurityQuestionLookupItemId"]);
							tableRow["SecurityQuestionLookupItemId"] = securityQuestionLookupItem.Id.ToString();
						}
					}
				}
			}
			table.CompareToSet(users);
		}
		[Given(@"I have (.*) entry\(ies\) in the password history table")]
		[Then(@"I have (.*) entry\(ies\) in the password history table")]
		public void GivenIHaveEntryIesInThePasswordHistoryTable(int numberOfPassWordHistoryEntries)
		{
			var passwordHistories = SeDatabase.GetPreviousPasswords();
			Assert.That(passwordHistories.Count, Is.EqualTo(numberOfPassWordHistoryEntries));

		}

		[Then(@"the user '(.*)' has the password reset token set and password reset expiry is at least (.*) minutes from now")]
		public void ThenTheUserHasThePasswordResetTokenSetAndPasswordResetExpirySetToMinutes(string userName, int expiryMinutes)
		{
			var user = SeDatabase.GetUsers().Single(a => a.UserName == userName);
			Assert.IsTrue(!string.IsNullOrEmpty(user.PasswordResetToken), $"User {user.UserName} should have had the password reset token set");
			Assert.IsTrue(user.PasswordResetExpiryDateUtc.HasValue, $"User {user.UserName} should have had the password reset token expiry date set");
			Assert.That(user.PasswordResetExpiryDateUtc.Value, Is.GreaterThan(DateTime.UtcNow.AddMinutes(expiryMinutes)));

		}

		[Given(@"the user '(.*)' has the password expiry date set")]
		[Then(@"the user '(.*)' has the password expiry date set")]
		public void ThenTheUserHasThePasswordExpiryDateSet(string userName)
		{
			var user = SeDatabase.GetUsers().Single(a => a.UserName == userName);
			Assert.IsTrue(user.PasswordExpiryDateUtc.HasValue, $"User {user.UserName} should have had the password expiry date set");
			Assert.That(user.PasswordExpiryDateUtc.Value, Is.LessThan(DateTime.UtcNow));

		}
		[Then(@"the user '(.*)' does not have the password expiry date set")]
		public void ThenTheUserDoesNotHaveThePasswordExpiryDateSet(string userName)
		{
			var user = SeDatabase.GetUsers().Single(a => a.UserName == userName);
			Assert.IsFalse(user.PasswordExpiryDateUtc.HasValue, $"User {user.UserName} should not have the password expiry date set");
		}
		[Then(@"the password reset token and expiry for user '(.*)' are not set")]
		public void ThenThePasswordResetTokenAndExpiryForUserAreNotSet(string userName)
		{
			var user = SeDatabase.GetUsers().Single(a => a.UserName == userName);
			Assert.IsTrue(string.IsNullOrEmpty(user.PasswordResetToken), $"User {user.UserName} should have had the password reset token cleared");
			Assert.IsFalse(user.PasswordResetExpiryDateUtc.HasValue, $"User {user.UserName} should have had the password reset token expiry date cleared");
		}

		[Then(@"the user '(.*)' has the new email address token and expiry cleared")]
		public void ThenTheUserHasTheNewEmailAddressTokenAndExpiryCleared(string userName)
		{
			var user = SeDatabase.GetUsers().Single(a => a.UserName == userName);
			Assert.IsNull(user.NewEmailAddress, "New Email Address should be cleared");
			Assert.IsNull(user.NewEmailAddressToken, $"User {user.UserName} should have had the new email address token cleared");
			Assert.IsFalse(user.NewEmailAddressRequestExpiryDateUtc.HasValue, $"User {user.UserName} should have had the new email address expiry date cleared");
		}

		[Then(@"the user '(.*)' has the new email address token set and new email address expiry is at least (.*) minutes from now")]
		public void ThenTheUserHasTheNewEmailAddressTokenSetAndNewEmailAddressExpiryIsAtLeastMinutesFromNow(string userName, int expiryMinutes)
		{
			var user = SeDatabase.GetUsers().Single(a => a.UserName == userName);
			Assert.IsTrue(!string.IsNullOrEmpty(user.NewEmailAddress), $"User {user.UserName} should have had the new email address set");
			Assert.IsTrue(!string.IsNullOrEmpty(user.NewEmailAddressToken), $"User {user.UserName} should have had the new email address token set");
			Assert.IsTrue(user.NewEmailAddressRequestExpiryDateUtc.HasValue, $"User {user.UserName} should have had the new email address expiry date set");
			Assert.That(user.NewEmailAddressRequestExpiryDateUtc.Value, Is.GreaterThan(DateTime.UtcNow.AddMinutes(expiryMinutes)));
		}
		[Given(@"the following user roles are setup in the system for user '(.*)'")]
		public void GivenTheFollowingUserRolesAreSetupInTheSystemForUser(string userName, Table table)
		{
			var user = SeDatabase.GetUsers().Single(a => a.UserName == userName);
			var userRoles = new List<UserRole>();
			foreach (var row in table.Rows)
			{
				var role = SeDatabase.GetRoleByName(row[0]);
				var userRole = new UserRole
				{
					UserId = user.Id,
					RoleId = role.Id
				};
				userRoles.Add(userRole);
			}
			SeDatabase.SetUserRoles(userRoles);
		}
		[Then(@"I have (.*) content security policy violation in the system")]
		public void ThenIHaveContentSecurityPolicyViolationInTheSystem(int expectedNumberOfCspViolations)
		{
			var cspViolations = SeDatabase.GetCspWarnings();
			Repeater.DoOrTimeout(() =>
			{
				cspViolations = SeDatabase.GetCspWarnings();
				return cspViolations.Count == expectedNumberOfCspViolations;
			}, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(2));
			Assert.That(cspViolations.Count, Is.EqualTo(expectedNumberOfCspViolations), $"Was not able to find {expectedNumberOfCspViolations} csp violations in the logs");
		}
		[Then(@"I have (.*) http public key pinning violation in the system")]
		public void ThenIHaveHttpPublicKeyPinningViolationInTheSystem(int expectedNumberOfHpkpViolations)
		{
			var hpkpWarnings = SeDatabase.GetHpkpWarnings();
			Repeater.DoOrTimeout(() =>
			{
				hpkpWarnings = SeDatabase.GetHpkpWarnings();
				return hpkpWarnings.Count == expectedNumberOfHpkpViolations;
			}, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(2));
			Assert.That(hpkpWarnings.Count, Is.EqualTo(expectedNumberOfHpkpViolations), $"Was not able to find {expectedNumberOfHpkpViolations} hpkp violations in the logs");
		}

		[Then(@"I have a log in the system matching the following:")]
		public void ThenIHaveALogInTheSystemMatchingTheFollowing(Table table)
		{
			var logModel = table.CreateInstance<LogModel>();
			var logs = SeDatabase.GetLogs();
			Assert.IsTrue(logs.Count(a => a.Level == logModel.Level && a.Message.Contains(logModel.Message)) == 1);
		}

		[Then(@"I have the following logs in the system:")]
		public void ThenIHaveTheFollowingLogsInTheSystem(Table table)
		{
			var logs = SeDatabase.GetLogs();
			table.CompareToSet(logs);
		}
		[Then(@"I have the following user logs in the system:")]
		public void ThenIHaveTheFollowingUserLogsInTheSystem(Table table)
		{
			var userLogs = SeDatabase.GetUserLogs();
			table.CompareToSet(userLogs);
		}

		[Given(@"I have the standard set of lookups")]
		public void GivenIHaveTheStandardSetOfLookups()
		{
			DatabaseCommand.Execute("SecurityEssentials.Acceptance.Tests.Resources.LookupRestore.sql");
		}

		[Given(@"I clear down the database")]
		public void GivenIClearDownTheDatabase()
		{
			SeDatabase.ClearDatabase();
		}

	}
}
