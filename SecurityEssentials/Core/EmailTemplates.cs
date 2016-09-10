using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SecurityEssentials.Core
{
	public static class EmailTemplates
	{

		public static string ChangePasswordPendingBodyText(string firstName, string lastName, string applicationName, string url, string passwordResetToken)
		{
			return string.Format("{0}A request has been received to reset your {1} password. You can complete this process any time within the next 15 minutes by clicking <a href='{2}Account/RecoverPassword?PasswordResetToken={3}'>{2}Account/RecoverPassword?PasswordResetToken={3}</a>. If you did not make this request this then please ignore this email.{4}", GetGreeting(firstName, lastName), applicationName, url, passwordResetToken, NotSpoofText(applicationName));
		}

		public static string ChangePasswordCompletedBodyText(string firstName, string lastName, string applicationName)
		{
			return string.Format("{0}Just a note from {1} to say your password has been changed today, if this wasn't done by yourself, please contact the site administrator asap{2}", GetGreeting(firstName, lastName), applicationName, NotSpoofText(applicationName));
		}

		public static string ChangeSecurityInformationCompletedBodyText(string firstName, string lastName, string applicationName)
		{
			return string.Format("{0}Please be advised that the security information on your {1} account been changed. If you did not initiate this action then please contact the site administrator as soon as possible{2}", GetGreeting(firstName, lastName), applicationName, NotSpoofText(applicationName));
		}

		public static string ChangeEmailAddressPendingBodyText(string firstName, string lastName, string applicationName, string url, string token)
		{
			return string.Format("{0}A request has been received to change your {1} username/email address. You can complete this process any time within the next 15 minutes by clicking <a href='{2}Account/ChangeEmailAddresConfirm?NewEmailAddressToken={3}'>{2}Account/ChangeEmailAddresConfirm?NewEmailAddressToken={3}</a>. If you did not request this then you can ignore this email.{4}", GetGreeting(firstName, lastName), applicationName, url, token, NotSpoofText(applicationName));
		}

		public static string ChangeEmailAddressCompletedBodyText(string firstName, string lastName, string applicationName, string oldEmailAddress, string newEmailAddress)
		{
			return string.Format("{0}A request has been completed to change your {1} username/email address from {2} to {3}. This email address can no longer be used to sign into the account. If you did not request this then please contact the website administration asap.{4}", GetGreeting(firstName, lastName), applicationName, oldEmailAddress, newEmailAddress, NotSpoofText(applicationName));
		}

		public static string GetGreeting(string firstName, string lastName)
		{
			return string.Format("Dear {0} {1},<br /><br />", firstName, lastName);
		}

		private static string NotSpoofText(string applicationName)
		{
			return string.Format("<br />How do I know this is not a Spoof email? Spoof or ‘phishing’ emails tend to have generic greetings such as \"Dear {0} member\". Emails from {0} will always contain your full name.<br />", applicationName);
		}
	}
}