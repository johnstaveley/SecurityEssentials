using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecurityEssentials.Core.Identity;

namespace SecurityEssentials.Unit.Tests.Core.Security
{
	[TestClass]
    public class PBKDF2SecuredPasswordTests
    {


        [TestMethod]
        public void Given_PasswordHashAndSalt_Then_NewPasswordIsHashedAsExpected()
        {
            string password = "password1*SASDes";
            var securedPassword = new SecuredPassword(password, HashStrategyKind.PBKDF2_5009Iterations);
            var storedSalt = Convert.ToBase64String(securedPassword.Salt);
            var storedHash = Convert.ToBase64String(securedPassword.Hash);

            var securedPassword2 = new SecuredPassword(password, Convert.FromBase64String(storedHash), 
				Convert.FromBase64String(storedSalt), HashStrategyKind.PBKDF2_5009Iterations);
            Assert.IsTrue(securedPassword2.Equals(securedPassword));
        }

        [TestMethod]
        public void Given_PasswordHashWithIterationsChanged_Then_PasswordHashesDoNotMatch()
        {
            string password = "password1*SASDes";
            var securedPassword = new SecuredPassword(password, HashStrategyKind.PBKDF2_5009Iterations);
            var securedPassword2 = new SecuredPassword(password, HashStrategyKind.PBKDF2_8000Iterations);
            Assert.IsFalse(securedPassword2.Equals(securedPassword));
        }


        [TestMethod]
        public void Given_PasswordString_Then_HashedAsExpected()
        {
            var securedPassword = new SecuredPassword("password", HashStrategyKind.PBKDF2_5009Iterations);

            Assert.AreNotEqual("password", securedPassword.Hash);
            Assert.AreEqual(256, securedPassword.Hash.Length);
        }

        [TestMethod]
        public void Given_TwoIdenticalPasswords_Then_SaltsGeneratedAreUnique()
        {
            var securedPassword = new SecuredPassword("password", HashStrategyKind.PBKDF2_5009Iterations);
            var securedPassword2 = new SecuredPassword("password", HashStrategyKind.PBKDF2_5009Iterations);

            Assert.IsNotNull(securedPassword.Salt);
            Assert.IsNotNull(securedPassword2.Salt);
            Assert.AreNotEqual(securedPassword.Salt, securedPassword2.Salt);

        }

        [TestMethod]
        public void Given_TwoIdenticalPasswords_Then_HashsGeneratedAreUnique()
        {
            var securedPassword = new SecuredPassword("password", HashStrategyKind.PBKDF2_5009Iterations);
            var securedPassword2 = new SecuredPassword("password", HashStrategyKind.PBKDF2_5009Iterations);

            Assert.IsNotNull(securedPassword.Hash);
            Assert.IsNotNull(securedPassword2.Hash);
            Assert.AreNotEqual(securedPassword.Hash, securedPassword2.Hash);
        }

        [TestMethod]
        public void Given_SecuredPasswordGenerated_Then_MatchesAnIdenticalHash()
        {
            var securedPassword = new SecuredPassword("password", HashStrategyKind.PBKDF2_5009Iterations);
			var securedPassword2 = new SecuredPassword("password", securedPassword.Hash, securedPassword.Salt, HashStrategyKind.PBKDF2_5009Iterations);
			Assert.IsTrue(securedPassword2.IsValid);
			Assert.AreEqual(securedPassword.HashStrategy, securedPassword2.HashStrategy);
			Assert.AreEqual(securedPassword.HashingParameter, securedPassword2.HashingParameter);
        }

        [TestMethod]
        public void Given_SecuredPasswordIsDifferentToGiven_Then_VerifiesFalse()
        {
            var securedPassword = new SecuredPassword("password", HashStrategyKind.PBKDF2_5009Iterations);
			var securedPassword2 = new SecuredPassword("Password2", HashStrategyKind.PBKDF2_5009Iterations);
			var result = securedPassword.Equals(securedPassword2);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Given_WhenRehydratedAndMatching_Then_ReturnsTrue()
        {
			var password = "password123";
			var securedPassword = new SecuredPassword(password, HashStrategyKind.PBKDF2_5009Iterations);
            var rehydrated = new SecuredPassword(password, securedPassword.Hash, securedPassword.Salt, HashStrategyKind.PBKDF2_5009Iterations);
			Assert.IsTrue(securedPassword.Equals(rehydrated));
			Assert.IsTrue(rehydrated.IsValid);
        }

        [TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
        public void Given_PasswordVerifiesIsNull_Then_ThrowsException()
        {
            var result = new SecuredPassword(null, HashStrategyKind.PBKDF2_5009Iterations);
        }

        /// <summary>
        /// Run on debug to get hash and salt for seeding database
        /// </summary>
        [TestMethod]
        public void CreatePasswordHashAndSaltForSeeding()
        {

            string password = "x12a;pP02icdjshER";
            var securedPassword = new SecuredPassword(password, HashStrategyKind.PBKDF2_5009Iterations);
            var storedSalt = Convert.ToBase64String(securedPassword.Salt);
            var storedHash = Convert.ToBase64String(securedPassword.Hash);
            System.Diagnostics.Debug.WriteLine(string.Format("salt for password {0} is {1}", password, storedSalt));
            System.Diagnostics.Debug.WriteLine(string.Format("hash for password {0} is {1}", password, storedHash));

        }


    }

}
