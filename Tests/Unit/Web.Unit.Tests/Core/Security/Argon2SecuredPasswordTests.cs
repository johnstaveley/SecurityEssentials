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
			Assert.That(securedPassword2.Equals(securedPassword));
		}

		[Test]
		public void Given_PasswordHashChanged_Then_PasswordHashesDoNotMatch()
		{
			string password = "password1*SASDes";
			var securedPassword = new SecuredPassword(password, HashStrategyKind.Argon2WorkCost);
			var securedPassword2 = new SecuredPassword(password, HashStrategyKind.Pbkdf210001Iterations);
			Assert.That(securedPassword2.Equals(securedPassword), Is.False);
		}


		[Test]
		public void Given_PasswordString_Then_HashedAsExpected()
		{
			var securedPassword = new SecuredPassword("password", HashStrategyKind.Argon2WorkCost);

			Assert.That("password", Is.Not.EqualTo(securedPassword.Hash));
			Assert.That(401, Is.EqualTo(securedPassword.Hash.Length));
		}

		[Test]
		public void Given_TwoIdenticalPasswords_Then_SaltsGeneratedAreUnique()
		{
			var securedPassword = new SecuredPassword("password", HashStrategyKind.Argon2WorkCost);
			var securedPassword2 = new SecuredPassword("password", HashStrategyKind.Argon2WorkCost);

			Assert.That(securedPassword.Salt, Is.Not.Null);
			Assert.That(securedPassword2.Salt, Is.Not.Null);
			Assert.That(securedPassword.Salt, Is.Not.EqualTo(securedPassword2.Salt));

		}

		[Test]
		public void Given_TwoIdenticalPasswords_Then_HashsGeneratedAreUnique()
		{
			var securedPassword = new SecuredPassword("password", HashStrategyKind.Argon2WorkCost);
			var securedPassword2 = new SecuredPassword("password", HashStrategyKind.Argon2WorkCost);

			Assert.That(securedPassword.Hash, Is.Not.Null);
			Assert.That(securedPassword2.Hash, Is.Not.Null);
			Assert.That(securedPassword.Hash, Is.Not.EqualTo(securedPassword2.Hash));
		}

		[Test]
		public void Given_SecuredPasswordGenerated_Then_MatchesAnIdenticalHash()
		{
			// Arrange
			var securedPassword = new SecuredPassword("password", HashStrategyKind.Argon2WorkCost);

			// Act
			var securedPassword2 = new SecuredPassword("password", securedPassword.Hash, securedPassword.Salt, HashStrategyKind.Argon2WorkCost);

			//Assert
			Assert.That(securedPassword2.IsValid, "Secured password 2 is not valid");
			Assert.That(securedPassword.HashStrategy, Is.EqualTo(securedPassword2.HashStrategy));
			Assert.That(securedPassword.HashingParameter, Is.EqualTo(securedPassword2.HashingParameter));
		}

		[Test]
		public void Given_SecuredPasswordIsDifferentToGiven_Then_VerifiesFalse()
		{
			var securedPassword = new SecuredPassword("password", HashStrategyKind.Argon2WorkCost);
			var securedPassword2 = new SecuredPassword("Password2", HashStrategyKind.Argon2WorkCost);
			var result = securedPassword.Equals(securedPassword2);
			Assert.That(result, Is.False);
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
			Assert.That(securedPassword.Equals(rehydrated));
			Assert.That(rehydrated.IsValid, "Rehydrated password is not valid");
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
