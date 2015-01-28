using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SecurityEssentials.Core
{
    public class SecuredPassword
    {

        #region Declarations

        private const int saltSize = 256;
        private const int defaultIterations = 5009;
        private int iterations = 5000;
        private readonly byte[] hash;
        private readonly byte[] salt;

        #endregion

        #region Properties

        public byte[] Hash
        {
            get { return hash; }
        }

        public int Iterations
        {
            get { return iterations; }
        }

        public byte[] Salt
        {
            get { return salt; }
        }

        #endregion

        #region Constructor

        public SecuredPassword(string plainPassword, int iterations = defaultIterations)
        {
            if (string.IsNullOrWhiteSpace(plainPassword))
                return;
            this.iterations = iterations;

            using (var deriveBytes = new Rfc2898DeriveBytes(plainPassword, saltSize, iterations))
            {
                salt = deriveBytes.Salt;
                hash = deriveBytes.GetBytes(saltSize);
            }
        }

        public SecuredPassword(byte[] hash, byte[] salt, int iterations = defaultIterations)
        {
            this.hash = hash;
            this.salt = salt;
            this.iterations = iterations;
        }

        #endregion

        #region Verify

        public bool Verify(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            using (var deriveBytes = new Rfc2898DeriveBytes(password, salt, iterations))
            {
                byte[] newKey = deriveBytes.GetBytes(saltSize);
                return newKey.SequenceEqual(hash);
            }
        }

        #endregion

    }
}
