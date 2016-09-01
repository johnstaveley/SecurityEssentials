using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SecurityEssentials.Core.Identity
{

	/// <summary>
	/// Uses a user specified algorithm with variable number of iterations
	/// </summary>
	public class SecuredPassword
	{

		private int _saltSize = 256;
		private int _iterations = 5000;
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

		public int Iterations
		{
			get { return _iterations; }
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
					using (var deriveBytes = new Rfc2898DeriveBytes(plainPassword, _saltSize, _iterations))
					{
						_salt = deriveBytes.Salt;
						_hash = deriveBytes.GetBytes(_saltSize);
					}
					break;
			}
			_isValid = true;
		}

		public SecuredPassword(string plainPassword, byte[] hash, byte[] salt, HashStrategyKind hashStrategy)
		{
			_hash = hash;
			_salt = salt;
			SetHashStrategy(hashStrategy);
			switch (hashStrategy)
			{
				case HashStrategyKind.PBKDF2_5009Iterations:
				case HashStrategyKind.PBKDF2_8000Iterations:
					using (var deriveBytes = new Rfc2898DeriveBytes(plainPassword, salt, _iterations))
					{
						byte[] newKey = deriveBytes.GetBytes(_saltSize);
						_isValid = newKey.SequenceEqual(hash);
					}
					break;
			}
			

		}

		private void SetHashStrategy(HashStrategyKind hashStrategy)
		{
			_hashStrategy = hashStrategy;
			switch (hashStrategy)
			{
				case HashStrategyKind.PBKDF2_5009Iterations:
					_iterations = 5009;
					_saltSize = 256;
					break;
				case HashStrategyKind.PBKDF2_8000Iterations:
					_iterations = 8000;
					_saltSize = 256;
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
