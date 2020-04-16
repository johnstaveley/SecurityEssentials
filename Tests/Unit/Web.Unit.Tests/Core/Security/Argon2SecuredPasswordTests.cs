using NUnit.Framework;
using SecurityEssentials.Core.Identity;
using System;

namespace SecurityEssentials.Unit.Tests.Core.Security
{
	[TestFixture]
	public class Argon2SecuredPasswordTests
	{

		[Test]
		public void Given_PasswordHashAndSalt_Then_NewPasswordIsHashedAsExpected()
		{
			// Arrange
			string password = "password1*SASDes";
			var securedPassword = new SecuredPassword(password, HashStrategyKind.Argon2WorkCost);
            var storedSalt = Convert.ToBase64String(securedPassword.Salt);
			var storedHash = Convert.ToBase64String(securedPassword.Hash);

			// Act
			var securedPassword2 = new SecuredPassword(password, Convert.FromBase64String(storedHash),
				Convert.FromBase64String(storedSalt), HashStrategyKind.Argon2WorkCost);

			// Assert
			Assert.IsTrue(securedPassword2.Equals(securedPassword));
		}

		[Test]
		public void Given_PasswordHashChanged_Then_PasswordHashesDoNotMatch()
		{
			string password = "password1*SASDes";
			var securedPassword = new SecuredPassword(password, HashStrategyKind.Argon2WorkCost);
			var securedPassword2 = new SecuredPassword(password, HashStrategyKind.Pbkdf210001Iterations);
			Assert.IsFalse(securedPassword2.Equals(securedPassword));
		}


		[Test]
		public void Given_PasswordString_Then_HashedAsExpected()
		{
			var securedPassword = new SecuredPassword("password", HashStrategyKind.Argon2WorkCost);

			Assert.AreNotEqual("password", securedPassword.Hash);
			Assert.AreEqual(401, securedPassword.Hash.Length);
		}

		[Test]
		public void Given_TwoIdenticalPasswords_Then_SaltsGeneratedAreUnique()
		{
			var securedPassword = new SecuredPassword("password", HashStrategyKind.Argon2WorkCost);
			var securedPassword2 = new SecuredPassword("password", HashStrategyKind.Argon2WorkCost);

			Assert.IsNotNull(securedPassword.Salt);
			Assert.IsNotNull(securedPassword2.Salt);
			Assert.AreNotEqual(securedPassword.Salt, securedPassword2.Salt);

		}

		[Test]
		public void Given_TwoIdenticalPasswords_Then_HashsGeneratedAreUnique()
		{
			var securedPassword = new SecuredPassword("password", HashStrategyKind.Argon2WorkCost);
			var securedPassword2 = new SecuredPassword("password", HashStrategyKind.Argon2WorkCost);

			Assert.IsNotNull(securedPassword.Hash);
			Assert.IsNotNull(securedPassword2.Hash);
			Assert.AreNotEqual(securedPassword.Hash, securedPassword2.Hash);
		}

		[Test]
		public void Given_SecuredPasswordGenerated_Then_MatchesAnIdenticalHash()
		{
			// Arrange
			var securedPassword = new SecuredPassword("password", HashStrategyKind.Argon2WorkCost);

			// Act
			var securedPassword2 = new SecuredPassword("password", securedPassword.Hash, securedPassword.Salt, HashStrategyKind.Argon2WorkCost);

			//Assert
			Assert.IsTrue(securedPassword2.IsValid, "Secured password 2 is not valid");
			Assert.AreEqual(securedPassword.HashStrategy, securedPassword2.HashStrategy);
			Assert.AreEqual(securedPassword.HashingParameter, securedPassword2.HashingParameter);
		}

		[Test]
		public void Given_SecuredPasswordIsDifferentToGiven_Then_VerifiesFalse()
		{
			var securedPassword = new SecuredPassword("password", HashStrategyKind.Argon2WorkCost);
			var securedPassword2 = new SecuredPassword("Password2", HashStrategyKind.Argon2WorkCost);
			var result = securedPassword.Equals(securedPassword2);
			Assert.IsFalse(result);
		}

		[Test]
		public void Given_WhenRehydratedAndMatching_Then_ReturnsTrue()
		{
			// Arrange
			var password = "password123";
			var securedPassword = new SecuredPassword(password, HashStrategyKind.Argon2WorkCost);

			// Act
			var rehydrated = new SecuredPassword(password, securedPassword.Hash, securedPassword.Salt, HashStrategyKind.Argon2WorkCost);

			// Assert
			Assert.IsTrue(securedPassword.Equals(rehydrated));
			Assert.IsTrue(rehydrated.IsValid, "Rehydrated password is not valid");
		}

		[Test]
		public void Given_PasswordVerifiesIsNull_Then_ThrowsException()
		{
			Assert.Throws<ArgumentNullException>(() =>
				{ new SecuredPassword(null, HashStrategyKind.Pbkdf210001Iterations); }
			);
		}
	}

}
