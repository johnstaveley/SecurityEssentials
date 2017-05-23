using SecurityEssentials.Core.Identity;
using Serilog;
using System;
using System.Linq;
using System.Web.Mvc;

namespace SecurityEssentials.Core.Attributes
{

	public class SeAuthorizeAttribute : AuthorizeAttribute
	{
		private readonly IUserIdentity _userIdentity;

		public SeAuthorizeAttribute()  : this(new UserIdentity())
		{
				
		}

		public SeAuthorizeAttribute(IUserIdentity userIdentity)
		{
			_userIdentity = userIdentity ?? throw new ArgumentNullException(nameof(userIdentity));
		}
		protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
		{
			string action = filterContext.ActionDescriptor.ActionName;
			string controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
			Requester requester = _userIdentity.GetRequester(filterContext.Controller as Controller);
			if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
			{
				// The user is not authenticated
				Log.Logger.Information("Failed access attempt on controller {controller} and action {action} which required authorization by requester {@requester}", controller, action, requester);
				base.HandleUnauthorizedRequest(filterContext);
			}
			else if (!Roles.Split(',').Any(filterContext.HttpContext.User.IsInRole))
			{
				// The user is not in any of the listed roles then log and show the unauthorized view
				Log.Logger.Information("Failed access attempt on controller {controller} and action {action} which required roles {roles} by requester {@requester}", controller, action, Roles, requester);
				filterContext.Result = new ViewResult
				{
					ViewName = "~/Views/Error/Unauthorized.cshtml"
				};
			}
			else
			{
				base.HandleUnauthorizedRequest(filterContext);
			}
		}
	}
}