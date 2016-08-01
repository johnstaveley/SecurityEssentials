using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SecurityEssentials.Core
{
	/// <summary>
	/// Http handler that ensures the site can never be loaded in an iFrame and any request to the site is made over SSL and removes server identity header
	/// </summary>
	public class HttpHeaders : IHttpModule
	{
		public void Init(HttpApplication context)
		{
			context.PreSendRequestHeaders += context_PreSendRequestHeaders;
		}

		void context_PreSendRequestHeaders(object sender, EventArgs e)
		{
			HttpContext.Current.Response.Headers.Add("X-Frame-Options", "Deny"); // SECURE: Prevent site being viewed in an iFrame
			HttpContext.Current.Response.Headers.Add("X-Content-Type-Options", "nosniff"); // SECURE: Prevent site being displayed in a different format in older browsers c.f. https://www.owasp.org/index.php/List_of_useful_HTTP_headers
			HttpContext.Current.Response.Headers.Add("X-XSS-Protection", "1; mode=block"); // SECURE: Enable browsers anti-XSS protection https://www.owasp.org/index.php/XSS_(Cross_Site_Scripting)_Prevention_Cheat_Sheet
#if !DEBUG
			HttpContext.Current.Response.AddHeader("Strict-Transport-Security", "max-age=31536000"); // Ensure all future requests to the site are made over SSL
#endif
			HttpContext.Current.Response.Headers.Remove("Server"); // Remove server information disclosure
		}

		public void Dispose()
		{
		}
	}
}