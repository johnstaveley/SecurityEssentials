using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using SecurityEssentials.Core;
using SecurityEssentials.Core.Identity;
using SecurityEssentials.Model;
using SecurityEssentials.Unit.Tests.TestDbSet;

namespace SecurityEssentials.Unit.Tests.Core.Identity
{

	[TestFixture]
	public class UserStoreTests
	{

		UserStore<User> _sut;
		ISeContext _context;
		protected User TestUser;
		private string _testRoleName;
		private IAppConfiguration _configuration;

		[SetUp]
		public void Setup()
		{
			_context = MockRepository.GenerateStub<ISeContext>();
			_context.User = new TestDbSet<User>();
			_context.Stub(a => a.SaveChangesAsync()).Return(Task.FromResult(0));
			_configuration = MockRepository.GenerateStub<IAppConfiguration>();
			_testRoleName = "test Role";
			TestUser = GetUser();
			_context.User.Add(TestUser);

			_sut = new UserStore<User>(_context, _configuration);
		}

		[Test]
		[TestCase("3099-12-12", false)]
		[TestCase("1099-12-12", true)]
		[TestCase(null, false)]
		public async Task Given_UserExists_When_FindAndCheckLogonAsync_Then_UserIsLoggedOn(DateTime? passwordExpiryDate, bool expectedMustChangePassword)
		{
			// Arrange
			TestUser.PasswordExpiryDateUtc = passwordExpiryDate;

			// Act
			var result = await _sut.TryLogOnAsync(TestUser.UserName, "xsHDjxshdjkKK917&");

			// Assert
			Assert.IsTrue(result.Success);
			Assert.AreEqual(0, TestUser.FailedLogonAttemptCount);
			Assert.AreEqual(TestUser.UserName, result.UserName);
			Assert.That(result.MustChangePassword, Is.EqualTo(expectedMustChangePassword));
			_context.AssertWasCalled(a => a.SaveChangesAsync());
		}

		[Test]
		public async Task Given_UserIsInAListOfCommonlyUsedNames_When_FindAndCheckLogonAsync_Then_EmptyResultWithFlagSet()
		{
			// Arrange

			// Act
			var result = await _sut.TryLogOnAsync("test", "AnythingYouWant");

			// Assert
			Assert.IsFalse(result.Success);
			Assert.IsTrue(result.IsCommonUserName, "Common User name flag not set");
			_context.AssertWasNotCalled(a => a.SaveChangesAsync());
		}

		[Test]
		public async Task Given_UserIsInAListOfCommonlyUsedNamesAndTheUserNameIsActuallyInUse_When_FindAndCheckLogonAsync_Then_UserIsNotLoggedOn()
		{
			// Arrange
			_context.User.Add(new User() { UserName = "test" });

			// Act
			var result = await _sut.TryLogOnAsync("test", "ThisIsAnIncorrectPassword");

			// Assert
			Assert.IsFalse(result.Success);
			Assert.IsFalse(result.IsCommonUserName, "Common User name flag was set");
			_context.AssertWasNotCalled(a => a.SaveChangesAsync());
		}

		[Test]
		public async Task Given_UserExistsButPasswordIncorrect_When_FindAndCheckLogonAsync_Then_UserIsNotLoggedOn()
		{
			// Arrange

			// Act
			var result = await _sut.TryLogOnAsync(TestUser.UserName, "rubbish");

			// Assert
			Assert.IsFalse(result.Success);
			Assert.AreEqual(2, TestUser.FailedLogonAttemptCount);
			Assert.AreNotEqual(TestUser.UserName, result.UserName);
			Assert.IsTrue(TestUser.UserLogs.Any(b => b.Description.Contains("Failed Logon")));
			Assert.IsFalse(TestUser.PasswordLastChangedDateUtc > DateTime.UtcNow.AddMinutes(-5));
			_context.AssertWasCalled(a => a.SaveChangesAsync());

		}

		[Test]
		public async Task Given_UserExists_When_CreateIdentityAsync_Then_ReturnsClaims()
		{
			// Arrange
			var authenticationType = "Forms";

			// Act
			var result = await _sut.CreateIdentityAsync(TestUser, authenticationType);

			// Assert
			Assert.AreEqual(4, result.Claims.Count());
			var nameIdentifier = result.Claims.FirstOrDefault(a => a.Type == ClaimTypes.NameIdentifier);
			Assert.IsNotNull(nameIdentifier);
			Assert.AreEqual(TestUser.Id.ToString(), nameIdentifier.Value);
			var roleName = result.Claims.FirstOrDefault(a => a.Type == ClaimTypes.Role);
			Assert.IsNotNull(roleName);
			Assert.AreEqual(_testRoleName, roleName.Value);
		}

