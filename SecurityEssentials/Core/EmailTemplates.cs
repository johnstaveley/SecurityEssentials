using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SecurityEssentials.Core
{
	public static class EmailTemplates
	{

		private static string NotSpoofText(string applicationName)
		{
			return string.Format("How do I know this is not a Spoof email? Spoof or ‘phishing’ emails tend to have generic greetings such as \"Dear {0} member\". Emails from {0} will always contain your full name.<br />", applicationName);
		}

		public static string ChangeEmailAddressBodyText(string firstName, string lastName, string applicationName, string url, string token)
		{
			return string.Format("Dear {0} {1},<br /><br />A request has been received to change your {2} username/email address. You can complete this process any time within the next 15 minutes by clicking <a href='{3}Account/ChangeEmailAddresConfirm?NewEmailAddressToken={4}'>{3}Account/ChangeEmailAddresConfirm?NewEmailAddressToken={4}</a>. If you did not request this then you can ignore this email.<br />{5}", firstName, lastName, applicationName, url, token, NotSpoofText(applicationName));
		}

	}
}