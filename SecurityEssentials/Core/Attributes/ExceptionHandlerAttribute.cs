using System;
using System.Linq;
using SecurityEssentials.Core.Identity;
using System.Web;
using System.Web.Mvc;
using Serilog;
using System.Reflection;
using System.Web.Routing;

namespace SecurityEssentials.Core.Attributes
{
	public class ExceptionHandlerAttribute : FilterAttribute, IExceptionFilter
	{

		private IUserIdentity _userIdentity;

		public ExceptionHandlerAttribute() : this(new UserIdentity())
		{
			// TODO: Replace with your DI Framework of choice
		}

		public ExceptionHandlerAttribute(IUserIdentity userIdentity)
		{
			if (userIdentity == null) throw new ArgumentNullException("userIdentity");

			_userIdentity = userIdentity;

		}

		public void OnException(ExceptionContext filterContext)
		{
			if (!filterContext.ExceptionHandled && filterContext.Exception is HttpRequestValidationException)
			{
				// SECURE: Log XSS Attempt
				string action = filterContext.RouteData.Values["action"].ToString();
				string controller = filterContext.RouteData.Values["controller"].ToString();
				Requester requester = _userIdentity.GetRequester(filterContext.Controller as Controller, Constants.AppSensorDetectionPointKind.AE1);
				Log.Logger.Information("Failed XSS attempt on controller {controller} and action {action} by requester {@requester}", controller, action, requester);
			}
		}
		
	}
}
