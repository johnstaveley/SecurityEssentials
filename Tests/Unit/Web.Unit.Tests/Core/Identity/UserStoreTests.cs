using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecurityEssentials.Core;
using SecurityEssentials.Core.Identity;
using SecurityEssentials.Model;
using System.Collections.Generic;
using Rhino.Mocks;
using SecurityEssentials.Unit.Tests.TestDbSet;
using System.Threading.Tasks;
using System;
using System.Security.Claims;

namespace SecurityEssentials.Unit.Tests.Core.Identity
{

	[TestClass]
    public class UserStoreTests
	{

		UserStore<User> _sut;
        ISEContext _context;
		protected User _testUser;
		private string testRoleName;
		private IAppConfiguration _configuration;

		[TestInitialize]
        public void Setup()
        {
			_context = MockRepository.GenerateStub<ISEContext>();
			_context.User = new TestDbSet<User>();
			_context.Stub(a => a.SaveChangesAsync()).Return(Task.FromResult(0));
			_configuration = MockRepository.GenerateStub<IAppConfiguration>();
			testRoleName = "test Role";
			_testUser = GetUser();
			_context.User.Add(_testUser);

			_sut = new UserStore<User>(_context, _configuration);
        }

		[TestMethod]
		public async Task Given_UserExists_When_FindAndCheckLogonAsync_Then_UserIsLoggedOn()
		{
			// Arrange

			// Act
			var result = await _sut.TryLogOnAsync(_testUser.UserName, "xsHDjxshdjkKK917&");

			// Assert
			Assert.IsTrue(result.Success);
			Assert.AreEqual(0, _testUser.FailedLogonAttemptCount);
			Assert.AreEqual(_testUser.UserName, result.UserName);
			_context.AssertWasCalled(a => a.SaveChangesAsync());
		}

		[TestMethod]
		public async Task Given_UserExistsButPasswordIncorrect_When_FindAndCheckLogonAsync_Then_UserIsNotLoggedOn()
		{
			// Arrange

			// Act
			var result = await _sut.TryLogOnAsync(_testUser.UserName, "rubbish");

			// Assert
			Assert.IsFalse(result.Success);
			Assert.AreEqual(2, _testUser.FailedLogonAttemptCount);
			Assert.AreNotEqual(_testUser.UserName, result.UserName);
			Assert.IsTrue(_testUser.UserLogs.Any(b => b.Description.Contains("Failed Logon")));
			_context.AssertWasCalled(a => a.SaveChangesAsync());
		}

		[TestMethod]
		public async Task Given_UserExists_When_CreateIdentityAsync_Then_ReturnsClaims()
		{
			// Arrange
			var authenticationType = "Forms";

			// Act
			var result = await _sut.CreateIdentityAsync(_testUser, authenticationType);

			// Assert
			Assert.AreEqual(4, result.Claims.Count());
			var nameIdentifier = result.Claims.Where(a => a.Type == ClaimTypes.NameIdentifier).FirstOrDefault();
			Assert.IsNotNull(nameIdentifier);
			Assert.AreEqual(_testUser.Id.ToString(), nameIdentifier.Value);
			var roleName = result.Claims.Where(a => a.Type == ClaimTypes.Role).FirstOrDefault();
			Assert.IsNotNull(roleName);
			Assert.AreEqual(testRoleName, roleName.Value);
		}

