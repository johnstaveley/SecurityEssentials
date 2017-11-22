using System;
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
			context.PreSendRequestHeaders += Context_PreSendRequestHeaders;
		}

		private void Context_PreSendRequestHeaders(object sender, EventArgs e)
		{
			// SECURE: Prevent site being viewed in an iFrame
			HttpContext.Current.Response.Headers.Add("X-Frame-Options", "Deny");
			// SECURE: Can effectively prevent information being inadvertently leaked to other websites
			HttpContext.Current.Response.Headers.Add("Referrer-Policy", "origin");
#if DEBUG
			// SECURE: Enable Content security policy
			HttpContext.Current.Response.Headers.Add("Content-Security-Policy", "default-src 'self'; style-src 'self' 'unsafe-inline'; img-src * data:; font-src 'self' data:; script-src 'self' 'unsafe-inline' 'unsafe-eval'; connect-src 'self'; report-uri /Security/CspReporting");
			//HttpContext.Current.Response.Headers.Add("Content-Security-Policy", "default-src 'self'; report-uri /Security/CspReporting");
#else
			HttpContext.Current.Response.Headers.Add("Content-Security-Policy", "default-src https:; style-src https: 'unsafe-inline'; img-src https: data:; font-src https: data:; script-src https: 'unsafe-inline' 'unsafe-eval'; connect-src https:; ");
#endif
			HttpContext.Current.Response.Headers.Add("X-Content-Type-Options", "nosniff"); // SECURE: Prevent site being displayed in a different format in older browsers c.f. https://www.owasp.org/index.php/List_of_useful_HTTP_headers
			HttpContext.Current.Response.Headers.Add("X-XSS-Protection", "1; mode=block"); // SECURE: Enable browsers anti-XSS protection https://www.owasp.org/index.php/XSS_(Cross_Site_Scripting)_Prevention_Cheat_Sheet
#if DEBUG
			HttpContext.Current.Response.Headers.Add("Strict-Transport-Security", "max-age=0; includeSubDomains"); // Remove HSTS header for debug
			HttpContext.Current.Response.Headers.Add("Public-Key-Pins", "max-age=0; includeSubDomains"); // Remove public key pinning for debug
#else
// SECURE: TODO: Consider use of public key pinning when used with an installed TLS certificate
//HttpContext.Current.Response.Headers.Add("Public-Key-Pins", "pin-sha256=\"<primary key>\"; pin-sha256=\"<backup key>\"; max-age=10000; includeSubDomains");
// SECURE: Enforce any further requests after initial request to be made over TLS
			HttpContext.Current.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
#endif
			HttpContext.Current.Response.Headers.Remove("Server"); // SECURE: Remove server information disclosure
		}

		public void Dispose()
		{
		}
	}
}
