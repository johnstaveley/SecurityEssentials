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
			var securedPassword = new SecuredPassword(password, HashStrategyKind.Pbkdf25009Iterations);
			var storedSalt = Convert.ToBase64String(securedPassword.Salt);
			var storedHash = Convert.ToBase64String(securedPassword.Hash);

			var securedPassword2 = new SecuredPassword(password, Convert.FromBase64String(storedHash),
				Convert.FromBase64String(storedSalt), HashStrategyKind.Pbkdf25009Iterations);
			Assert.IsTrue(securedPassword2.Equals(securedPassword));
		}

		//[Test]
		//public void SetupPassword()
		//{
		//	string password = "test";
		//	var securedPassword = new SecuredPassword(password, HashStrategyKind.Pbkdf2_8000Iterations);
		//	var storedSalt = Convert.ToBase64String(securedPassword.Salt);
		//	var storedHash = Convert.ToBase64String(securedPassword.Hash);
		//	System.Diagnostics.Trace.WriteLine("Hash = ", storedHash);
		//	System.Diagnostics.Trace.WriteLine("Salt = ", storedSalt);
		//}

		[Test]
		public void Given_PasswordHashWithIterationsChanged_Then_PasswordHashesDoNotMatch()
		{
			string password = "password1*SASDes";
			var securedPassword = new SecuredPassword(password, HashStrategyKind.Pbkdf25009Iterations);
			var securedPassword2 = new SecuredPassword(password, HashStrategyKind.Pbkdf28000Iterations);
			Assert.IsFalse(securedPassword2.Equals(securedPassword));
		}


		[Test]
		public void Given_PasswordString_Then_HashedAsExpected()
		{
			var securedPassword = new SecuredPassword("password", HashStrategyKind.Pbkdf25009Iterations);

			Assert.AreNotEqual("password", securedPassword.Hash);
			Assert.AreEqual(256, securedPassword.Hash.Length);
		}

		[Test]
		public void Given_TwoIdenticalPasswords_Then_SaltsGeneratedAreUnique()
		{
			var securedPassword = new SecuredPassword("password", HashStrategyKind.Pbkdf25009Iterations);
			var securedPassword2 = new SecuredPassword("password", HashStrategyKind.Pbkdf25009Iterations);

			Assert.IsNotNull(securedPassword.Salt);
			Assert.IsNotNull(securedPassword2.Salt);
			Assert.AreNotEqual(securedPassword.Salt, securedPassword2.Salt);

		}

		[Test]
		public void Given_TwoIdenticalPasswords_Then_HashsGeneratedAreUnique()
		{
			var securedPassword = new SecuredPassword("password", HashStrategyKind.Pbkdf25009Iterations);
			var securedPassword2 = new SecuredPassword("password", HashStrategyKind.Pbkdf25009Iterations);

			Assert.IsNotNull(securedPassword.Hash);
			Assert.IsNotNull(securedPassword2.Hash);
			Assert.AreNotEqual(securedPassword.Hash, securedPassword2.Hash);
		}

		[Test]
		public void Given_SecuredPasswordGenerated_Then_MatchesAnIdenticalHash()
		{
			var securedPassword = new SecuredPassword("password", HashStrategyKind.Pbkdf25009Iterations);
			var securedPassword2 = new SecuredPassword("password", securedPassword.Hash, securedPassword.Salt, HashStrategyKind.Pbkdf25009Iterations);
			Assert.IsTrue(securedPassword2.IsValid);
			Assert.AreEqual(securedPassword.HashStrategy, securedPassword2.HashStrategy);
			Assert.AreEqual(securedPassword.HashingParameter, securedPassword2.HashingParameter);
		}

		[Test]
		public void Given_SecuredPasswordIsDifferentToGiven_Then_VerifiesFalse()
		{
			var securedPassword = new SecuredPassword("password", HashStrategyKind.Pbkdf25009Iterations);
			var securedPassword2 = new SecuredPassword("Password2", HashStrategyKind.Pbkdf25009Iterations);
			var result = securedPassword.Equals(securedPassword2);
			Assert.IsFalse(result);
		}

		[Test]
		public void Given_WhenRehydratedAndMatching_Then_ReturnsTrue()
		{
			var password = "password123";
			var securedPassword = new SecuredPassword(password, HashStrategyKind.Pbkdf25009Iterations);
			var rehydrated = new SecuredPassword(password, securedPassword.Hash, securedPassword.Salt, HashStrategyKind.Pbkdf25009Iterations);
			Assert.IsTrue(securedPassword.Equals(rehydrated));
			Assert.IsTrue(rehydrated.IsValid);
		}

		[Test]
		public void Given_PasswordVerifiesIsNull_Then_ThrowsException()
		{
			Assert.Throws<ArgumentNullException>(() =>
				{ var result = new SecuredPassword(null, HashStrategyKind.Pbkdf25009Iterations); }
			);
		}

		/// <summary>
		/// Run on debug to get hash and salt for seeding database
		/// </summary>
		[Test]
		public void CreatePasswordHashAndSaltForSeeding()
		{

			string password = "x12a;pP02icdjshER";
			var securedPassword = new SecuredPassword(password, HashStrategyKind.Pbkdf25009Iterations);
			var storedSalt = Convert.ToBase64String(securedPassword.Salt);
			var storedHash = Convert.ToBase64String(securedPassword.Hash);
			System.Diagnostics.Debug.WriteLine(string.Format("salt for password {0} is {1}", password, storedSalt));
			System.Diagnostics.Debug.WriteLine(string.Format("hash for password {0} is {1}", password, storedHash));

		}


	}

}
