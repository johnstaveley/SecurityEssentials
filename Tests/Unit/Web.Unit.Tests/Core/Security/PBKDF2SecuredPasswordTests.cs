using System;
using NUnit.Framework;
using SecurityEssentials.Core.Identity;

namespace SecurityEssentials.Unit.Tests.Core.Security
{
	[TestFixture]
	public class Pbkdf2SecuredPasswordTests
	{


		[Test]
		public void Given_PasswordHashAndSalt_Then_NewPasswordIsHashedAsExpected()
		{
			string password = "password1*SASDes";
			var securedPassword = new SecuredPassword(password, HashStrategyKind.Pbkdf210001Iterations);
			var storedSalt = Convert.ToBase64String(securedPassword.Salt);
			var storedHash = Convert.ToBase64String(securedPassword.Hash);

			var securedPassword2 = new SecuredPassword(password, Convert.FromBase64String(storedHash),
				Convert.FromBase64String(storedSalt), HashStrategyKind.Pbkdf210001Iterations);
			Assert.That(securedPassword2.Equals(securedPassword));
		}

		//[Test]
		//public void SetupPassword()
		//{
		//	string password = "xsHDjxshdjkKK917&";
		//	var securedPassword = new SecuredPassword(password, HashStrategyKind.Pbkdf210001Iterations);
		//	var storedSalt = Convert.ToBase64String(securedPassword.Salt);
		//	var storedHash = Convert.ToBase64String(securedPassword.Hash);
		//	System.Diagnostics.Trace.WriteLine("Hash = ", storedHash);
		//	System.Diagnostics.Trace.WriteLine("Salt = ", storedSalt);
		//}

		[Test]
		public void Given_PasswordHashWithIterationsChanged_Then_PasswordHashesDoNotMatch()
		{
			string password = "password1*SASDes";
			var securedPassword = new SecuredPassword(password, HashStrategyKind.Pbkdf210001Iterations);
			var securedPassword2 = new SecuredPassword(password, HashStrategyKind.Pbkdf210001Iterations);
			Assert.That(securedPassword2.Equals(securedPassword), Is.False);
		}


		[Test]
		public void Given_PasswordString_Then_HashedAsExpected()
		{
			var securedPassword = new SecuredPassword("password", HashStrategyKind.Pbkdf210001Iterations);

			Assert.That("password", Is.Not.EqualTo(securedPassword.Hash));
			Assert.That(256, Is.EqualTo(securedPassword.Hash.Length));
		}

		[Test]
		public void Given_TwoIdenticalPasswords_Then_SaltsGeneratedAreUnique()
		{
			var securedPassword = new SecuredPassword("password", HashStrategyKind.Pbkdf210001Iterations);
			var securedPassword2 = new SecuredPassword("password", HashStrategyKind.Pbkdf210001Iterations);

			Assert.That(securedPassword.Salt, Is.Not.Null);
			Assert.That(securedPassword2.Salt, Is.Not.Null);
			Assert.That(securedPassword.Salt, Is.Not.EqualTo(securedPassword2.Salt));

		}

		[Test]
		public void Given_TwoIdenticalPasswords_Then_HashsGeneratedAreUnique()
		{
			var securedPassword = new SecuredPassword("password", HashStrategyKind.Pbkdf210001Iterations);
			var securedPassword2 = new SecuredPassword("password", HashStrategyKind.Pbkdf210001Iterations);

			Assert.That(securedPassword.Hash, Is.Not.Null);
			Assert.That(securedPassword2.Hash, Is.Not.Null);
			Assert.That(securedPassword.Hash, Is.Not.EqualTo(securedPassword2.Hash));
		}

		[Test]
		public void Given_SecuredPasswordGenerated_Then_MatchesAnIdenticalHash()
		{
			var securedPassword = new SecuredPassword("password", HashStrategyKind.Pbkdf210001Iterations);
			var securedPassword2 = new SecuredPassword("password", securedPassword.Hash, securedPassword.Salt, HashStrategyKind.Pbkdf210001Iterations);
			Assert.That(securedPassword2.IsValid);
			Assert.That(securedPassword.HashStrategy, Is.EqualTo(securedPassword2.HashStrategy));
			Assert.That(securedPassword.HashingParameter, Is.EqualTo(securedPassword2.HashingParameter));
		}

		[Test]
		public void Given_SecuredPasswordIsDifferentToGiven_Then_VerifiesFalse()
		{
			var securedPassword = new SecuredPassword("password", HashStrategyKind.Pbkdf210001Iterations);
			var securedPassword2 = new SecuredPassword("Password2", HashStrategyKind.Pbkdf210001Iterations);
			var result = securedPassword.Equals(securedPassword2);
			Assert.That(result, Is.False);
		}

		[Test]
		public void Given_WhenRehydratedAndMatching_Then_ReturnsTrue()
		{
			var password = "password123";
			var securedPassword = new SecuredPassword(password, HashStrategyKind.Pbkdf210001Iterations);
			var rehydrated = new SecuredPassword(password, securedPassword.Hash, securedPassword.Salt, HashStrategyKind.Pbkdf210001Iterations);
			Assert.That(securedPassword.Equals(rehydrated));
			Assert.That(rehydrated.IsValid);
		}

		[Test]
		public void Given_PasswordVerifiesIsNull_Then_ThrowsException()
		{
			Assert.Throws<ArgumentNullException>(() =>
                {
                    new SecuredPassword(null, HashStrategyKind.Pbkdf210001Iterations);
                }
			);
		}

		/// <summary>
		/// Run on debug to get hash and salt for seeding database
		/// </summary>
		[Test]
		public void CreatePasswordHashAndSaltForSeeding()
		{

			string password = "x12a;pP02icdjshER";
			var securedPassword = new SecuredPassword(password, HashStrategyKind.Pbkdf210001Iterations);
			var storedSalt = Convert.ToBase64String(securedPassword.Salt);
			var storedHash = Convert.ToBase64String(securedPassword.Hash);
			System.Diagnostics.Debug.WriteLine($"salt for password {password} is {storedSalt}");
			System.Diagnostics.Debug.WriteLine($"hash for password {password} is {storedHash}");
		}
	}
}
