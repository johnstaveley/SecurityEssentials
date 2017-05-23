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
		private readonly byte[] _hash;
		private readonly byte[] _salt;
		private HashStrategyKind _hashStrategy;

		public byte[] Hash => _hash;

		public HashStrategyKind HashStrategy => _hashStrategy;

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
				case HashStrategyKind.Pbkdf25009Iterations:
				case HashStrategyKind.Pbkdf28000Iterations:
					using (var deriveBytes = new Rfc2898DeriveBytes(plainPassword, _saltSize, (int) _hashingParameter))
					{
						_salt = deriveBytes.Salt;
						_hash = deriveBytes.GetBytes(_saltSize);
					}
					break;
				case HashStrategyKind.Argon248KWorkCost:
					var argon2Hasher = new PasswordHasher(memoryCost: _hashingParameter);
					_salt = PasswordHasher.GenerateSalt(256);
					_hash = Encoding.ASCII.GetBytes(argon2Hasher.Hash(Encoding.ASCII.GetBytes(plainPassword), _salt));
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
				case HashStrategyKind.Pbkdf25009Iterations:
				case HashStrategyKind.Pbkdf28000Iterations:
					using (var deriveBytes = new Rfc2898DeriveBytes(plainPassword, salt, (int)_hashingParameter))
					{
						_hash = deriveBytes.GetBytes(_saltSize);
					}
					break;
				case HashStrategyKind.Argon248KWorkCost:
					var argon2Hasher = new PasswordHasher(memoryCost: _hashingParameter);
					_hash = Encoding.ASCII.GetBytes(argon2Hasher.Hash(Encoding.ASCII.GetBytes(plainPassword), salt));
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
				case HashStrategyKind.Pbkdf25009Iterations:
				case HashStrategyKind.Pbkdf28000Iterations:
					using (var deriveBytes = new Rfc2898DeriveBytes(plainPassword, salt, (int) _hashingParameter))
					{
						newKey = deriveBytes.GetBytes(_saltSize);
						IsValid = newKey.SequenceEqual(hash);
					}
					break;
				case HashStrategyKind.Argon248KWorkCost:
					var argon2Hasher = new PasswordHasher(memoryCost: _hashingParameter);
					newKey = Encoding.ASCII.GetBytes(argon2Hasher.Hash(Encoding.ASCII.GetBytes(plainPassword), salt));
					IsValid = newKey.SequenceEqual(hash);
					break;
			}
			

		}

		private void SetHashStrategy(HashStrategyKind hashStrategy)
		{
			_hashStrategy = hashStrategy;
			switch (hashStrategy)
			{
				case HashStrategyKind.Pbkdf25009Iterations:
					_hashingParameter = 5009;
					_saltSize = 256;
					break;
				case HashStrategyKind.Pbkdf28000Iterations:
					_hashingParameter = 8000;
					_saltSize = 256;
					break;
				case HashStrategyKind.Argon248KWorkCost:
					_hashingParameter = 48000;
					_saltSize = 0;
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
