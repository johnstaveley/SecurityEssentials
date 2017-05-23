using System;
using System.Web;
using System.Web.Mvc;

namespace SecurityEssentials.Core.Attributes
{
	/// <summary>
	/// SECURE: Apply this to any controller where the data is sensitive and should not be cached locally
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public sealed class NoCacheAttribute : ActionFilterAttribute
	{
		public override void OnResultExecuting(ResultExecutingContext filterContext)
		{
			filterContext.HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
			filterContext.HttpContext.Response.Cache.SetValidUntilExpires(false);
			filterContext.HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
			filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
			filterContext.HttpContext.Response.Cache.SetNoStore();

			base.OnResultExecuting(filterContext);
		}
	}
}