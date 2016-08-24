using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecurityEssentials.Core;
using SecurityEssentials.Core.Identity;
using SecurityEssentials.Model;
using System.Collections.Generic;
using Rhino.Mocks;
using SecurityEssentials.Unit.Tests.TestDbSet;

namespace SecurityEssentials.Unit.Tests.Core.Identity
{

    [TestClass]
    public class UserManagerTests
    {

        IUserManager _sut;
        IAppConfiguration _configuration;
		IEncryption _encryption;
        ISEContext _context;
        UserStore<User> _userStore;
		List<string> _bannedWords;
       
        [TestInitialize]
        public void Setup()
        {
			_context = MockRepository.GenerateStub<ISEContext>();
			_context.LookupItem = new TestDbSet<LookupItem>();
			_context.LookupItem.Add(new LookupItem() { LookupTypeId = CONSTS.LookupTypeId.BadPassword, Description = "Password1" });
			_context.LookupItem.Add(new LookupItem() { LookupTypeId = CONSTS.LookupTypeId.BadPassword, Description = "LetMeIn123" });
			_configuration = MockRepository.GenerateStub<IAppConfiguration>();
			_encryption = MockRepository.GenerateMock<IEncryption>();
			_userStore = new UserStore<User>(_context, _configuration);
			_bannedWords = new List<string>() { "First Name", "SurName", "My Town", "My PostCode" };
            _sut = new AppUserManager(_configuration, _context, _encryption, _userStore);
        }

        [TestCleanup()]
        public void MyTestCleanup()
        {
			_encryption.VerifyAllExpectations();
        }
        
		[TestMethod]
		public void GIVEN_ValidPassword_WHEN_ValidatePassword_THEN_Success()
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
		public void GIVEN_PasswordContainsUserInformation_WHEN_ValidatePassword_THEN_Fails()
		{

			// Arrange
			var password = "First Name1";

			// Act
			var result = _sut.ValidatePassword(password, _bannedWords);

			// Assert
			AssertValidationResultFailed(result);

		}

		[TestMethod]
		public void GIVEN_PasswordIsKnownBadPassword_WHEN_ValidatePassword_THEN_Fails()
		{

			// Arrange
			var password = "LetMeIn123";

			// Act
			var result = _sut.ValidatePassword(password, _bannedWords);

			// Assert
			AssertValidationResultFailed(result);
		}

		[TestMethod]
		public void GIVEN_PasswordDoesNotMeetMinimumComplexity_WHEN_ValidatePassword_THEN_Fails()
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
