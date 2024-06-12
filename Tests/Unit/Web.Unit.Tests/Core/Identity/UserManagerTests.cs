using Microsoft.AspNet.Identity;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin.Security;
using SecurityEssentials.Core;
using SecurityEssentials.Core.Constants;
using SecurityEssentials.Core.Identity;
using SecurityEssentials.Model;
using SecurityEssentials.Unit.Tests.TestDbSet;

namespace SecurityEssentials.Unit.Tests.Core.Identity
{

	[TestFixture]
	public class UserManagerTests
	{

		AppUserManager _sut;
		IAppConfiguration _configuration;
		IEncryption _encryption;
		ISeContext _context;
		IPwnedPasswordValidator _pwnedPasswordValidator;
		private IServices _services;
		IAppUserStore<User> _userStore;
		List<string> _bannedWords;

		[SetUp]
		public void Setup()
		{
			_context = MockRepository.GenerateStub<ISeContext>();
			_context.LookupItem = new TestDbSet<LookupItem>
			{
				new LookupItem {LookupTypeId = Consts.LookupTypeId.BadPassword, Description = "Password1"},
				new LookupItem {LookupTypeId = Consts.LookupTypeId.BadPassword, Description = "LetMeIn123"},
				new LookupItem {LookupTypeId = Consts.LookupTypeId.SecurityQuestion, Id = 142}
			};
			_context.User = new TestDbSet<User>();
			_context.UserLog = new TestDbSet<UserLog>();
			_configuration = MockRepository.GenerateStub<IAppConfiguration>();
			_encryption = MockRepository.GenerateMock<IEncryption>();
			_services = MockRepository.GenerateMock<IServices>();
			_userStore = MockRepository.GenerateMock<IAppUserStore<User>>();
			_bannedWords = new List<string> { "First Name", "SurName", "My Town", "My PostCode" };
            _pwnedPasswordValidator = MockRepository.GenerateMock<IPwnedPasswordValidator>();
			_sut = new AppUserManager(_configuration, _context, _encryption, _pwnedPasswordValidator, _services, _userStore);
		}

		[TearDown]
		public void MyTestCleanup()
		{
			_encryption.VerifyAllExpectations();
            _pwnedPasswordValidator.VerifyAllExpectations();
			_services.VerifyAllExpectations();
			_userStore.VerifyAllExpectations();
		}

		[Test]
		public async Task Given_ValidDetails_When_ChangePassword_Then_PasswordChanged()
		{
			// Arrange
			var userId = 1;
			var oldPassword = "oldPassword910";
			var newPassword = "newPassword345";
			_userStore.Expect(a => a.ChangePasswordAsync(userId, oldPassword, newPassword)).Return(Task.FromResult(1));
			string decryptedSecurityAnswer = "blah";
			_encryption.Expect(a => a.Decrypt(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<int>.Is.Anything, Arg<string>.Is.Anything, out Arg<string>.Out(decryptedSecurityAnswer).Dummy)).Return(true);
			_userStore.Expect(a => a.FindByIdAsync(userId)).Return(Task.FromResult(new User { FirstName = "Bob", LastName = "Joseph", SecurityAnswer = "encryptedblah" }));
            _pwnedPasswordValidator.Expect(a => a.Validate(Arg<string>.Is.Anything)).Return(Task.FromResult(new PwnedPasswordDto{ IsPwned = false, Count = 0 }));

			// Act
			var result = await _sut.ChangePasswordAsync(userId, oldPassword, newPassword);

			// Assert
			Assert.That(result.Succeeded, "Expected to Succeed but result was failure");

		}

