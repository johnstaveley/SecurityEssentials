using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;

namespace SecurityEssentials.Core
{
	public static class Services
	{

		#region SendEmail

		/// <summary>
		///     Send Email - Requires smtp settings inside the web.config file
		/// </summary>
		/// <param name="from">Who to send email from.</param>
		/// <param name="to">Who to send email to.</param>
		/// <param name="cc">Who to copy into the email.</param>
		/// <param name="bcc">Who to blind copy into the email.</param>
		/// <param name="subject">The subject line of the email.</param>
		/// <param name="body">The body of the email.</param>
		/// <param name="htmlEmail">Is the email html</param>
		/// <returns>boolean value</returns>
		public static bool SendEmail(string from, ICollection<string> toAddresses, ICollection<string> cc, ICollection<string> bcc, string subject,
									string body, bool htmlEmail)
		{
			if (toAddresses == null) throw new ArgumentException("toAddresses has not been specified");
			try
			{
				using (var message = new MailMessage())
				{
					message.From = new MailAddress(from);

					foreach (string item in toAddresses)
					{
						message.To.Add(new MailAddress(item));
					}

					if (cc != null)
					{
						foreach (string item in cc)
						{
							message.CC.Add(new MailAddress(item));
						}
					}

					if (bcc != null)
					{
						foreach (string item in bcc)
						{
							message.Bcc.Add(new MailAddress(item));
						}
					}

					message.Subject = subject;
					message.IsBodyHtml = htmlEmail;
					message.Body = body;

					using (var client = new SmtpClient())
					{
						client.Send(message);
					}

					return true;
				}
			}
			catch// (Exception ex)
			{
				//Elmah.ErrorLog.GetDefault(HttpContext.Current).Log(new Elmah.Error(ex));
				return false;
			}
		}

		#endregion

	}
}

