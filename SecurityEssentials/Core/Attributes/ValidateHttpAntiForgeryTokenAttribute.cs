using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Helpers;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Mvc;

namespace SecurityEssentials.Core.Attributes
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public sealed class ValidateHttpAntiForgeryTokenAttribute : AuthorizationFilterAttribute
	{
		public override void OnAuthorization(HttpActionContext actionContext)
		{
			if (actionContext == null) throw new ArgumentNullException(nameof(actionContext));
			HttpRequestMessage request = actionContext.ControllerContext.Request;
			try
			{
				IEnumerable<string> tokenHeaders;
				if (request.Headers.TryGetValues("RequestVerificationToken", out tokenHeaders))
				{
					ValidateRequestHeader(tokenHeaders);
				}
				else
				{
					AntiForgery.Validate();
				}
			}
			catch (HttpAntiForgeryException e)
			{
				actionContext.Response = request.CreateErrorResponse(HttpStatusCode.Forbidden, e);
			}
		}

		private static void ValidateRequestHeader(IEnumerable<string> tokenHeaders)
		{
			string cookieToken = String.Empty;
			string formToken = String.Empty;
			string tokenValue = tokenHeaders.FirstOrDefault();
			if (!String.IsNullOrEmpty(tokenValue))
			{
				string[] tokens = tokenValue.Split(':');
				if (tokens.Length == 2)
				{
					cookieToken = tokens[0].Trim();
					formToken = tokens[1].Trim();
				}
			}
			AntiForgery.Validate(cookieToken, formToken);
		}
	}
}