		[Test]
		[TestCase("Bob")]
		[TestCase("bob")]
		[TestCase("Joseph")]
		[TestCase("joseph")]
		[TestCase("blah")]
		[TestCase("BLAH")]
		public async Task Given_PersonalInformationUsedInPassword_When_ChangePassword_Then_PasswordChangeRejected(string wordReused)
		{
			// Arrange
			var userId = 1;
			var oldPassword = "oldPassword910";
			var newPassword = $"{wordReused}345";
			string decryptedSecurityAnswer = "blah";
			_encryption.Expect(a => a.Decrypt(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<int>.Is.Anything, Arg<string>.Is.Anything, out Arg<string>.Out(decryptedSecurityAnswer).Dummy)).Return(true);
			_userStore.Expect(a => a.FindByIdAsync(userId)).Return(Task.FromResult(new User { FirstName = "Bob", LastName = "Joseph", SecurityAnswer = "encryptedblah" }));

			// Act
			var result = await _sut.ChangePasswordAsync(userId, oldPassword, newPassword);

			// Assert
			Assert.That(result.Succeeded, Is.False);
			_userStore.AssertWasNotCalled(a => a.ChangePasswordAsync(userId, oldPassword, newPassword));
		}

		[Test]
		public async Task Given_ValidDetails_When_ChangePasswordFromToken_Then_PasswordChanged()
		{
			// Arrange
			var userId = 1;
			var token = "asbcefghijklmnop";
			var newPassword = "newPassword345";
			string decryptedSecurityAnswer = "blah";
			_encryption.Expect(a => a.Decrypt(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<int>.Is.Anything, Arg<string>.Is.Anything, out Arg<string>.Out(decryptedSecurityAnswer).Dummy)).Return(true);
			_userStore.Expect(a => a.FindByIdAsync(userId)).Return(Task.FromResult(new User { FirstName = "Bob", LastName = "Joseph", SecurityAnswer = "encryptedblah" }));
			_userStore.Expect(a => a.ChangePasswordFromTokenAsync(userId, token, newPassword)).Return(Task.FromResult(new IdentityResult()));
            _pwnedPasswordValidator.Expect(a => a.Validate(Arg<string>.Is.Anything)).Return(Task.FromResult(new PwnedPasswordDto{ IsPwned = false, Count = 0 }));

			// Act
			var result = await _sut.ChangePasswordFromTokenAsync(userId, token, newPassword);

			// Assert
			Assert.That(result.Succeeded, "Expected to Succeed but result was failure");

		}

		[Test]
		[TestCase("Bob")]
		[TestCase("bob")]
		[TestCase("Joseph")]
		[TestCase("joseph")]
		[TestCase("blah")]
		[TestCase("BLAH")]
		public async Task Given_PersonalInformationUsedInPassword_When_ChangePasswordFromToken_Then_PasswordChangeRejected(string wordReused)
		{
			// Arrange
			var userId = 1;
			var newPassword = $"{wordReused}345";
			var token = "asbcefghijklmnop";
			string decryptedSecurityAnswer = "blah";
			_encryption.Expect(a => a.Decrypt(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<int>.Is.Anything, Arg<string>.Is.Anything, out Arg<string>.Out(decryptedSecurityAnswer).Dummy)).Return(true);
			_userStore.Expect(a => a.FindByIdAsync(userId)).Return(Task.FromResult(new User { FirstName = "Bob", LastName = "Joseph", SecurityAnswer = "encryptedblah" }));

			// Act
			var result = await _sut.ChangePasswordFromTokenAsync(userId, token, newPassword);

			// Assert
			Assert.That(result.Succeeded, Is.False);
			_userStore.AssertWasNotCalled(a => a.ChangePasswordFromTokenAsync(userId, token, newPassword));

		}

		[Test]
		public async Task Given_ValidDetails_When_ResetPasswordAsync_Then_PasswordReset()
		{
			// Arrange
			var userId = 1;
			var actioningUserName = "bob";
			_userStore.Expect(a => a.FindByIdAsync(userId)).Return(Task.FromResult(new User() { FirstName = "Bob", LastName = "Joseph", SecurityAnswer = "blah" }));
			_userStore.Expect(a => a.ResetPasswordAsync(Arg<int>.Is.Equal(userId), Arg<string>.Is.Anything, Arg<string>.Is.Equal(actioningUserName))).Return(Task.FromResult(IdentityResult.Success));
			_services.Expect(a => a.SendEmail(Arg<string>.Is.Anything, Arg<List<string>>.Is.Anything, Arg<List<string>>.Is.Anything, Arg<List<string>>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<bool>.Is.Anything)).Return(true);
            _pwnedPasswordValidator.Expect(a => a.Validate(Arg<string>.Is.Anything)).Return(Task.FromResult(new PwnedPasswordDto{ IsPwned = false, Count = 0 }));

			// Act
			var result = await _sut.ResetPasswordAsync(userId, actioningUserName);

			// Assert
			Assert.That(result.Succeeded, "Expected to Succeed but result was failure");

		}

