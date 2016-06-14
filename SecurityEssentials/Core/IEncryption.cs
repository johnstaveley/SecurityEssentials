using System;
namespace SecurityEssentials.Core
{
    public interface IEncryption
    {
        bool Decrypt(string password, string salt, int iterationCount, string input, out string output);
        void Dispose();
        bool Encrypt(string password, string salt, int iterationCount, string input, out string output);
    }
}
