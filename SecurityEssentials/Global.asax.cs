using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Security.Claims;
using SecurityEssentials.Model;
using SecurityEssentials.Core;
using System.Web.Helpers;

namespace SecurityEssentials
{
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801

	public class MvcApplication : System.Web.HttpApplication
	{

		protected void Application_PreSendRequestHeaders()
		{
            // SECURE: Remove server information disclosure
			Response.Headers.Remove("Server");
			Response.Headers.Remove("X-AspNet-Version");
			Response.Headers.Remove("X-AspNetMvc-Version");
		}

		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();
			GlobalConfiguration.Configure(WebApiConfig.Register);
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
			AuthConfig.RegisterAuth();
			AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.Name;
			using (var context = new SEContext())
			{
				context.Database.Initialize(true);
			}
		}


		protected void Application_BeginRequest()
		{
#if !DEBUG
            // SECURE: Ensure any request is returned over SSL in production
            if (!Request.IsLocal && !Context.Request.IsSecureConnection) {
                var redirect = Context.Request.Url.ToString().ToLower(CultureInfo.CurrentCulture).Replace("http:", "https:");
                Response.Redirect(redirect);
            }
#endif
		}
	}
}