		[Test]
		public async Task Given_BogusSecurityQuestion_When_CreateUser_Then_UserCreatedFailure()
		{
			var userName = "bob@bob.net";
			_userStore.Expect(a => a.FindByNameAsync(userName)).Return(Task.FromResult<User>(null));

			// Act
			var result = await _sut.CreateAsync(userName, "bob", "the bod", "Secure1HJ", "Secure1HJ", 143, "Jo was my mother");

			// Assert
			Assert.That(result.Succeeded, Is.False);
			Assert.That(result.Errors.Contains("Illegal security question"));
			_userStore.AssertWasNotCalled(u => u.CreateAsync(Arg<User>.Is.Anything));

		}

		[Test]
		public async Task Given_PasswordInvalid_When_CreateUser_Then_UserCreatedFailure()
		{
			var userName = "bob@bob.net";
			_userStore.Expect(a => a.FindByNameAsync(userName)).Return(Task.FromResult<User>(null));

			// Act
			var result = await _sut.CreateAsync(userName, "bob", "the bod", "insecure", "insecure", 142, "Jo was my mother");

			// Assert
			Assert.That(result.Succeeded, Is.False);
			Assert.That(result.Errors.Contains(Consts.UserManagerMessages.PasswordValidityMessage));
			_userStore.AssertWasNotCalled(u => u.CreateAsync(Arg<User>.Is.Anything));

		}


		[Test]
		public async Task Given_ValidDetails_When_CreateUser_Then_UserCreatedSuccess()
		{
			var userName = "bob@bob.net";
			_userStore.Expect(a => a.FindByNameAsync(userName)).Return(Task.FromResult<User>(null));
			_userStore.Expect(a => a.CreateAsync(Arg<User>.Is.Anything)).Return(Task.FromResult(0));
            _pwnedPasswordValidator.Expect(a => a.Validate(Arg<string>.Is.Anything)).Return(Task.FromResult(new PwnedPasswordDto{ IsPwned = false, Count = 0 }));

			// Act
			var result = await _sut.CreateAsync(userName, "bob", "the bod", "Secure1HJ", "Secure1HJ", 142, "Jo was my mother");

			// Assert
			Assert.That(result.Succeeded);
			_userStore.AssertWasCalled(u => u.CreateAsync(Arg<User>.Matches(c =>
				!string.IsNullOrEmpty(c.EmailConfirmationToken) &&
				c.Approved == _configuration.AccountManagementRegisterAutoApprove &&
				c.EmailVerified == false &&
				c.Enabled &&
				c.FirstName == "bob" &&
				c.LastName == "the bod" &&
				c.PasswordLastChangedDateUtc > DateTime.UtcNow.AddMinutes(-5) &&
				!string.IsNullOrEmpty(c.PasswordHash) &&
				!string.IsNullOrEmpty(c.PasswordSalt) &&
				c.SecurityQuestionLookupItemId == 142 &&
				c.SecurityAnswer != "Jo was my mother" &&
				c.UserLogs.Any(a => a.Description == "Account Created")
			)));

		}