		[Test]
		public async Task Given_ValidResetToken_When_ChangePasswordFromTokenAsync_Then_Success()
		{

			// Arrange
			var passwordResetToken = "jafueokvnsdsjrogdsjvnasqpzlmveyij";
			var newPassword = "pqlzmwoskeidjS1";
			TestUser.PasswordResetToken = passwordResetToken;
			TestUser.PasswordResetExpiryDateUtc = DateTime.UtcNow.AddMinutes(10);
			TestUser.PasswordHash = null;
			TestUser.PasswordSalt = null;

			// Act
			var result = await _sut.ChangePasswordFromTokenAsync(TestUser.Id, passwordResetToken, newPassword);

			// Assert
			Assert.AreEqual(0, result.Errors.Count());
			_context.AssertWasCalled(a => a.SaveChangesAsync());
			Assert.IsNull(TestUser.PasswordResetExpiryDateUtc);
			Assert.IsNull(TestUser.PasswordResetToken);
			Assert.AreEqual(0, TestUser.FailedLogonAttemptCount);
			Assert.AreEqual(1, TestUser.UserLogs.Count);
			Assert.IsTrue(TestUser.UserLogs.Any(a => a.Description.Contains("Password changed")));
			Assert.That(TestUser.PreviousPasswords.Count, Is.EqualTo(1));
			Assert.IsNotNull(TestUser.PasswordHash);
			Assert.IsNotNull(TestUser.PasswordSalt);
			Assert.IsTrue(TestUser.PasswordLastChangedDateUtc > DateTime.UtcNow.AddMinutes(-5));

		}

		[Test]
		public async Task Given_OldPasswordCorrect_When_ChangePasswordAsync_Then_Success()
		{

			// Arrange
			var oldPasswordHash = TestUser.PasswordHash;
			var oldSalt = TestUser.PasswordSalt;
			var oldPassword = "xsHDjxshdjkKK917&";
			var newPassword = "Anything12345";

			// Act
			var result = await _sut.ChangePasswordAsync(TestUser.Id, oldPassword, newPassword);

			// Assert
			Assert.AreEqual(0, result);
			_context.AssertWasCalled(a => a.SaveChanges());
			Assert.AreEqual(1, TestUser.UserLogs.Count);
			Assert.IsTrue(TestUser.UserLogs.Any(a => a.Description.Contains("Password changed")));
			Assert.That(TestUser.PreviousPasswords.Count, Is.EqualTo(1));
			Assert.AreNotEqual(oldPasswordHash, TestUser.PasswordHash);
			Assert.AreNotEqual(oldSalt, TestUser.PasswordSalt);
			Assert.IsTrue(TestUser.PasswordLastChangedDateUtc > DateTime.UtcNow.AddMinutes(-5));

		}

		[Test]
		public async Task Given_MaxPreviousPasswordChangesReached_When_ChangePasswordAsync_Then_PasswordHistoryListDoesNotGrow()
		{

			// Arrange
			var oldPasswordHash = TestUser.PasswordHash;
			var oldSalt = TestUser.PasswordSalt;
			var oldPassword = "xsHDjxshdjkKK917&";
			var newPassword = "Anything12345";
			_configuration.MaxNumberOfPreviousPasswords = 4;
			var passwordLastChangedDate = DateTime.UtcNow;
			TestUser.PasswordLastChangedDateUtc = passwordLastChangedDate;
			TestUser.PreviousPasswords = new List<PreviousPassword>
			{
				new PreviousPassword { ActiveFromDateUtc = DateTime.UtcNow.AddDays(-1)},
				new PreviousPassword { ActiveFromDateUtc = DateTime.UtcNow.AddDays(-2)},
				new PreviousPassword { ActiveFromDateUtc = DateTime.UtcNow.AddDays(-3)},
				new PreviousPassword { ActiveFromDateUtc = DateTime.UtcNow.AddDays(-4), Salt = "To be removed"}
			};

			// Act
			var result = await _sut.ChangePasswordAsync(TestUser.Id, oldPassword, newPassword);

			// Assert
			Assert.AreEqual(0, result);
			_context.AssertWasCalled(a => a.SaveChanges());
			_context.AssertWasCalled(a => a.SetDeleted(Arg<PreviousPassword>.Matches(b => b.Salt == "To be removed")));
			Assert.AreNotEqual(oldPasswordHash, TestUser.PasswordHash);
			Assert.AreNotEqual(oldSalt, TestUser.PasswordSalt);
			var mostRecentPasswordChange = TestUser.PreviousPasswords.OrderByDescending(a => a.ActiveFromDateUtc).Take(1).Single();
			Assert.That(mostRecentPasswordChange.ActiveFromDateUtc, Is.EqualTo(passwordLastChangedDate));

		}

