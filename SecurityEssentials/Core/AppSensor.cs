using SecurityEssentials.Core.Constants;
using SecurityEssentials.Core.Identity;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace SecurityEssentials.Core
{
	public class AppSensor : IAppSensor
	{

		private IUserIdentity _userIdentity;
		private ILogger _logger;

		public AppSensor()
			: this(new UserIdentity(), Log.Logger)
		{
		}

		public AppSensor(IUserIdentity userIdentity, ILogger logger)
		{
			if (logger == null) throw new ArgumentNullException("logger");
			if (userIdentity == null) throw new ArgumentNullException("userIdentity");

			_logger = logger;
			_userIdentity = userIdentity;
		}

		/// <summary>
		/// Validate if the user has attempted to over or under supply fields to the application
		/// </summary>
		/// <param name="controller"></param>
		/// <param name="expectedFormKeys"></param>
		public void ValidateFormData(Controller controller, List<string> expectedFormKeys)
		{
			var keysSent = controller.Request.Form.AllKeys.ToList();
			var controllerMethod = controller.Request.CurrentExecutionFilePath.Trim('~').Trim('/').Split('/');
			var controllerName = controllerMethod[0];
			var methodName = controllerMethod[1];
			var httpMethod = controller.Request.HttpMethod;
			if (!expectedFormKeys.Contains("__RequestVerificationToken") && (httpMethod == "POST" || httpMethod == "PUT")) expectedFormKeys.Add("__RequestVerificationToken");
			// Check if any additional fields have been provided
			var additionalKeys = keysSent.Except(expectedFormKeys).ToList();
			if (additionalKeys.Count > 0)
			{
				var requester = _userIdentity.GetRequester(controller, AppSensorDetectionPointKind.RE5);
				if (controllerName == "Account" && methodName == "LogOn" && httpMethod == "POST")
				{
					requester.AppSensorDetectionPoint = AppSensorDetectionPointKind.AE10;
				}
				var additionalFormKeys = string.Join(",", additionalKeys);
				_logger.Information("{@controllerName} {@methodName} {@httpMethod} additional form keys {additionalFormKeys} sent by requester {@requester}",
					controllerName, methodName, httpMethod, additionalFormKeys, requester);
			}
			// Check if any fields are missing from request
			var missingKeys = expectedFormKeys.Except(keysSent).ToList();
			if (missingKeys.Count > 0)
			{
				var requester = _userIdentity.GetRequester(controller, AppSensorDetectionPointKind.RE6);
				if (controllerName == "Account" && methodName == "LogOn" && httpMethod == "POST")
				{
					requester.AppSensorDetectionPoint = AppSensorDetectionPointKind.AE11;
				}
				var missingFormKeys = string.Join(",", missingKeys);
				_logger.Information("{@controllerName} {@methodName} {@httpMethod} missing form keys {missingFormKeys} sent by requester {@requester}", 
					controllerName, methodName, httpMethod, missingFormKeys, requester);
			}
		}

		/// <summary>
		/// Detect if user has bypassed front end validation to try and get some illegal content into the database
		/// </summary>
		/// <param name="controller"></param>
		public void InspectModelStateErrors(Controller controller)
		{
			// Assumption is that javascript is turned on on the client
			var allErrors = controller.ModelState.Values.SelectMany(v => v.Errors).ToList();
			Requester requester = _userIdentity.GetRequester(controller);
			var controllerMethod = controller.Request.CurrentExecutionFilePath.Trim('~').Trim('/').Split('/');
			var controllerName = controllerMethod[0];
			var methodName = controllerMethod[1];
			var httpMethod = controller.Request.HttpMethod;
			foreach (var error in allErrors)
			{
				var errorMessage = error.ErrorMessage;
				requester.AppSensorDetectionPoint = null;
				// Required field
				if (errorMessage.Contains("is required"))
				{
					requester.AppSensorDetectionPoint = Core.Constants.AppSensorDetectionPointKind.RE6;
				}
				// Field with validity determined by Regex
				if (errorMessage.Contains("does not appear to be valid"))
				{
					requester.AppSensorDetectionPoint = Core.Constants.AppSensorDetectionPointKind.IE2;
				}
				// Field with maximum and minimum length
				if (errorMessage.Contains("with a maximum length of") || error.ErrorMessage.Contains("with a minimum length of"))
				{
					requester.AppSensorDetectionPoint = Core.Constants.AppSensorDetectionPointKind.RE7;
				}
				// Field with StringLength attribute and custom message
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
				_logger.Information("Failed {@controllerName} {@methodName} {@httpMethod} validation bypass {errorMessage} attempted by user {@requester}", controllerName, methodName, httpMethod, errorMessage, requester);
			}
		}

	}
}