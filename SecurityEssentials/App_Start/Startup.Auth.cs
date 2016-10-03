using System;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;

namespace SecurityEssentials.App_Start
{
	public static partial class Startup
	{
		// For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
		public static void ConfigureAuthentication(IAppBuilder app)
		{
			// Enable the application to use a cookie to store information for the signed in user
			app.UseCookieAuthentication(new CookieAuthenticationOptions
			{
				AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
				LoginPath = new PathString("/Account/Logon"),
				CookieName = "SecureAuth",
				CookieSecure = CookieSecureOption.SameAsRequest,
				CookieHttpOnly = true,
				Provider = new CookieAuthenticationProvider
				{
					OnApplyRedirect = ctx =>
					{
						if (!IsWebApiRequest(ctx.Request))
						{
							ctx.Response.Redirect(ctx.RedirectUri);
						}
					}
				},
				ExpireTimeSpan = TimeSpan.FromMinutes(60),
				SlidingExpiration = false
			});
			//// Use a cookie to temporarily store information about a user logging in with a third party login provider
			//app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
		}

		private static bool IsWebApiRequest(IOwinRequest request)
		{
			return (request.Path.StartsWithSegments(new PathString("/api")) || request.Path.StartsWithSegments(new PathString("/breeze")));
		}
	}


}