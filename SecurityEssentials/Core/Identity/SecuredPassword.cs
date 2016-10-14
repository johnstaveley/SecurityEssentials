using System;
using System.Linq;
using System.Security.Cryptography;
using Liphsoft.Crypto.Argon2;
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
		private bool _isValid = false;
		private readonly byte[] _hash;
		private readonly byte[] _salt;
		private HashStrategyKind _hashStrategy;

		public byte[] Hash
		{
			get { return _hash; }
		}

		public HashStrategyKind HashStrategy
		{
			get { return _hashStrategy; }
		}

		/// <summary>
		/// Number of iterations, work cost etc
		/// </summary>
		public uint HashingParameter
		{
			get { return _hashingParameter; }
		}

		public byte[] Salt
		{
			get { return _salt; }
		}		

		public bool IsValid
		{
			get { return _isValid; }
		}

		public SecuredPassword(string plainPassword, HashStrategyKind hashStrategy)
		{
			if (string.IsNullOrWhiteSpace(plainPassword))
			{
				throw new ArgumentNullException(plainPassword);
			}
			SetHashStrategy(hashStrategy);

			switch (hashStrategy)
			{
				case HashStrategyKind.PBKDF2_5009Iterations:
				case HashStrategyKind.PBKDF2_8000Iterations:
					using (var deriveBytes = new Rfc2898DeriveBytes(plainPassword, _saltSize, (int) _hashingParameter))
					{
						_salt = deriveBytes.Salt;
						_hash = deriveBytes.GetBytes(_saltSize);
					}
					break;
				case HashStrategyKind.Argon2_48kWorkCost:
					var argon2Hasher = new PasswordHasher(memoryCost: _hashingParameter);
					_salt = PasswordHasher.GenerateSalt((uint) 256);
					_hash = Encoding.ASCII.GetBytes(argon2Hasher.Hash(Encoding.ASCII.GetBytes(plainPassword), _salt));
					break;
			}
			_isValid = true;
		}

		public SecuredPassword(string plainPassword, byte[] hash, byte[] salt, HashStrategyKind hashStrategy)
		{
			_hash = hash;
			_salt = salt;
			SetHashStrategy(hashStrategy);
			byte[] newKey = null;
			switch (hashStrategy)
			{
				case HashStrategyKind.PBKDF2_5009Iterations:
				case HashStrategyKind.PBKDF2_8000Iterations:
					using (var deriveBytes = new Rfc2898DeriveBytes(plainPassword, salt, (int) _hashingParameter))
					{
						newKey = deriveBytes.GetBytes(_saltSize);
						_isValid = newKey.SequenceEqual(hash);
					}
					break;
				case HashStrategyKind.Argon2_48kWorkCost:
					var argon2Hasher = new PasswordHasher(memoryCost: _hashingParameter);
					newKey = Encoding.ASCII.GetBytes(argon2Hasher.Hash(Encoding.ASCII.GetBytes(plainPassword), salt));
					_isValid = newKey.SequenceEqual(hash);
					break;
			}
			

		}

		private void SetHashStrategy(HashStrategyKind hashStrategy)
		{
			_hashStrategy = hashStrategy;
			switch (hashStrategy)
			{
				case HashStrategyKind.PBKDF2_5009Iterations:
					_hashingParameter = 5009;
					_saltSize = 256;
					break;
				case HashStrategyKind.PBKDF2_8000Iterations:
					_hashingParameter = 8000;
					_saltSize = 256;
					break;
				case HashStrategyKind.Argon2_48kWorkCost:
					_hashingParameter = 48000;
					_saltSize = 0;
					break;
				default:
					throw new ArgumentException(string.Format("hashStrategy {0} is not defined", hashStrategy.ToString()));
			}

		}

		public bool Equals(SecuredPassword comparison)
		{
			if (_hash.SequenceEqual(comparison.Hash) && _salt.SequenceEqual(comparison.Salt)) return true;
			return false;
		}

	}
}
