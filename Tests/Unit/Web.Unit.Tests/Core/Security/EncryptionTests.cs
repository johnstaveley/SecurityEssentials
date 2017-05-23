using NUnit.Framework;
using SecurityEssentials.Core;

namespace SecurityEssentials.Unit.Tests.Core.Security
{
	/// <summary>
	/// Summary description for Encryption2
	/// </summary>
	[TestFixture]
	public class EncryptionTests
	{


		private readonly Encryption _testEncrypt;
		private readonly Encryption _testDecrypt;
		private string _password = "qrstuvwxzy1234567890";
		private int _iterationCount = 68;

		public EncryptionTests()
		{

			_testEncrypt = new Encryption();
			_testDecrypt = new Encryption();

		}

		[TearDown]
		public void MyTestCleanup()
		{
			_testEncrypt.Dispose();
			_testDecrypt.Dispose();
		}

		[Test]
		public void EnDecrypt_BasicMessage_Succeeds()
		{

			string input = "John Was here John was here again";
			string salt;
			string output;
			string encrypted;
			Assert.AreEqual(true, _testEncrypt.Encrypt(_password, _iterationCount, input, out salt, out encrypted), "Encryption was not successful");
			Assert.AreNotEqual(input, encrypted);
			Assert.AreEqual(true, _testDecrypt.Decrypt(_password, salt, _iterationCount, encrypted, out output), "Decryption was not successful");
			Assert.AreEqual(input, output);

		}

		[Test]
		public void EnDecrypt_ComplexMessage_Succeeds()
		{

			string input = "john was here 1234567890!£$%^&**()-_=+[]{}';,./#:@~?><  \\|`¬";
			string salt;
			string output;
			string encrypted;
			Assert.AreEqual(true, _testEncrypt.Encrypt(_password, _iterationCount, input, out salt, out encrypted), "Encryption was not successful");
			Assert.AreNotEqual(input, encrypted);
			Assert.AreEqual(true, _testDecrypt.Decrypt(_password, salt, _iterationCount, encrypted, out output), "Decryption was not successful");
			Assert.AreEqual(input, output);

		}

		[Test]
		public void EnDecrypt_BlankString_Succeeds()
		{

			string input = "";
			string salt;
			string output;
			string encrypted;
			Assert.AreEqual(true, _testEncrypt.Encrypt(_password, _iterationCount, input, out salt, out encrypted));
			Assert.AreNotEqual(input, encrypted);
			Assert.AreEqual(true, _testDecrypt.Decrypt(_password, salt, _iterationCount, encrypted, out output));
			Assert.AreEqual(input, output);

		}

		[Test]
		public void Encrypt_Null_DecryptsToZeroLengthString()
		{

			string input = null;
			string salt;
			string encrypted;
			string output;
			Assert.AreEqual(true, _testEncrypt.Encrypt(_password, _iterationCount, input, out salt, out encrypted));
			Assert.AreEqual(true, _testDecrypt.Decrypt(_password, salt, _iterationCount, encrypted, out output));
			Assert.AreNotEqual(input, output);
			Assert.AreEqual("", output);

		}

		[Test]
		public void EnDecrypt_Message_ChangeIterations_Fails()
		{

			string input = "The quick brown fox jumped over the lazy dog";
			string salt;
			string output;
			string encrypted;
			Assert.AreEqual(true, _testEncrypt.Encrypt(_password, _iterationCount, input, out salt, out encrypted));
			Assert.AreNotEqual(input, encrypted);
			Assert.AreNotEqual(true, _testDecrypt.Decrypt(_password, salt, _iterationCount + 1, encrypted, out output));
			Assert.AreNotEqual(input, output);

		}

		[Test]
		public void EnDecrypt_Message_ChangeSalt_Fails()
		{

			string input = "The quick brown fox jumped over the lazy dog";
			string salt;
			string output;
			string encrypted;
			Assert.AreEqual(true, _testEncrypt.Encrypt(_password, _iterationCount, input, out salt, out encrypted));
			Assert.AreNotEqual(input, encrypted);
			Assert.AreNotEqual(true, _testDecrypt.Decrypt(_password, $"{salt}1", _iterationCount, encrypted, out output));
			Assert.AreNotEqual(input, output);

		}

		[Test]
		public void EnDecrypt_Message_ChangePassword_Fails()
		{

			string input = "The quick brown fox jumped over the lazy dog";
			string salt;
			string output;
			string encrypted;
			Assert.AreEqual(true, _testEncrypt.Encrypt(_password, _iterationCount, input, out salt, out encrypted));
			Assert.AreNotEqual(input, encrypted);
			Assert.AreNotEqual(true, _testDecrypt.Decrypt(string.Format("{0}1", _password), salt, _iterationCount, encrypted, out output));
			Assert.AreNotEqual(input, output);
		}

	}
}