		[Test]
		public async Task When_LogonAsync_Then_LogsUpdated()
		{
			// Arrange
			var username = "Test User";
			var isPersistent = true;
			var testUser = new User { Id = 4 };
			_userStore.Expect(a => a.FindByNameAsync(username)).Return(Task.FromResult(testUser));
			var authenticationManager = MockRepository.GenerateMock<IAuthenticationManager>();
			_sut.AuthenticationManager = authenticationManager;
			_userStore.Expect(a => a.CreateIdentityAsync(Arg<User>.Matches(b => b.Id == testUser.Id), Arg<string>.Is.Equal(DefaultAuthenticationTypes.ApplicationCookie))).Return(Task.FromResult(new ClaimsIdentity()));
			_userStore.Expect(a => a.UpdateAsync(Arg<User>.Matches(b => b.Id == 4))).Return(Task.FromResult(testUser));
			authenticationManager.Expect(a => a.SignOut(Arg<string[]>.Is.Equal(
				new[] { DefaultAuthenticationTypes.ExternalCookie })));
			authenticationManager.Expect(a => a.SignIn(Arg<AuthenticationProperties>.Is.Anything, Arg<ClaimsIdentity>.Is.Anything));

			// Act
			var result = await _sut.LogOnAsync(username, isPersistent);

			// Assert
			Assert.That(testUser.UserLogs.Any(a => a.Description.Contains("Logged On")));
			Assert.That(result, Is.EqualTo(testUser.Id));
			authenticationManager.VerifyAllExpectations();

		}

		[Test]
		public void When_SignOut_Then_LogsUpdated()
		{
			// Arrange
			var authenticationManager = MockRepository.GenerateMock<IAuthenticationManager>();
			_sut.AuthenticationManager = authenticationManager;
			authenticationManager.Expect(a => a.User.Identity.Name).Return("Test User");
			authenticationManager.Expect(a => a.SignOut(Arg<string[]>.Is.Equal(
				new[] { DefaultAuthenticationTypes.ApplicationCookie })));
			var user = new User { UserName = "Test User" };
			_context.User.Add(user);

			// Act
			_sut.SignOut();

			// Assert

			Assert.That(user.UserLogs.Any(a => a.Description.Contains("Logged Off")));
			_context.AssertWasCalled(a => a.SaveChanges());
			authenticationManager.VerifyAllExpectations();
		}

		[Test]
		public async Task Given_ValidPassword_When_ValidatePassword_Then_Success()
		{
			// Arrange
			var password = "MyNewPassword1";
            _pwnedPasswordValidator.Expect(a => a.Validate(Arg<string>.Is.Anything)).Return(Task.FromResult(new PwnedPasswordDto{ IsPwned = false, Count = 0 }));

			// Act
			var result = await _sut.ValidatePassword(new User(), password, _bannedWords);

			// Assert
			Assert.That(result.Succeeded);
			Assert.That(0, Is.EqualTo(result.Errors.Count()));

		}

		[Test]
		[TestCase(HashStrategyKind.Pbkdf210001Iterations)]
		[TestCase(HashStrategyKind.Argon2WorkCost)]
		public async Task Given_UserIsReusingAPassword_When_ValidatePassword_Then_ErrorReturned(HashStrategyKind hashStrategy)
		{

			// Arrange
			var password = "MyNewPassword1";
			var previousPassword = new SecuredPassword(password, hashStrategy);
			_configuration.MaxNumberOfPreviousPasswords = 4;
			var user = new User
			{
				PreviousPasswords = new List<PreviousPassword>
				{
					new PreviousPassword { Hash = Convert.ToBase64String(previousPassword.Hash), Salt = Convert.ToBase64String(previousPassword.Salt), HashStrategy = previousPassword.HashStrategy}
				}
			};

			// Act
			var result = await _sut.ValidatePassword(user, password, _bannedWords);

			// Assert
			Assert.That(result.Succeeded, Is.False);
			Assert.That(result.Errors.Count(), Is.EqualTo(1));
			Assert.That(result.Errors.Single(), Is.EqualTo("You cannot use any of your 4 previous passwords"));

		}

		[Test]
		public async Task Given_UserIsReusingCurrentPassword_When_ValidatePassword_Then_ErrorReturned()
		{
			// Arrange
			var password = "MyNewPassword2";
			var currentPassword = new SecuredPassword(password, HashStrategyKind.Pbkdf210001Iterations);
			_configuration.MaxNumberOfPreviousPasswords = 4;
			var user = new User
			{
				PasswordHash = Convert.ToBase64String(currentPassword.Hash),
				PasswordSalt = Convert.ToBase64String(currentPassword.Salt),
				HashStrategy = currentPassword.HashStrategy
			};

			// Act
			var result = await _sut.ValidatePassword(user, password, _bannedWords);

			// Assert
			Assert.That(result.Succeeded, Is.False);
			Assert.That(result.Errors.Count(), Is.EqualTo(1));
			Assert.That(result.Errors.Single(), Is.EqualTo("You cannot use any of your 4 previous passwords"));

		}

