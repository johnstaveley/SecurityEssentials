using System;
using NUnit.Framework;
using SecurityEssentials.Core.Identity;

namespace SecurityEssentials.Unit.Tests.Core.Security
{
	// TODO: Does not run on CI server, can't find library
	[TestFixture]
	public class Argon2SecuredPasswordTests
	{


		[Test]
		public void Given_PasswordHashAndSalt_Then_NewPasswordIsHashedAsExpected()
		{
			string password = "password1*SASDes";
			var securedPassword = new SecuredPassword(password, HashStrategyKind.Argon248KWorkCost);
			var storedSalt = Convert.ToBase64String(securedPassword.Salt);
			var storedHash = Convert.ToBase64String(securedPassword.Hash);

			var securedPassword2 = new SecuredPassword(password, Convert.FromBase64String(storedHash),
				Convert.FromBase64String(storedSalt), HashStrategyKind.Argon248KWorkCost);
			Assert.IsTrue(securedPassword2.Equals(securedPassword));
		}

		[Test]
		public void Given_PasswordHashWithIterationsChanged_Then_PasswordHashesDoNotMatch()
		{
			string password = "password1*SASDes";
			var securedPassword = new SecuredPassword(password, HashStrategyKind.Argon248KWorkCost);
			var securedPassword2 = new SecuredPassword(password, HashStrategyKind.Pbkdf28000Iterations);
			Assert.IsFalse(securedPassword2.Equals(securedPassword));
		}


		[Test]
		public void Given_PasswordString_Then_HashedAsExpected()
		{
			var securedPassword = new SecuredPassword("password", HashStrategyKind.Argon248KWorkCost);

			Assert.AreNotEqual("password", securedPassword.Hash);
			Assert.AreEqual(416, securedPassword.Hash.Length);
		}

		[Test]
		public void Given_TwoIdenticalPasswords_Then_SaltsGeneratedAreUnique()
		{
			var securedPassword = new SecuredPassword("password", HashStrategyKind.Argon248KWorkCost);
			var securedPassword2 = new SecuredPassword("password", HashStrategyKind.Argon248KWorkCost);

			Assert.IsNotNull(securedPassword.Salt);
			Assert.IsNotNull(securedPassword2.Salt);
			Assert.AreNotEqual(securedPassword.Salt, securedPassword2.Salt);

		}

		[Test]
		public void Given_TwoIdenticalPasswords_Then_HashsGeneratedAreUnique()
		{
			var securedPassword = new SecuredPassword("password", HashStrategyKind.Argon248KWorkCost);
			var securedPassword2 = new SecuredPassword("password", HashStrategyKind.Argon248KWorkCost);

			Assert.IsNotNull(securedPassword.Hash);
			Assert.IsNotNull(securedPassword2.Hash);
			Assert.AreNotEqual(securedPassword.Hash, securedPassword2.Hash);
		}

		[Test]
		public void Given_SecuredPasswordGenerated_Then_MatchesAnIdenticalHash()
		{
			var securedPassword = new SecuredPassword("password", HashStrategyKind.Argon248KWorkCost);
			var securedPassword2 = new SecuredPassword("password", securedPassword.Hash, securedPassword.Salt, HashStrategyKind.Argon248KWorkCost);
			Assert.IsTrue(securedPassword2.IsValid);
			Assert.AreEqual(securedPassword.HashStrategy, securedPassword2.HashStrategy);
			Assert.AreEqual(securedPassword.HashingParameter, securedPassword2.HashingParameter);
		}

		[Test]
		public void Given_SecuredPasswordIsDifferentToGiven_Then_VerifiesFalse()
		{
			var securedPassword = new SecuredPassword("password", HashStrategyKind.Argon248KWorkCost);
			var securedPassword2 = new SecuredPassword("Password2", HashStrategyKind.Argon248KWorkCost);
			var result = securedPassword.Equals(securedPassword2);
			Assert.IsFalse(result);
		}

		[Test]
		public void Given_WhenRehydratedAndMatching_Then_ReturnsTrue()
		{
			var password = "password123";
			var securedPassword = new SecuredPassword(password, HashStrategyKind.Argon248KWorkCost);
			var rehydrated = new SecuredPassword(password, securedPassword.Hash, securedPassword.Salt, HashStrategyKind.Argon248KWorkCost);
			Assert.IsTrue(securedPassword.Equals(rehydrated));
			Assert.IsTrue(rehydrated.IsValid);
		}

		[Test]
		public void Given_PasswordVerifiesIsNull_Then_ThrowsException()
		{
			Assert.Throws<ArgumentNullException>(() =>
				{ new SecuredPassword(null, HashStrategyKind.Pbkdf25009Iterations); }
			);
		}
	}

}
