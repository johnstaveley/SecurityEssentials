using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecurityEssentials.Core;

namespace SecurityEssentials.Unit.Tests.Core
{
	[TestClass]
    public class EmailTemplatesTests
    {

		private string _firstName = "John";
		private string _lastName = "Staveley";
		private string _applicationName = "Security Essentials";
		private string _url = "http://localhost:4986/";
		private string _token = "c4f32838-9ab4-46d8-bc49-d080264bd13e";
		private string _oldEmailAddress = "old@old.com";
		private string _newEmailAddress = "new@new.com";

		[TestMethod]
		public void When_GetRegistrationPendingBodyText_Then_CorrectEmailIsReturned()
		{

			// Act
			var emailText = EmailTemplates.RegistrationPendingBodyText(_firstName, _lastName, _applicationName, _url, _token);

			Assert.AreEqual("Dear John Staveley,<br /><br />Welcome to Security Essentials, to complete your registration we just need to confirm your email address by clicking <a href='http://localhost:4986/Account/EmailVerify?EmailVerficationToken=c4f32838-9ab4-46d8-bc49-d080264bd13e'>http://localhost:4986/Account/EmailVerify?EmailVerficationToken=c4f32838-9ab4-46d8-bc49-d080264bd13e</a>. If you did not request this registration then you can ignore this email and do not need to take any further action<br />How do I know this is not a Spoof email? Spoof or ‘phishing’ emails tend to have generic greetings such as \"Dear Security Essentials member\". Emails from Security Essentials will always contain your full name.<br />", emailText);

		}

		[TestMethod]
		public void When_GetRegistrationDuplicatedBodyText_Then_CorrectEmailIsReturned()
		{

			// Act
			var emailText = EmailTemplates.RegistrationDuplicatedBodyText(_firstName, _lastName, _applicationName, _url);

			Assert.AreEqual("Dear John Staveley,<br /><br />You already have an account on Security Essentials. You (or possibly someone else) just attempted to register on Security Essentials with this email address. However you are registered and cannot re-register with the same address. If you'd like to login you can do so by clicking here: <a href='http://localhost:4986/Account/LogOn'>http://localhost:4986/Account/LogOn</a>. If you have forgotten your password you can answer some security questions here to reset your password:<a href='http://localhost:4986/Account/LogOn'>http://localhost:4986/Account/Recover</a>. If it wasn't you who attempted to register with this email address or you did it by mistake, you can safely ignore this email<br />How do I know this is not a Spoof email? Spoof or ‘phishing’ emails tend to have generic greetings such as \"Dear Security Essentials member\". Emails from Security Essentials will always contain your full name.<br />", emailText);

		}

		[TestMethod]
        public void When_GetChangeEmailPendingBodyText_Then_CorrectEmailIsReturned()
        {

			// Act
			var emailBodyText = EmailTemplates.ChangeEmailAddressPendingBodyText(_firstName, _lastName, _applicationName, _url, _token);

			Assert.AreEqual("Dear John Staveley,<br /><br />A request has been received to change your Security Essentials username/email address. You can complete this process any time within the next 15 minutes by clicking <a href='http://localhost:4986/Account/ChangeEmailAddresConfirm?NewEmailAddressToken=c4f32838-9ab4-46d8-bc49-d080264bd13e'>http://localhost:4986/Account/ChangeEmailAddresConfirm?NewEmailAddressToken=c4f32838-9ab4-46d8-bc49-d080264bd13e</a>. If you did not request this then you can ignore this email.<br />How do I know this is not a Spoof email? Spoof or ‘phishing’ emails tend to have generic greetings such as \"Dear Security Essentials member\". Emails from Security Essentials will always contain your full name.<br />", emailBodyText);	

        }

		[TestMethod]
		public void When_GetChangeEmailCompletedBodyText_Then_CorrectEmailIsReturned()
		{

			// Act
			var emailText = EmailTemplates.ChangeEmailAddressCompletedBodyText(_firstName, _lastName, _applicationName, _oldEmailAddress, _newEmailAddress);

			Assert.AreEqual("Dear John Staveley,<br /><br />A request has been completed to change your Security Essentials username/email address from old@old.com to new@new.com. This email address can no longer be used to sign into the account. If you did not request this then please contact the website administration asap.<br />How do I know this is not a Spoof email? Spoof or ‘phishing’ emails tend to have generic greetings such as \"Dear Security Essentials member\". Emails from Security Essentials will always contain your full name.<br />", emailText);

		}

		[TestMethod]
		public void When_GetChangePasswordPendingBodyText_Then_CorrectEmailIsReturned()
		{

			// Act
			var emailText = EmailTemplates.ChangePasswordPendingBodyText(_firstName, _lastName, _applicationName, _url, _token);

			Assert.AreEqual("Dear John Staveley,<br /><br />A request has been received to reset your Security Essentials password. You can complete this process any time within the next 15 minutes by clicking <a href='http://localhost:4986/Account/RecoverPassword?PasswordResetToken=c4f32838-9ab4-46d8-bc49-d080264bd13e'>http://localhost:4986/Account/RecoverPassword?PasswordResetToken=c4f32838-9ab4-46d8-bc49-d080264bd13e</a>. If you did not make this request this then please ignore this email.<br />How do I know this is not a Spoof email? Spoof or ‘phishing’ emails tend to have generic greetings such as \"Dear Security Essentials member\". Emails from Security Essentials will always contain your full name.<br />", emailText);

		}

		[TestMethod]
		public void When_GetChangePasswordCompletedBodyText_Then_CorrectEmailIsReturned()
		{

			// Act
			var emailText = EmailTemplates.ChangePasswordCompletedBodyText(_firstName, _lastName, _applicationName);

			Assert.AreEqual("Dear John Staveley,<br /><br />Just a note from Security Essentials to say your password has been changed today, if this wasn't done by yourself, please contact the site administrator asap<br />How do I know this is not a Spoof email? Spoof or ‘phishing’ emails tend to have generic greetings such as \"Dear Security Essentials member\". Emails from Security Essentials will always contain your full name.<br />", emailText);

		}

		[TestMethod]
		public void When_GetChangeSecurityInformationCompletedBodyText_Then_CorrectEmailIsReturned()
		{

			// Act
			var emailText = EmailTemplates.ChangeSecurityInformationCompletedBodyText(_firstName, _lastName, _applicationName);

			Assert.AreEqual("Dear John Staveley,<br /><br />Please be advised that the security information on your Security Essentials account been changed. If you did not initiate this action then please contact the site administrator as soon as possible<br />How do I know this is not a Spoof email? Spoof or ‘phishing’ emails tend to have generic greetings such as \"Dear Security Essentials member\". Emails from Security Essentials will always contain your full name.<br />", emailText);

		}

	}

}
