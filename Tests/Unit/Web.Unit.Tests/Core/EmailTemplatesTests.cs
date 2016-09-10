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
