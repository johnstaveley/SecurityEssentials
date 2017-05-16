using System;
using System.Linq;
using System.Web.Mvc;
using SecurityEssentials.Core.Identity;
using Serilog;

namespace SecurityEssentials.Core.Attributes
{
    public class SEAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly IUserIdentity _userIdentity;

        public SEAuthorizeAttribute() : this(new UserIdentity())
        {
            // TODO: Replace with your DI Framework of choice
        }

        public SEAuthorizeAttribute(IUserIdentity userIdentity)
        {
            if (userIdentity == null) throw new ArgumentNullException("userIdentity");

            _userIdentity = userIdentity;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            var action = filterContext.ActionDescriptor.ActionName;
            var controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            var requester = _userIdentity.GetRequester(filterContext.Controller as Controller, null);
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                // The user is not authenticated
                Log.Logger.Information(
                    "Failed access attempt on controller {controller} and action {action} which required authorization by requester {@requester}",
                    controller, action, requester);
                base.HandleUnauthorizedRequest(filterContext);
            }
            else if (!Roles.Split(',').Any(filterContext.HttpContext.User.IsInRole))
            {
                // The user is not in any of the listed roles then log and show the unauthorized view
                Log.Logger.Information(
                    "Failed access attempt on controller {controller} and action {action} which required roles {roles} by requester {@requester}",
                    controller, action, Roles, requester);
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