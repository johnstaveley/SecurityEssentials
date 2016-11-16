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
    public abstract class AntiForgeryControllerBase : Controller
    {

        private readonly ValidateAntiForgeryTokenAttribute _validator;
        private readonly AcceptVerbsAttribute _verbs;
		public ILogger Logger;
		public IUserIdentity _userIdentity;

        protected AntiForgeryControllerBase() : this(HttpVerbs.Post, new UserIdentity())
        {
        }

        protected AntiForgeryControllerBase(HttpVerbs verbs, IUserIdentity userIdentity)
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
				requester.AppSensorDetectionPoint = null;
				if (error.ErrorMessage.Contains("is required"))
				{
					requester.AppSensorDetectionPoint = Core.Constants.AppSensorDetectionPointKind.RE6;
				}
				if (Regex.Match(error.ErrorMessage, @"The (.*) must be at least (\d+) and less than (\d+) characters long").Success)
				{
					requester.AppSensorDetectionPoint = Core.Constants.AppSensorDetectionPointKind.RE7;
				}
				var errorMessage = error.ErrorMessage;
				Logger.Information("Failed {@method} validation bypass {errorMessage} attempted by user {@requester}", method, errorMessage, requester);
			}
		}


	}
}