		[Test]
		public async Task Given_PasswordContainsSpecialCharacters_When_ValidatePassword_Then_Succeeds()
		{
			// Arrange
			var password = "*&^%$£\"!aAb2";
			_pwnedPasswordValidator.Expect(a => a.Validate(Arg<string>.Is.Anything)).Return(Task.FromResult(new PwnedPasswordDto { IsPwned = false, Count = 0 }));

			// Act
			var result = await _sut.ValidatePassword(new User(), password, _bannedWords);

			// Assert
			Assert.That(result.Succeeded);
			Assert.That(0, Is.EqualTo(result.Errors.Count()));
		}
        [Test]
        public async Task Given_PasswordIsPwned_When_ValidatePassword_Then_Succeeds()
        {
            // Arrange
            var password = "*&^%$£\"!aAb2";
            _pwnedPasswordValidator.Expect(a => a.Validate(Arg<string>.Is.Anything)).Return(Task.FromResult(new PwnedPasswordDto { IsPwned = true, Count = 1 }));

            // Act
            var result = await _sut.ValidatePassword(new User(), password, _bannedWords);

            // Assert
            AssertValidationResultFailed(result, "Your password has previously been found in a data breach, please choose another");
        }
		[Test]
		public async Task Given_PasswordContainsUserInformation_When_ValidatePassword_Then_Fails()
		{
			// Arrange
			var password = "First Name1";

			// Act
			var result = await _sut.ValidatePassword(new User(), password, _bannedWords);

			// Assert
			AssertValidationResultFailed(result, "personal information");
		}

		[Test]
		public async Task Given_PasswordContainsConsecutivelyRepeatedCharacters_When_ValidatePassword_Then_Fails()
		{
			// Arrange
			var password = "L7s8xvdooo123O";

			// Act
			var result = await _sut.ValidatePassword(new User(), password, _bannedWords);

			// Assert
			AssertValidationResultFailed(result, "repeat the same character or digit more than 3 times consecutively");
		}

		[Test]
		public async Task Given_PasswordIsKnownBadPassword_When_ValidatePassword_Then_Fails()
		{
			// Arrange
			var password = "LetMeIn123";

			// Act
			var result = await _sut.ValidatePassword(new User(), password, _bannedWords);

			// Assert
			AssertValidationResultFailed(result, "password is on a list of easy to guess passwords");
		}

		[Test]
		public async Task Given_PasswordDoesNotMeetMinimumComplexity_When_ValidatePassword_Then_Fails()
		{
			// Arrange
			var password = "aidhjthejfhgkfhds";

			// Act
			var result = await _sut.ValidatePassword(new User(), password, _bannedWords);

			// Assert
			AssertValidationResultFailed(result, "password must consist of 8 characters, digits or special characters and must contain at least 1 uppercase, 1 lowercase and 1 numeric value");
		}

		[Test]
		public async Task When_GenerateSecurePassword_Then_NewPasswordIsReturned()
		{
			// Arrange
            _pwnedPasswordValidator.Expect(a => a.Validate(Arg<string>.Is.Anything)).Return(Task.FromResult(new PwnedPasswordDto { IsPwned = false, Count = 0 }));

			// Act
			var result = await _sut.GenerateSecurePassword(new User());

			// Assert
			Assert.That(result.Length, Is.GreaterThan(9));
		}

		private void AssertValidationResultFailed(SeIdentityResult result, string errorMessageContains)
		{
			Assert.That(result.Succeeded, Is.False);
			Assert.That(1, Is.EqualTo(result.Errors.Count()));
			if (!string.IsNullOrEmpty(errorMessageContains))
			{
				Assert.That(result.Errors.All(a => a.Contains(errorMessageContains)), Is.True);
			}
		}

	}
}