		[TestMethod]
		public async Task Given_ValidResetToken_When_ChangePasswordFromTokenAsync_Then_Success()
		{

			// Arrange
			var passwordResetToken = "jafueokvnsdsjrogdsjvnasqpzlmveyij";
			var newPassword = "pqlzmwoskeidjS1";
			_testUser.PasswordResetToken = passwordResetToken;
			_testUser.PasswordResetExpiry = DateTime.UtcNow.AddMinutes(10);
			_testUser.PasswordHash = null;
			_testUser.Salt = null;

			// Act
			var result = await _sut.ChangePasswordFromTokenAsync(_testUser.Id, passwordResetToken, newPassword);

			// Assert
			Assert.AreEqual(0, result.Errors.Count());
			_context.AssertWasCalled(a => a.SaveChangesAsync());
			Assert.IsNull(_testUser.PasswordResetExpiry);
			Assert.IsNull(_testUser.PasswordResetToken);
			Assert.AreEqual(0, _testUser.FailedLogonAttemptCount);
			Assert.AreEqual(1, _testUser.UserLogs.Count());
			Assert.IsTrue(_testUser.UserLogs.Any(a => a.Description.Contains("Password changed")));
			Assert.IsNotNull(_testUser.PasswordHash);
			Assert.IsNotNull(_testUser.Salt);

		}

		[TestMethod]
		public async Task Given_OldPasswordCorrect_When_ChangePasswordAsync_Then_Success()
		{

			// Arrange
			var oldPasswordHash = _testUser.PasswordHash;
			var oldSalt = _testUser.Salt;
			var oldPassword = "xsHDjxshdjkKK917&";
			var newPassword = "Anything12345";

			// Act
			var result = await _sut.ChangePasswordAsync(_testUser.Id, oldPassword, newPassword);

			// Assert
			Assert.AreEqual(0, result);
			_context.AssertWasCalled(a => a.SaveChangesAsync());
			Assert.AreEqual(1, _testUser.UserLogs.Count());
			Assert.IsTrue(_testUser.UserLogs.Any(a => a.Description.Contains("Password changed")));
			Assert.AreNotEqual(oldPasswordHash, _testUser.PasswordHash);
			Assert.AreNotEqual(oldSalt, _testUser.Salt);

		}

		protected User GetUser()
		{

			return new User()
			{
				Id = 7,
				PasswordHash = "BpC/5HcMA4pnktXCPGY6HeNY9fPPk24JvvN2YyR3JFcd2j6Nen0sZHrf1mucLSMuuxp3CfHWaPIct8jp11YYyUXgihhS+9VA4OUJVz7Ak1uvuT6M+qItK1+tdlsihrpk3PkiuWafte0lcStImz2sCJroxtoGzOxOGSnpFehPIgd5TZBvmI3Crphdxq/dJhRwHIVQrnrXzwA+Aapy3bcXvutFmxS9F3/31BU4F5dJcYWHu+KbPydUlFl7RnM6A7DsnNKVcoDnk1CJZiJCz7WWNos+m+iv0CBE4ENDuP20sLW6x51S/ktcz3mdbn9wT38JM5CoLbS1UdVxdYC+Dkv+kQ==",
				Salt = "K6GuRmwFwOupdDba+C1FqKYwyBuxCykesgiY+fmCVBNVwr7qafuQ7oj9HrgM3LTXMB9LtOkWc4Z7VzB3AjobRk4trmwy7yOyvXnZj9XcBom2s5htHz8tiYhgsV/fHLlNfbeFseOXMLqUN4AFf+/+07j2NiaQK+qLFDSOAFpvsfB6kHF5vk2JgJb8qQSaLAW5FrDFn4f6cqYQJg8H127xPm8WYJiU94sw4dd13XxneKUbzez3yikR20U7rfQMRFKUr2a14vApH4kGsg3F89n8B+w2A/Orz/iarA9uzATag0t2r5MPnQeG58odK5uOPTbWz1mka+gXVcY620SAdyo07Q==", // xsHDjxshdjkKK917&
				Enabled = true,
				Approved = true,
				EmailVerified = true,
				FailedLogonAttemptCount = 1,
				FirstName = "Bobby",
				UserName = "testuser@micro.com",
				EmailConfirmationToken = "test1",
				SecurityQuestionLookupItemId = 1,
				SecurityQuestionLookupItem = new LookupItem() { Id = 1, Description = "test question" }				,
				UserRoles = new List<UserRole>()
				{
					new UserRole() { Id = 1, Role = new Role() { Id = 7, Description = testRoleName }   }
				}
			};
		}

	}
}
