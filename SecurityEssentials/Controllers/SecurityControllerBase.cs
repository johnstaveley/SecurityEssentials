using SecurityEssentials.Core;
using SecurityEssentials.Core.Attributes;
using SecurityEssentials.Core.Identity;
using Serilog;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace SecurityEssentials.Controllers
{
	[ExceptionHandler]
    public abstract class SecurityControllerBase : Controller
    {

        private readonly ValidateAntiForgeryTokenAttribute _validator;
        private readonly AcceptVerbsAttribute _verbs;
		public ILogger Logger;
		public IUserIdentity _userIdentity;

        protected SecurityControllerBase() : this(HttpVerbs.Post, new UserIdentity())
        {
        }

        protected SecurityControllerBase(HttpVerbs verbs, IUserIdentity userIdentity)
        {
			if (userIdentity == null) throw new ArgumentNullException("userIdentity");
            _verbs = new AcceptVerbsAttribute(verbs);
            _validator = new ValidateAntiForgeryTokenAttribute();
			Logger = Log.Logger;
			_userIdentity = userIdentity;

		}

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null) throw new ArgumentNullException("filterContext is null");
            base.OnAuthorization(filterContext);

            string httpMethodOverride = filterContext.HttpContext.Request.GetHttpMethodOverride();
            if (_verbs.Verbs.Contains(httpMethodOverride, StringComparer.OrdinalIgnoreCase))
            {
                _validator.OnAuthorization(filterContext);
            }
        }

		protected void LogModelStateErrors(string method, ModelStateDictionary modelState)
		{
			// Assumption is that javascript is turned on on the client
			var allErrors = ModelState.Values.SelectMany(v => v.Errors).ToList();
			Requester requester = _userIdentity.GetRequester(this, null);
			foreach (var error in allErrors)
			{
				var errorMessage = error.ErrorMessage;
				requester.AppSensorDetectionPoint = null;
				if (errorMessage.Contains("is required"))
				{
					requester.AppSensorDetectionPoint = Core.Constants.AppSensorDetectionPointKind.RE6;
				}
				if (errorMessage.Contains("does not appear to be valid"))
				{
					requester.AppSensorDetectionPoint = Core.Constants.AppSensorDetectionPointKind.IE2;
				}
				if (errorMessage.Contains("with a maximum length of") || error.ErrorMessage.Contains("with a minimum length of"))
				{
					requester.AppSensorDetectionPoint = Core.Constants.AppSensorDetectionPointKind.RE7;
				}
				if (Regex.Match(errorMessage, @"The (.*) must be at least (\d+) and less than (\d+) characters long").Success)
				{
					if (errorMessage.Contains("User name") || errorMessage.Contains("Email Address"))
					{
						requester.AppSensorDetectionPoint = Core.Constants.AppSensorDetectionPointKind.AE4;
					}
					else if (errorMessage.Contains("Password"))
					{
						requester.AppSensorDetectionPoint = Core.Constants.AppSensorDetectionPointKind.AE5;
					}
					else
					{
						requester.AppSensorDetectionPoint = Core.Constants.AppSensorDetectionPointKind.RE7;
					}
				}
				Logger.Information("Failed {@method} validation bypass {errorMessage} attempted by user {@requester}", method, errorMessage, requester);
			}
		}
	}
}