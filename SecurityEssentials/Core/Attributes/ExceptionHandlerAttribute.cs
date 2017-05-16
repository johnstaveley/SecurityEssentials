using System;
using System.Web;
using System.Web.Mvc;
using SecurityEssentials.Core.Constants;
using SecurityEssentials.Core.Identity;
using Serilog;

namespace SecurityEssentials.Core.Attributes
{
    public class ExceptionHandlerAttribute : FilterAttribute, IExceptionFilter
    {
        private readonly IUserIdentity _userIdentity;

        public ExceptionHandlerAttribute() : this(new UserIdentity())
        {
            // TODO: Replace with your DI Framework of choice
        }

        public ExceptionHandlerAttribute(IUserIdentity userIdentity)
        {
            if (userIdentity == null) throw new ArgumentNullException(nameof(userIdentity));

            _userIdentity = userIdentity;
        }

        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled || !(filterContext.Exception is HttpRequestValidationException)) return;
            // SECURE: Log XSS Attempt
            var action = filterContext.RouteData.Values["action"].ToString();
            var controller = filterContext.RouteData.Values["controller"].ToString();
            var requester = _userIdentity.GetRequester(filterContext.Controller as Controller,
                AppSensorDetectionPointKind.AE1);
            Log.Logger.Information(
                "Failed XSS attempt on controller {controller} and action {action} by requester {@requester}",
                controller, action, requester);
        }
    }
}