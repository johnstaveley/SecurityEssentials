using Isopoh.Cryptography.Argon2;
using Isopoh.Cryptography.SecureArray;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SecurityEssentials.Core.Identity
{

	/// <summary>
	/// Uses a user specified algorithm with variable number of iterations
	/// </summary>
	public class SecuredPassword
	{

		private int _saltSize = 256;
		private uint _hashingParameter = 5000;
		private readonly byte[] _hash;
		private readonly byte[] _salt;
		private HashStrategyKind _hashStrategy;

		public byte[] Hash => _hash;

		public HashStrategyKind HashStrategy => _hashStrategy;
        private static readonly RandomNumberGenerator RandomNumberGenerator = RandomNumberGenerator.Create();

		/// <summary>
		/// Number of iterations, work cost etc
		/// </summary>
		public uint HashingParameter => _hashingParameter;
		public byte[] Salt => _salt;
		public bool IsValid { get; }

		/// <summary>
		/// Given a plain password and a hash strategy, calculate the salt and hash
		/// </summary>
		public SecuredPassword(string plainPassword, HashStrategyKind hashStrategy)
		{
			if (string.IsNullOrWhiteSpace(plainPassword))
			{
				throw new ArgumentNullException(plainPassword);
			}
			SetHashStrategy(hashStrategy);

			switch (hashStrategy)
			{
				case HashStrategyKind.Pbkdf210001Iterations:
                    var numberOfIterations = (int) _hashingParameter;
					if (numberOfIterations <= 10000) throw new ArgumentException("Iterations must be greater than 10000");
					using (var deriveBytes = new Rfc2898DeriveBytes(plainPassword, _saltSize, numberOfIterations, HashAlgorithmName.SHA256))
					{
						_salt = deriveBytes.Salt;
						_hash = deriveBytes.GetBytes(_saltSize);
					}
					break;
				case HashStrategyKind.Argon2WorkCost:
                    byte[] passwordBytes = Encoding.UTF8.GetBytes(plainPassword);
                    _salt = new byte[_saltSize];
                    RandomNumberGenerator.GetBytes(_salt); 
                    var config = new Argon2Config
                    {
                        Type = Argon2Type.DataIndependentAddressing,
                        Version = Argon2Version.Nineteen,
                        TimeCost = 10,
                        MemoryCost = (int) _hashingParameter,
                        Lanes = 5,
                        Threads = Environment.ProcessorCount,
                        Password = passwordBytes,
                        Salt = _salt, 
                        HashLength = 20
                    };
                    var argon2A = new Argon2(config);
                    using(SecureArray<byte> hashArgon = argon2A.Hash())
                    {
                        _hash = Encoding.ASCII.GetBytes(config.EncodeString(hashArgon.Buffer));
                    }
					break;
			}
			IsValid = true;
		}

		/// <summary>
		/// Given a password, salt and hash strategy, calculate the hash
		/// </summary>
		/// <param name="plainPassword"></param>
		/// <param name="salt"></param>
		/// <param name="hashStrategy"></param>
		public SecuredPassword(string plainPassword, byte[] salt, HashStrategyKind hashStrategy)
		{
			_salt = salt;
			SetHashStrategy(hashStrategy);
			switch (hashStrategy)
			{
				case HashStrategyKind.Pbkdf210001Iterations:
					var numberOfIterations = (int) _hashingParameter;
                    if (numberOfIterations <= 10000) throw new ArgumentException("Iterations must be greater than 10000");
					using (var deriveBytes = new Rfc2898DeriveBytes(plainPassword, salt, numberOfIterations, HashAlgorithmName.SHA256))
					{
						_hash = deriveBytes.GetBytes(_saltSize);
					}
					break;
				case HashStrategyKind.Argon2WorkCost:
                    byte[] passwordBytes = Encoding.UTF8.GetBytes(plainPassword);
                    var config = new Argon2Config
                    {
                        Type = Argon2Type.DataIndependentAddressing,
                        Version = Argon2Version.Nineteen,
                        TimeCost = 10,
                        MemoryCost = (int) _hashingParameter,
                        Lanes = 5,
                        Threads = Environment.ProcessorCount,
                        Password = passwordBytes,
                        Salt = _salt,
                        HashLength = 20 // >= 4
                    };
                    var argon2A = new Argon2(config);
                    using(SecureArray<byte> hashArgon = argon2A.Hash())
                    {
                        _hash = Encoding.ASCII.GetBytes(config.EncodeString(hashArgon.Buffer));
                    }
					break;
			}
			IsValid = true;
		}

		/// <summary>
		/// Compares a hash, salt and plain password and sets IsValid
		/// </summary>
		public SecuredPassword(string plainPassword, byte[] hash, byte[] salt, HashStrategyKind hashStrategy)
		{
			_hash = hash;
			_salt = salt;
			SetHashStrategy(hashStrategy);
			byte[] newKey;
			switch (hashStrategy)
			{
				case HashStrategyKind.Pbkdf210001Iterations:
                    var numberOfIterations = (int) _hashingParameter;
                    if (numberOfIterations <= 10000) throw new ArgumentException("Iterations must be greater than 10000");
					using (var deriveBytes = new Rfc2898DeriveBytes(plainPassword, salt, numberOfIterations, HashAlgorithmName.SHA256))
					{
						newKey = deriveBytes.GetBytes(_saltSize);
						IsValid = newKey.SequenceEqual(hash);
					}
					break;
				case HashStrategyKind.Argon2WorkCost:
                    SecureArray<byte> hashB = null;
                    try
                    {
                        var passwordBytes = Encoding.ASCII.GetBytes(plainPassword);
                        var configOfPasswordToVerify = new Argon2Config 
						{ 
                            Type = Argon2Type.DataIndependentAddressing,
                            Version = Argon2Version.Nineteen,
                            TimeCost = 10,
                            MemoryCost = (int) _hashingParameter,
                            Lanes = 5,
                            Threads = Environment.ProcessorCount,
                            Salt = _salt, 
                            Password = passwordBytes,
							HashLength = 20
                        };
						var hashString = Encoding.ASCII.GetString(_hash);
                        if (configOfPasswordToVerify.DecodeString(hashString, out hashB) && hashB != null)
                        {
                            var argon2ToVerify = new Argon2(configOfPasswordToVerify);
                            using(var hashToVerify = argon2ToVerify.Hash())
                            {
                                if (!hashB.Buffer.Where((b, i) => b != hashToVerify[i]).Any())
                                {
                                    IsValid = true;
                                }
                            }
                        }
                    }
                    finally
                    {
                        hashB?.Dispose();
                    }
					break;
			}
			

		}

		private void SetHashStrategy(HashStrategyKind hashStrategy)
		{
			_hashStrategy = hashStrategy;
			switch (hashStrategy)
			{
				case HashStrategyKind.Pbkdf210001Iterations:
					_hashingParameter = 100001;
					_saltSize = 256;
					break;
				case HashStrategyKind.Argon2WorkCost:
					_hashingParameter = 32768;
					_saltSize = 256;
					break;
				default:
					throw new ArgumentException($"hashStrategy {hashStrategy} is not defined");
			}

		}

		public bool Equals(SecuredPassword comparison)
		{
			if (_hash.SequenceEqual(comparison.Hash) && _salt.SequenceEqual(comparison.Salt)) return true;
			return false;
		}

	}
}
