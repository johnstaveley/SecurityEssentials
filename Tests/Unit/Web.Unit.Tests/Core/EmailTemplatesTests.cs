using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecurityEssentials.Core.Identity;
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

		[TestMethod]
        public void When_GetChangeEmailBodyText_Then_CorrectEmailIsReturned()
        {

			// Act
			var changeEmailBodyText = EmailTemplates.ChangeEmailAddressBodyText(_firstName, _lastName, _applicationName, _url, _token);

			Assert.AreEqual("Dear John Staveley,<br /><br />A request has been received to change your Security Essentials username/email address. You can complete this process any time within the next 15 minutes by clicking <a href='http://localhost:4986/Account/ChangeEmailAddresConfirm?NewEmailAddressToken=c4f32838-9ab4-46d8-bc49-d080264bd13e'>http://localhost:4986/Account/ChangeEmailAddresConfirm?NewEmailAddressToken=c4f32838-9ab4-46d8-bc49-d080264bd13e</a>. If you did not request this then you can ignore this email.<br />How do I know this is not a Spoof email? Spoof or ‘phishing’ emails tend to have generic greetings such as \"Dear Security Essentials member\". Emails from Security Essentials will always contain your full name.<br />", changeEmailBodyText);	

        }

	}

}
