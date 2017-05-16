using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecurityEssentials.Core.Identity;

namespace SecurityEssentials.Unit.Tests.Core.Security
{
	// TODO: Does not run on CI server, can't find library
	[TestClass]
    public class Argon2SecuredPasswordTests
    {


        [TestMethod]
        public void Given_PasswordHashAndSalt_Then_NewPasswordIsHashedAsExpected()
        {
            var password = "password1*SASDes";
            var securedPassword = new SecuredPassword(password, HashStrategyKind.Argon2_48kWorkCost);
            var storedSalt = Convert.ToBase64String(securedPassword.Salt);
            var storedHash = Convert.ToBase64String(securedPassword.Hash);

            var securedPassword2 = new SecuredPassword(password, Convert.FromBase64String(storedHash), 
				Convert.FromBase64String(storedSalt), HashStrategyKind.Argon2_48kWorkCost);
            Assert.IsTrue(securedPassword2.Equals(securedPassword));
        }

        [TestMethod]
        public void Given_PasswordHashWithIterationsChanged_Then_PasswordHashesDoNotMatch()
        {
            var password = "password1*SASDes";
            var securedPassword = new SecuredPassword(password, HashStrategyKind.Argon2_48kWorkCost);
            var securedPassword2 = new SecuredPassword(password, HashStrategyKind.PBKDF2_8000Iterations);
            Assert.IsFalse(securedPassword2.Equals(securedPassword));
        }


        [TestMethod]
        public void Given_PasswordString_Then_HashedAsExpected()
        {
            var securedPassword = new SecuredPassword("password", HashStrategyKind.Argon2_48kWorkCost);

            Assert.AreNotEqual("password", securedPassword.Hash);
            Assert.AreEqual(416, securedPassword.Hash.Length);
        }

        [TestMethod]
        public void Given_TwoIdenticalPasswords_Then_SaltsGeneratedAreUnique()
        {
            var securedPassword = new SecuredPassword("password", HashStrategyKind.Argon2_48kWorkCost);
            var securedPassword2 = new SecuredPassword("password", HashStrategyKind.Argon2_48kWorkCost);

            Assert.IsNotNull(securedPassword.Salt);
            Assert.IsNotNull(securedPassword2.Salt);
            Assert.AreNotEqual(securedPassword.Salt, securedPassword2.Salt);

        }

        [TestMethod]
        public void Given_TwoIdenticalPasswords_Then_HashsGeneratedAreUnique()
        {
            var securedPassword = new SecuredPassword("password", HashStrategyKind.Argon2_48kWorkCost);
            var securedPassword2 = new SecuredPassword("password", HashStrategyKind.Argon2_48kWorkCost);

            Assert.IsNotNull(securedPassword.Hash);
            Assert.IsNotNull(securedPassword2.Hash);
            Assert.AreNotEqual(securedPassword.Hash, securedPassword2.Hash);
        }

        [TestMethod]
        public void Given_SecuredPasswordGenerated_Then_MatchesAnIdenticalHash()
        {
            var securedPassword = new SecuredPassword("password", HashStrategyKind.Argon2_48kWorkCost);
			var securedPassword2 = new SecuredPassword("password", securedPassword.Hash, securedPassword.Salt, HashStrategyKind.Argon2_48kWorkCost);
			Assert.IsTrue(securedPassword2.IsValid);
			Assert.AreEqual(securedPassword.HashStrategy, securedPassword2.HashStrategy);
			Assert.AreEqual(securedPassword.HashingParameter, securedPassword2.HashingParameter);
        }

        [TestMethod]
        public void Given_SecuredPasswordIsDifferentToGiven_Then_VerifiesFalse()
        {
            var securedPassword = new SecuredPassword("password", HashStrategyKind.Argon2_48kWorkCost);
			var securedPassword2 = new SecuredPassword("Password2", HashStrategyKind.Argon2_48kWorkCost);
			var result = securedPassword.Equals(securedPassword2);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Given_WhenRehydratedAndMatching_Then_ReturnsTrue()
        {
			var password = "password123";
			var securedPassword = new SecuredPassword(password, HashStrategyKind.Argon2_48kWorkCost);
            var rehydrated = new SecuredPassword(password, securedPassword.Hash, securedPassword.Salt, HashStrategyKind.Argon2_48kWorkCost);
			Assert.IsTrue(securedPassword.Equals(rehydrated));
			Assert.IsTrue(rehydrated.IsValid);
        }

        [TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
        public void Given_PasswordVerifiesIsNull_Then_ThrowsException()
        {
            var result = new SecuredPassword(null, HashStrategyKind.Argon2_48kWorkCost);
        }

        /// <summary>
        /// Run on debug to get hash and salt for seeding database
        /// </summary>
        [TestMethod]
        public void CreatePasswordHashAndSaltForSeeding()
        {

            var password = "x12a;pP02icdjshER";
            var securedPassword = new SecuredPassword(password, HashStrategyKind.Argon2_48kWorkCost);
            var storedSalt = Convert.ToBase64String(securedPassword.Salt);
            var storedHash = Convert.ToBase64String(securedPassword.Hash);
            System.Diagnostics.Debug.WriteLine(string.Format("salt for password {0} is {1}", password, storedSalt));
            System.Diagnostics.Debug.WriteLine(string.Format("hash for password {0} is {1}", password, storedHash));

        }


    }

}
