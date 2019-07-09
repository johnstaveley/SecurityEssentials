using SecurityEssentials.Core.Identity;
using Serilog;
using System;
using System.Web;
using System.Web.Mvc;

namespace SecurityEssentials.Core.Attributes
{
	public class ExceptionHandlerAttribute : FilterAttribute, IExceptionFilter
	{

		private readonly IUserIdentity _userIdentity;

		public ExceptionHandlerAttribute() : this(new UserIdentity())
		{
				
		}

		public ExceptionHandlerAttribute(IUserIdentity userIdentity)
		{
			_userIdentity = userIdentity ?? throw new ArgumentNullException(nameof(userIdentity));
		}

		public void OnException(ExceptionContext filterContext)
		{
			if (!filterContext.ExceptionHandled)
			{
				string action = filterContext.RouteData.Values["action"].ToString();
				string controller = filterContext.RouteData.Values["controller"].ToString();
				Requester requester = _userIdentity.GetRequester(filterContext.Controller as Controller);
				if (filterContext.Exception is HttpRequestValidationException)
				{
					// SECURE: Log XSS Attempt
					requester.AppSensorDetectionPoint = Constants.AppSensorDetectionPointKind.Ae1;
					Log.Logger.Information("Failed XSS attempt on controller {controller} and action {action}", controller, action);
				}
				else
				{
					Log.Logger.Information("Error on controller {controller} and action {action} by requester {@requester}", controller, action, requester);
#if !DEBUG
					filterContext.Result = new RedirectResult("/Error/Index/");
					filterContext.ExceptionHandled = true;
					filterContext.HttpContext.ClearError();
#endif

				}
			}

		}

	}
}
