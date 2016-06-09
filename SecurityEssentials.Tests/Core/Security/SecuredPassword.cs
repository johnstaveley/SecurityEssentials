using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecurityEssentials.Core;

namespace SecurityEssentials.Unit.Tests
{
    [TestClass]
    public class When_PasswordHash
    {


        [TestMethod]
        public void Given_PasswordHashAndSalt_Then_NewPasswordIsHashedAsExpected()
        {
            int iterations = 4652;
            string password = "password1*SASDes";
            var securedPassword = new SecuredPassword(password, iterations);
            var storedSalt = Convert.ToBase64String(securedPassword.Salt);
            var storedHash = Convert.ToBase64String(securedPassword.Hash);

            var securedPassword2 = new SecuredPassword(Convert.FromBase64String(storedHash), Convert.FromBase64String(storedSalt), iterations);
            Assert.IsTrue(securedPassword2.Verify(password));
        }

        [TestMethod]
        public void Given_PasswordHashAndSaltWithIterationsChanged_Then_NewPasswordDoesNotMatch()
        {
            int iterations = 4652;
            string password = "password1*SASDes";
            var securedPassword = new SecuredPassword(password, iterations);
            var storedSalt = Convert.ToBase64String(securedPassword.Salt);
            var storedHash = Convert.ToBase64String(securedPassword.Hash);

            iterations = 4653;
            var securedPassword2 = new SecuredPassword(Convert.FromBase64String(storedHash), Convert.FromBase64String(storedSalt), iterations);
            Assert.IsFalse(securedPassword2.Verify(password));
        }


        [TestMethod]
        public void Given_PasswordString_Then_HashedAsExpected()
        {
            var securedPassword = new SecuredPassword("password");

            Assert.AreNotEqual("password", securedPassword.Hash);
            Assert.AreEqual(256, securedPassword.Hash.Length);
        }

        [TestMethod]
        public void Given_TwoIdenticalPasswords_Then_SaltsGeneratedAreUnique()
        {
            var securedPassword = new SecuredPassword("password");
            var securedPassword2 = new SecuredPassword("password");

            Assert.IsNotNull(securedPassword.Salt);
            Assert.IsNotNull(securedPassword2.Salt);
            Assert.AreNotEqual(securedPassword.Salt, securedPassword2.Salt);

        }

        [TestMethod]
        public void Given_TwoIdenticalPasswords_Then_HashsGeneratedAreUnique()
        {
            var securedPassword = new SecuredPassword("password");
            var securedPassword2 = new SecuredPassword("password");

            Assert.IsNotNull(securedPassword.Hash);
            Assert.IsNotNull(securedPassword2.Hash);
            Assert.AreNotEqual(securedPassword.Hash, securedPassword2.Hash);
        }

        [TestMethod]
        public void Given_SecuredPasswordGenerated_Then_VerifiesOk()
        {
            var securedPassword = new SecuredPassword("password");
            var result = securedPassword.Verify("password");
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Given_SecuredPasswordIsDifferentToGiven_Then_VerifiesFalse()
        {
            var securedPassword = new SecuredPassword("password");
            var result = securedPassword.Verify("Password");
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Given_WhenRehydratedAndMatching_Then_ReturnsTrue()
        {
            var securedPassword = new SecuredPassword("password123");
            var rehydrated = new SecuredPassword(securedPassword.Hash, securedPassword.Salt);
            var result = rehydrated.Verify("password123");
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Given_PasswordVerifiesIsNull_Then_VerifiesFalse()
        {
            Assert.IsFalse(new SecuredPassword("password").Verify(null));
        }

        /// <summary>
        /// Run on debug to get hash and salt for seeding database
        /// </summary>
        [TestMethod]
        public void CreatePasswordHashAndSaltForSeeding()
        {

            string password = "x12a;pP02icdjshER";
            var securedPassword = new SecuredPassword(password);
            var storedSalt = Convert.ToBase64String(securedPassword.Salt);
            var storedHash = Convert.ToBase64String(securedPassword.Hash);
            System.Diagnostics.Debug.WriteLine(string.Format("salt for password {0} is {1}", password, storedSalt));
            System.Diagnostics.Debug.WriteLine(string.Format("hash for password {0} is {1}", password, storedHash));

        }


    }

}
