using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;

namespace SecurityEssentials.App_Start
{
    public static partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public static void ConfigureAuthorisation(IAppBuilder app)
        {
            // Enable the application to use a cookie to store information for the signed in user
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Logon"),
                CookieName = "SecureAuth",
#if !DEBUG
                CookieSecure = CookieSecureOption.Always,
#endif
				//Provider = new CookieAuthenticationProvider
				//{
				//	OnApplyRedirect = ctx =>
				//	{
				//		if (!IsWebApiRequest(ctx.Request))
				//		{
				//			ctx.Response.Redirect(ctx.RedirectUri);
				//		}
				//	}
				//}
			});
			//// Use a cookie to temporarily store information about a user logging in with a third party login provider
			//app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
        }
        
    }
}