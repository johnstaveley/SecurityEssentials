using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Liphsoft.Crypto.Argon2;

namespace SecurityEssentials.Core.Identity
{
    /// <summary>
    ///     Uses a user specified algorithm with variable number of iterations
    /// </summary>
    public class SecuredPassword
    {
        private int _saltSize = 256;

        public SecuredPassword(string plainPassword, HashStrategyKind hashStrategy)
        {
            if (string.IsNullOrWhiteSpace(plainPassword))
                throw new ArgumentNullException(plainPassword);
            SetHashStrategy(hashStrategy);

            switch (hashStrategy)
            {
                case HashStrategyKind.PBKDF2_5009Iterations:
                case HashStrategyKind.PBKDF2_8000Iterations:
                    using (var deriveBytes = new Rfc2898DeriveBytes(plainPassword, _saltSize, (int) HashingParameter))
                    {
                        Salt = deriveBytes.Salt;
                        Hash = deriveBytes.GetBytes(_saltSize);
                    }
                    break;
                case HashStrategyKind.Argon2_48kWorkCost:
                    var argon2Hasher = new PasswordHasher(memoryCost: HashingParameter);
                    Salt = PasswordHasher.GenerateSalt(256);
                    Hash = Encoding.ASCII.GetBytes(argon2Hasher.Hash(Encoding.ASCII.GetBytes(plainPassword), Salt));
                    break;
            }
            IsValid = true;
        }

        public SecuredPassword(string plainPassword, byte[] hash, byte[] salt, HashStrategyKind hashStrategy)
        {
            Hash = hash;
            Salt = salt;
            SetHashStrategy(hashStrategy);
            byte[] newKey = null;
            switch (hashStrategy)
            {
                case HashStrategyKind.PBKDF2_5009Iterations:
                case HashStrategyKind.PBKDF2_8000Iterations:
                    using (var deriveBytes = new Rfc2898DeriveBytes(plainPassword, salt, (int) HashingParameter))
                    {
                        newKey = deriveBytes.GetBytes(_saltSize);
                        IsValid = newKey.SequenceEqual(hash);
                    }
                    break;
                case HashStrategyKind.Argon2_48kWorkCost:
                    var argon2Hasher = new PasswordHasher(memoryCost: HashingParameter);
                    newKey = Encoding.ASCII.GetBytes(argon2Hasher.Hash(Encoding.ASCII.GetBytes(plainPassword), salt));
                    IsValid = newKey.SequenceEqual(hash);
                    break;
            }
        }

        public byte[] Hash { get; }

        public HashStrategyKind HashStrategy { get; private set; }

        /// <summary>
        ///     Number of iterations, work cost etc
        /// </summary>
        public uint HashingParameter { get; private set; } = 5000;

        public byte[] Salt { get; }

        public bool IsValid { get; }

        private void SetHashStrategy(HashStrategyKind hashStrategy)
        {
            HashStrategy = hashStrategy;
            switch (hashStrategy)
            {
                case HashStrategyKind.PBKDF2_5009Iterations:
                    HashingParameter = 5009;
                    _saltSize = 256;
                    break;
                case HashStrategyKind.PBKDF2_8000Iterations:
                    HashingParameter = 8000;
                    _saltSize = 256;
                    break;
                case HashStrategyKind.Argon2_48kWorkCost:
                    HashingParameter = 48000;
                    _saltSize = 0;
                    break;
                default:
                    throw new ArgumentException(string.Format("hashStrategy {0} is not defined", hashStrategy));
            }
        }

        public bool Equals(SecuredPassword comparison)
        {
            if (Hash.SequenceEqual(comparison.Hash) && Salt.SequenceEqual(comparison.Salt)) return true;
            return false;
        }
    }
}