namespace SecurityEssentials.Core
{
	public interface IEncryption
	{
		bool Decrypt(string password, string salt, int iterationCount, string input, out string output);
		void Dispose();
		bool Encrypt(string encryptionPassword, int iterationCount, string input, out string salt, out string output);
	}
}
