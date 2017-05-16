using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Helpers;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Mvc;

// For use with WebAPI
// See http://aspnet13.orcsweb.com/web-api/overview/security/preventing-cross-site-request-forgery-(csrf)-attacks
// taken from http://www.asp.net/single-page-application/overview/templates/breezeknockout-template
namespace SecurityEssentials.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public sealed class ValidateHttpAntiForgeryTokenAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (actionContext == null) throw new ArgumentNullException(nameof(actionContext));
            var request = actionContext.ControllerContext.Request;
            try
            {
                IEnumerable<string> tokenHeaders;
                if (request.Headers.TryGetValues("RequestVerificationToken", out tokenHeaders))
                    ValidateRequestHeader(tokenHeaders);
                else
                    AntiForgery.Validate();
            }
            catch (HttpAntiForgeryException e)
            {
                actionContext.Response = request.CreateErrorResponse(HttpStatusCode.Forbidden, e);
            }
        }

        private static void ValidateRequestHeader(IEnumerable<string> tokenHeaders)
        {
            var cookieToken = string.Empty;
            var formToken = string.Empty;
            var tokenValue = tokenHeaders.FirstOrDefault();
            if (!string.IsNullOrEmpty(tokenValue))
            {
                var tokens = tokenValue.Split(':');
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