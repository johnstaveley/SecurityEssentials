using Microsoft.AspNet.Identity;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using SecurityEssentials.Core.Identity;

namespace SecurityEssentials.Core.Attributes
{
	/// <summary>
	/// SECURE: If user has to change their password then redirect them to the change password page. If user has been logged off, then log them off
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public sealed class AccountManagementAttribute : ActionFilterAttribute
	{
		public IUserManager UserManager { get; set; }

		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			if (filterContext.HttpContext.User.Identity.IsAuthenticated)
			{
				var cachedForceLogout = HttpRuntime.Cache[string.Concat("ForceLogOff-", filterContext.HttpContext.User.Identity.GetUserId())];
				if (cachedForceLogout != null)
				{
					UserManager.SignOut();
					HttpRuntime.Cache.Remove(string.Concat("ForceLogOff-", filterContext.HttpContext.User.Identity.GetUserId()));
					filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "Controller", "Account" }, { "Action", "Logon" }, { "Reason", "ForcedLogOff" } });
				}
				if (!(filterContext.RouteData.Values.ContainsValue("Account") && (filterContext.RouteData.Values.ContainsValue("ChangePassword") || filterContext.RouteData.Values.ContainsValue("ChangePasswordAsync") || filterContext.RouteData.Values.ContainsValue("LogOff"))))
				{
					var cachedMustChangePassword = HttpRuntime.Cache[string.Concat("MustChangePassword-", filterContext.HttpContext.User.Identity.GetUserId())];
					if (cachedMustChangePassword != null)
					{
						filterContext.Result =
							new RedirectToRouteResult(new RouteValueDictionary { { "Controller", "Account" }, { "Action", "ChangePassword" }, { "Reason", "MustChangePassword" } });
					}
				}
			}
		}
	}
}