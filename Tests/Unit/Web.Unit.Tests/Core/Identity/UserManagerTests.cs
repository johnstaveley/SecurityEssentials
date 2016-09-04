using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecurityEssentials.Core;
using SecurityEssentials.Core.Identity;
using SecurityEssentials.Model;
using System.Collections.Generic;
using Rhino.Mocks;
using SecurityEssentials.Unit.Tests.TestDbSet;
using System.Threading.Tasks;

namespace SecurityEssentials.Unit.Tests.Core.Identity
{

	[TestClass]
    public class UserManagerTests
    {

        AppUserManager _sut;
        IAppConfiguration _configuration;
		IEncryption _encryption;
        ISEContext _context;
        IAppUserStore<User> _userStore;
		List<string> _bannedWords;
       
        [TestInitialize]
        public void Setup()
        {
			_context = MockRepository.GenerateStub<ISEContext>();
			_context.User = new TestDbSet<User>();
			_context.LookupItem = new TestDbSet<LookupItem>();
			_context.LookupItem.Add(new LookupItem() { LookupTypeId = CONSTS.LookupTypeId.BadPassword, Description = "Password1" });
			_context.LookupItem.Add(new LookupItem() { LookupTypeId = CONSTS.LookupTypeId.BadPassword, Description = "LetMeIn123" });
			_configuration = MockRepository.GenerateStub<IAppConfiguration>();
			_encryption = MockRepository.GenerateMock<IEncryption>();
			_userStore = MockRepository.GenerateMock<IAppUserStore<User>>(); 
			_bannedWords = new List<string>() { "First Name", "SurName", "My Town", "My PostCode" };
            _sut = new AppUserManager(_configuration, _context, _encryption, _userStore);
        }

        [TestCleanup()]
        public void MyTestCleanup()
        {
			_encryption.VerifyAllExpectations();
			_userStore.VerifyAllExpectations();
        }

		[TestMethod]
		public async Task Given_ValidDetails_When_ChangePassword_Then_PasswordChanged()
		{
			// Arrange
			var userId = 1;
			var oldPassword = "oldPassword910";
			var newPassword = "newPassword345";
			_userStore.Expect(a => a.ChangePasswordAsync(userId, oldPassword, newPassword)).Return(Task.FromResult(0));
			_userStore.Expect(a => a.FindByIdAsync(userId)).Return(Task.FromResult(new User() { FirstName = "Bob", LastName = "Joseph", SecurityAnswer = "blah" }));

			// Act
			var result = await _sut.ChangePasswordAsync(userId, oldPassword, newPassword);

			// Assert
			Assert.IsTrue(result.Succeeded);

		}

		[TestMethod]
		public async Task Given_PersonalInformationUsed_When_ChangePassword_Then_PasswordNotChanged()
		{
			// Arrange
			var userId = 1;
			var oldPassword = "oldPassword910";
			var newPassword = "Joseph345";
			_userStore.Expect(a => a.FindByIdAsync(userId)).Return(Task.FromResult(new User() { FirstName = "Bob", LastName = "Joseph", SecurityAnswer = "blah" }));

			// Act
			var result = await _sut.ChangePasswordAsync(userId, oldPassword, newPassword);

			// Assert
			Assert.IsFalse(result.Succeeded);
			_userStore.AssertWasNotCalled(a => a.ChangePasswordAsync(userId, oldPassword, newPassword));

		}

		[TestMethod]
		public async Task Given_ValidDetails_When_CreateUser_Then_UserCreatedSuccess()
		{
			var userName = "bob@bob.net";
			_userStore.Expect(a => a.FindByNameAsync(userName)).Return(Task.FromResult<User>(null));
			_userStore.Expect(a => a.CreateAsync(Arg<User>.Is.Anything)).Return(Task.FromResult(0));

			// Act
			var result = await _sut.CreateAsync(userName, "bob", "the bod", "unsecure1H", "unsecure1H", 142, "Jo was my mother");

			// Assert
			Assert.IsTrue(result.Succeeded);
			_userStore.AssertWasCalled(u => u.CreateAsync(Arg<User>.Matches(c => 
				!string.IsNullOrEmpty(c.EmailConfirmationToken) &&
				c.Approved == _configuration.AccountManagementRegisterAutoApprove &&
				c.EmailVerified == false &&
				c.Enabled == true &&
				c.FirstName == "bob" &&
				c.LastName == "the bod" &&
				!string.IsNullOrEmpty(c.PasswordHash) &&
				!string.IsNullOrEmpty(c.Salt) &&
				c.SecurityQuestionLookupItemId == 142 &&
				c.SecurityAnswer != "Jo was my mother" &&
				c.UserLogs.Any(a => a.Description == "Account Created")
			)));
						
		}

		[TestMethod]
		public void Given_ValidPassword_When_ValidatePassword_Then_Success()
		{

			// Arrange
			var password = "MyNewPassword1";

			// Act
			var result = _sut.ValidatePassword(password, _bannedWords);

			// Assert
			Assert.IsTrue(result.Succeeded);
			Assert.AreEqual(0, result.Errors.Count());

		}

		[TestMethod]
		public void Given_PasswordContainsUserInformation_When_ValidatePassword_Then_Fails()
		{

			// Arrange
			var password = "First Name1";

			// Act
			var result = _sut.ValidatePassword(password, _bannedWords);

			// Assert
			AssertValidationResultFailed(result);

		}

		[TestMethod]
		public void Given_PasswordIsKnownBadPassword_When_ValidatePassword_Then_Fails()
		{

			// Arrange
			var password = "LetMeIn123";

			// Act
			var result = _sut.ValidatePassword(password, _bannedWords);

			// Assert
			AssertValidationResultFailed(result);
		}

		[TestMethod]
		public void Given_PasswordDoesNotMeetMinimumComplexity_When_ValidatePassword_Then_Fails()
		{

			// Arrange
			var password = "aidhjthejfhgkfhds";

			// Act
			var result = _sut.ValidatePassword(password, _bannedWords);

			// Assert
			AssertValidationResultFailed(result);
		}

		private void AssertValidationResultFailed(SEIdentityResult result)
		{
			Assert.IsFalse(result.Succeeded);
			Assert.AreEqual(1, result.Errors.Count());
		}

    }
}
