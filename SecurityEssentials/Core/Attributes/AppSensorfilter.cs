using SecurityEssentials.Core.Constants;
using SecurityEssentials.Core.Identity;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace SecurityEssentials.Core.Attributes
{
	public class AppSensorFilter : ActionFilterAttribute
	{

		public List<string> ExpectedFormKeys { get; set; }
		private IUserIdentity _userIdentity;
		private ILogger _logger;

		public AppSensorFilter()
			: this(new UserIdentity(), Log.Logger)
		{
			// TODO: Implement using IoC
		}

		public AppSensorFilter(IUserIdentity userIdentity, ILogger logger)
		{
			if (logger == null) throw new ArgumentNullException("logger");
			if (userIdentity == null) throw new ArgumentNullException("userIdentity");

			_logger = logger;
			_userIdentity = userIdentity;
		}

		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			
			var httpMethod = filterContext.HttpContext.Request.HttpMethod;
			if (httpMethod == "POST" || httpMethod == "PUT")
			{
				var controller = filterContext.Controller;
				var keysSent = filterContext.HttpContext.Request.Form.AllKeys.ToList();
				var controllerMethod = filterContext.HttpContext.Request.CurrentExecutionFilePath.Trim('~').Trim('/').Split('/');
				var controllerName = controllerMethod[0];
				var methodName = controllerMethod[1];

				if (!ExpectedFormKeys.Contains("__RequestVerificationToken") && (httpMethod == "POST" || httpMethod == "PUT")) ExpectedFormKeys.Add("__RequestVerificationToken");
				// Check if any additional fields have been provided
				var additionalKeys = keysSent.Except(ExpectedFormKeys).ToList();
				if (additionalKeys.Count > 0)
				{
					var requester = _userIdentity.GetRequester((Controller)controller, AppSensorDetectionPointKind.RE5);
					if (controllerName == "Account" && methodName == "LogOn" && httpMethod == "POST")
					{
						requester.AppSensorDetectionPoint = AppSensorDetectionPointKind.AE10;
					}
					var additionalFormKeys = string.Join(",", additionalKeys);
					_logger.Information("{@controllerName} {@methodName} {@httpMethod} additional form keys {additionalFormKeys} sent by requester {@requester}",
						controllerName, methodName, httpMethod, additionalFormKeys, requester);
				}
				// Check if any fields are missing from request
				var missingKeys = ExpectedFormKeys.Except(keysSent).ToList();
				if (missingKeys.Count > 0)
				{
					var requester = _userIdentity.GetRequester((Controller)controller, AppSensorDetectionPointKind.RE6);
					if (controllerName == "Account" && methodName == "LogOn" && httpMethod == "POST")
					{
						requester.AppSensorDetectionPoint = AppSensorDetectionPointKind.AE11;
					}
					var missingFormKeys = string.Join(",", missingKeys);
					_logger.Information("{@controllerName} {@methodName} {@httpMethod} missing form keys {missingFormKeys} sent by requester {@requester}",
						controllerName, methodName, httpMethod, missingFormKeys, requester);
				}
				// Check for potential SQL Injection Comments
				foreach (var keySent in keysSent)
				{
					var valuesSent = filterContext.HttpContext.Request.Form.GetValues(keySent);
					foreach (var valueSent in valuesSent)
					{
						if (Regex.Match(valueSent, @"\*!?|\*|[';]--|--[\s\r\n\v\f]|(?:--[^-]*?-)|([^\-&])#.*?[\s\r\n\v\f]|;?\\x00").Success)
						{
							var requester = _userIdentity.GetRequester((Controller)controller, AppSensorDetectionPointKind.CIE1);
							_logger.Information("{@controllerName} {@methodName} {@httpMethod} SQL injection sent in form submission {@valueSent} by requester {@requester}",
								controllerName, methodName, httpMethod, valueSent, requester);
						}

					}
				}
			}
		}

	}
}