		[Test]
		public async Task Given_ValidData_When_ResetPassword_Then_Success()
		{

			// Arrange
			var actioningUserName = "bob";
			var newPassword = "ashkjdfEYURER124";
			var oldHash = TestUser.PasswordHash;
			var oldSalt = TestUser.PasswordSalt;

			// Act
			var result = await _sut.ResetPasswordAsync(TestUser.Id, newPassword, actioningUserName);

			// Assert
			Assert.AreEqual(0, result.Errors.Count());
			_context.AssertWasCalled(a => a.SaveChangesAsync());
			Assert.IsNull(TestUser.PasswordResetExpiryDateUtc);
			Assert.IsNull(TestUser.PasswordResetToken);
			Assert.AreEqual(0, TestUser.FailedLogonAttemptCount);
			Assert.AreEqual(1, TestUser.UserLogs.Count);
			Assert.IsTrue(TestUser.UserLogs.Any(a => a.Description.Contains("User had password reset sent out via email by bob")));
			Assert.AreNotEqual(TestUser.PasswordHash, oldHash);
			Assert.AreNotEqual(TestUser.PasswordSalt, oldSalt);
			Assert.AreEqual(1, TestUser.PreviousPasswords.Count);
			var previousPassword = TestUser.PreviousPasswords.First();
			Assert.That(previousPassword.Hash, Is.EqualTo(oldHash));
			Assert.That(previousPassword.Salt, Is.EqualTo(oldSalt));
			Assert.IsTrue(TestUser.PasswordLastChangedDateUtc > DateTime.UtcNow.AddMinutes(-5));
			Assert.That(TestUser.PasswordExpiryDateUtc, Is.LessThanOrEqualTo(DateTime.UtcNow), "User should be required to change password on next logon");

		}

		protected User GetUser()
		{

			return new User
			{
				Id = 7,
				PasswordHash = "BpC/5HcMA4pnktXCPGY6HeNY9fPPk24JvvN2YyR3JFcd2j6Nen0sZHrf1mucLSMuuxp3CfHWaPIct8jp11YYyUXgihhS+9VA4OUJVz7Ak1uvuT6M+qItK1+tdlsihrpk3PkiuWafte0lcStImz2sCJroxtoGzOxOGSnpFehPIgd5TZBvmI3Crphdxq/dJhRwHIVQrnrXzwA+Aapy3bcXvutFmxS9F3/31BU4F5dJcYWHu+KbPydUlFl7RnM6A7DsnNKVcoDnk1CJZiJCz7WWNos+m+iv0CBE4ENDuP20sLW6x51S/ktcz3mdbn9wT38JM5CoLbS1UdVxdYC+Dkv+kQ==",
				PasswordSalt = "K6GuRmwFwOupdDba+C1FqKYwyBuxCykesgiY+fmCVBNVwr7qafuQ7oj9HrgM3LTXMB9LtOkWc4Z7VzB3AjobRk4trmwy7yOyvXnZj9XcBom2s5htHz8tiYhgsV/fHLlNfbeFseOXMLqUN4AFf+/+07j2NiaQK+qLFDSOAFpvsfB6kHF5vk2JgJb8qQSaLAW5FrDFn4f6cqYQJg8H127xPm8WYJiU94sw4dd13XxneKUbzez3yikR20U7rfQMRFKUr2a14vApH4kGsg3F89n8B+w2A/Orz/iarA9uzATag0t2r5MPnQeG58odK5uOPTbWz1mka+gXVcY620SAdyo07Q==", // xsHDjxshdjkKK917&
				Enabled = true,
				Approved = true,
				EmailVerified = true,
				FailedLogonAttemptCount = 1,
				FirstName = "Bobby",
				UserName = "testuser@micro.com",
				EmailConfirmationToken = "test1",
				SecurityQuestionLookupItemId = 1,
				SecurityQuestionLookupItem = new LookupItem { Id = 1, Description = "test question" },
				UserRoles = new List<UserRole>
				{
					new UserRole { Id = 1, Role = new Role { Id = 7, Description = _testRoleName }   }
				}
			};
		}

	}
}
