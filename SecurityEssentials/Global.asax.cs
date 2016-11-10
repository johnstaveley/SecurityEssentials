using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Security.Claims;
using SecurityEssentials.Core;
using System.Web.Helpers;
using Serilog;
using System.Configuration;
using SecurityEssentials.Controllers;
using System.Web;
using System;

namespace SecurityEssentials
{
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801

	public class MvcApplication : System.Web.HttpApplication
	{

		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();
			GlobalConfiguration.Configure(WebApiConfig.Register);
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
			AuthConfig.RegisterAuth();
			AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.Name;
			// SECURE: Remove automatic XFrame option header so we can add it in filters to entire site
			System.Web.Helpers.AntiForgeryConfig.SuppressXFrameOptionsHeader = true;

			// SECURE: Remove server information disclosure
			MvcHandler.DisableMvcResponseHeader = true;

			using (var context = new SEContext())
			{
				context.Database.Initialize(true);
			}
			Log.Logger = new LoggerConfiguration()
				.WriteTo.MSSqlServer(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString(), "Logs")
				.MinimumLevel.Debug()
				.CreateLogger();
			Log.Information("Application started");
		}

		protected void Application_End()
		{
			Log.Information("Application finished");
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


		protected void Application_EndRequest()
		{
			// Divert user to custom 404 page
			if (Context.Response.StatusCode == 404)
			{
				Response.Clear();

				var rd = new RouteData();
				rd.Values["controller"] = "Error";
				rd.Values["action"] = "NotFound";

				IController c = new ErrorController();
				c.Execute(new RequestContext(new HttpContextWrapper(Context), rd));
			}
		}

		void Session_Start(object sender, EventArgs e)
		{
			// Adding a variable to session keeps the session id constant between requests
			HttpContext.Current.Session.Add("__MyAppSession", string.Empty);
		}

	}
}