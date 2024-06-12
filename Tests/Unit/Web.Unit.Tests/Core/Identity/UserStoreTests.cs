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
			Assert.That(result.Success);
			Assert.That(0, Is.EqualTo(TestUser.FailedLogonAttemptCount));
			Assert.That(TestUser.UserName, Is.EqualTo(result.UserName));
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
			Assert.That(result.Success, Is.False);
			Assert.That(result.IsCommonUserName, "Common User name flag not set");
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
			Assert.That(result.Success, Is.False);
			Assert.That(result.IsCommonUserName, Is.False, "Common User name flag was set");
			_context.AssertWasNotCalled(a => a.SaveChangesAsync());
		}

		[Test]
		public async Task Given_UserExistsButPasswordIncorrect_When_FindAndCheckLogonAsync_Then_UserIsNotLoggedOn()
		{
			// Arrange

			// Act
			var result = await _sut.TryLogOnAsync(TestUser.UserName, "rubbish");

			// Assert
			Assert.That(result.Success, Is.False);
			Assert.That(2, Is.EqualTo(TestUser.FailedLogonAttemptCount));
			Assert.That(TestUser.UserName, Is.Not.EqualTo(result.UserName));
			Assert.That(TestUser.UserLogs.Any(b => b.Description.Contains("Failed Logon")));
			Assert.That(TestUser.PasswordLastChangedDateUtc > DateTime.UtcNow.AddMinutes(-5), Is.False);
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
			Assert.That(4, Is.EqualTo(result.Claims.Count()));
			var nameIdentifier = result.Claims.FirstOrDefault(a => a.Type == ClaimTypes.NameIdentifier);
			Assert.That(nameIdentifier, Is.Not.Null);
			Assert.That(TestUser.Id.ToString(), Is.EqualTo(nameIdentifier.Value));
			var roleName = result.Claims.FirstOrDefault(a => a.Type == ClaimTypes.Role);
			Assert.That(roleName, Is.Not.Null);
			Assert.That(_testRoleName, Is.EqualTo(roleName.Value));
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
			Assert.That(0, Is.EqualTo(result.Errors.Count()));
			_context.AssertWasCalled(a => a.SaveChangesAsync());
			Assert.That(TestUser.PasswordResetExpiryDateUtc, Is.Null);
			Assert.That(TestUser.PasswordResetToken, Is.Null);
			Assert.That(0, Is.EqualTo(TestUser.FailedLogonAttemptCount));
			Assert.That(1, Is.EqualTo(TestUser.UserLogs.Count));
			Assert.That(TestUser.UserLogs.Any(a => a.Description.Contains("Password changed")));
			Assert.That(TestUser.PreviousPasswords.Count, Is.EqualTo(1));
			Assert.That(TestUser.PasswordHash, Is.Not.Null);
			Assert.That(TestUser.PasswordSalt, Is.Not.Null);
			Assert.That(TestUser.PasswordLastChangedDateUtc > DateTime.UtcNow.AddMinutes(-5));

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
			Assert.That(0, Is.EqualTo(result), "Password was invalid");
			_context.AssertWasCalled(a => a.SaveChanges());
			Assert.That(1, Is.EqualTo(TestUser.UserLogs.Count));
			Assert.That(TestUser.UserLogs.Any(a => a.Description.Contains("Password changed")));
			Assert.That(TestUser.PreviousPasswords.Count, Is.EqualTo(1));
			Assert.That(oldPasswordHash, Is.Not.EqualTo(TestUser.PasswordHash));
			Assert.That(oldSalt, Is.Not.EqualTo(TestUser.PasswordSalt));
			Assert.That(TestUser.PasswordLastChangedDateUtc > DateTime.UtcNow.AddMinutes(-5));

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
			Assert.That(0, Is.EqualTo(result));
			_context.AssertWasCalled(a => a.SaveChanges());
			_context.AssertWasCalled(a => a.SetDeleted(Arg<PreviousPassword>.Matches(b => b.Salt == "To be removed")));
			Assert.That(oldPasswordHash, Is.Not.EqualTo(TestUser.PasswordHash));
			Assert.That(oldSalt, Is.Not.EqualTo(TestUser.PasswordSalt));
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
			Assert.That(0, Is.EqualTo(result.Errors.Count()));
			_context.AssertWasCalled(a => a.SaveChangesAsync());
			Assert.That(TestUser.PasswordResetExpiryDateUtc, Is.Null);
			Assert.That(TestUser.PasswordResetToken, Is.Null);
			Assert.That(0, Is.EqualTo(TestUser.FailedLogonAttemptCount));
			Assert.That(1, Is.EqualTo(TestUser.UserLogs.Count));
			Assert.That(TestUser.UserLogs.Any(a => a.Description.Contains("User had password reset sent out via email by bob")));
			Assert.That(TestUser.PasswordHash, Is.Not.EqualTo(oldHash));
			Assert.That(TestUser.PasswordSalt, Is.Not.EqualTo(oldSalt));
			Assert.That(1, Is.EqualTo(TestUser.PreviousPasswords.Count));
			var previousPassword = TestUser.PreviousPasswords.First();
			Assert.That(previousPassword.Hash, Is.EqualTo(oldHash));
			Assert.That(previousPassword.Salt, Is.EqualTo(oldSalt));
			Assert.That(TestUser.PasswordLastChangedDateUtc > DateTime.UtcNow.AddMinutes(-5));
			Assert.That(TestUser.PasswordExpiryDateUtc, Is.LessThanOrEqualTo(DateTime.UtcNow), "User should be required to change password on next logon");

		}

		protected User GetUser()
		{

			return new User
			{
				Id = 7,
				PasswordHash = "Wt0p8sAmUDvxS+HmZ42imVg815KBGh0r/svNe6V+zvmvfmHshMhnX/792wUVSLp1F8jJ+HIGwS19zwM6LKpUEPJpZ9NlfzMsNPoKXeLz3VyC2vBEar0CGKzwcGQu3cn674L5WI/J8n0mZYKbZlbKuTBw8bqx7TLXXKZE0I2+hdAE4XkLvaL+1d3A1ILSwJnyYcIWvuhEtBc9BpiCFc7JmfKAS44WZry873Zj7qSB2cQ/EYfX+jVKHc0MAer0aSjFNRDnl1Kp5ZbwuIiTj2ROtbP0uKmvbTYwnPlTHmD+iTn9Go2Sg+mWRmrIYWdBCF9q5RLfJbf6fpdESSiQDcXwfw==",
				PasswordSalt = "JObSM3IqX1sTTCRd77tyzi/DYemkzH9y4DceDEqnF88kuYvs/oPkyLFiLsgyv7IGN2LWnkE2ZIClLpt+msg6VFZ5vdkvfBLUD9HrzL5YXu//mEwQQjA1wx1EJ8ZMtkWffo19qp6UAJF1elPQ0ihARfUxgqVAkB8w1BsXnX44/dQmCyRm5rmU4h9uACyWDFyjGblY1nt3h9BWNjUmL+5tXnN/cq1XJO4M2ivRRr8ZlQgZNh3DC2H86n9nZjKob1nft6E27+2oza7KwtVlq0GIielqtqzF974mQ+lQ30ViDvfbV9EduEOe7Yeicv/ZycwOwx1FINrq52Nb7bh83eDv6Q==", // xsHDjxshdjkKK917&
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
