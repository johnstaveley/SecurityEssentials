using SecurityEssentials.App_Start;
using SecurityEssentials.Controllers;
using SecurityEssentials.Core;
using SecurityEssentials.Core.Identity;
using Serilog;
using System;
using System.Configuration;
using System.Net.Http.Formatting;
using System.Security.Claims;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.AspNet.Identity;
using Serilog.Core;

namespace SecurityEssentials
{
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801

	public class MvcApplication : HttpApplication
	{

		protected void Application_Start()
		{
			DependencyInjection.Configure();
			AreaRegistration.RegisterAllAreas();
			GlobalConfiguration.Configure(WebApiConfig.Register);
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
			AuthConfig.RegisterAuth();
			AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.Name;
			// SECURE: Remove automatic XFrame option header so we can add it in filters to entire site
			AntiForgeryConfig.SuppressXFrameOptionsHeader = true;

			// SECURE: Remove server information disclosure
			MvcHandler.DisableMvcResponseHeader = true;

			using (var context = new SeContext())
			{
				context.Database.Initialize(true);
			}
			Log.Logger = new LoggerConfiguration()
				.WriteTo.MSSqlServer(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString(), "Logs")
				.MinimumLevel.Debug()
				.CreateLogger();
			Log.Information("Application started");
		}

		protected void Application_Error(object sender, EventArgs e)
		{
			var httpContext = ((MvcApplication)sender).Context;
			var currentController = " ";
			var currentAction = " ";
			var currentRouteData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(httpContext));

			if (currentRouteData != null)
			{
				if (currentRouteData.Values["controller"] != null && !String.IsNullOrEmpty(currentRouteData.Values["controller"].ToString()))
				{
					currentController = currentRouteData.Values["controller"].ToString();
				}

				if (currentRouteData.Values["action"] != null && !String.IsNullOrEmpty(currentRouteData.Values["action"].ToString()))
				{
					currentAction = currentRouteData.Values["action"].ToString();
				}
			}

			var ex = Server.GetLastError();
			// TODO: Remove service location antipattern
			var controller = new ErrorController(DependencyInjection.Container.GetInstance<IUserIdentity>(), DependencyInjection.Container.GetInstance<AppSensor>());
			var routeData = new RouteData();
			var action = "Index";

			if (ex is HttpException)
			{
				var httpEx = ex as HttpException;

				switch (httpEx.GetHttpCode())
				{
					case 404:
						action = "NotFound";
						break;
					case 403:
						action = "Forbidden";
						break;

						// others if any
				}
			}

			//httpContext.ClearError();
			httpContext.Response.Clear();
			httpContext.Response.StatusCode = ex is HttpException ? ((HttpException)ex).GetHttpCode() : 500;
			httpContext.Response.TrySkipIisCustomErrors = true;

			routeData.Values["controller"] = "Error";
			routeData.Values["action"] = action;

			controller.ViewData.Model = new HandleErrorInfo(ex, currentController, currentAction);
			((IController)controller).Execute(new RequestContext(new HttpContextWrapper(httpContext), routeData));
		}

		protected void Application_End(Object sender, EventArgs e)
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

		//protected void Application_EndRequest()
		//{
		//	// Divert user to custom 404 page
		//	if (Context.Response.StatusCode == 404)
		//	{
		//		Response.Clear();

		//		var rd = new RouteData();
		//		rd.Values["controller"] = "Error";
		//		rd.Values["action"] = "NotFound";

		//		IController c = new ErrorController();
		//		c.Execute(new RequestContext(new HttpContextWrapper(Context), rd));
		//	}
		//}

		void Session_Start(object sender, EventArgs e)
		{
			// SECURE: Adding a variable to session keeps the session id constant between requests which is useful for logging and identifying a user between requests
			HttpContext.Current.Session.Add("__MyAppSession", string.Empty);
		}

	}
}