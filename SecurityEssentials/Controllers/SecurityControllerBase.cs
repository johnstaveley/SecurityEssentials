using System;
using System.Linq;
using System.Web.Mvc;
using SecurityEssentials.Core;
using SecurityEssentials.Core.Attributes;
using SecurityEssentials.Core.Identity;
using Serilog;

namespace SecurityEssentials.Controllers
{
    [ExceptionHandler]
    public abstract class SecurityControllerBase : Controller
    {
        private readonly ValidateAntiForgeryTokenAttribute _validator;
        private readonly AcceptVerbsAttribute _verbs;
        protected IAppSensor AppSensor;
        protected IUserIdentity UserIdentity;
        public ILogger Logger;

        protected SecurityControllerBase() : this(new UserIdentity(), new AppSensor())
        {
            // TODO: replace with DI container of choice
        }

        protected SecurityControllerBase(IUserIdentity userIdentity, IAppSensor appSensor)
        {
            if (appSensor == null) throw new ArgumentNullException(nameof(appSensor));
            if (userIdentity == null) throw new ArgumentNullException(nameof(userIdentity));
            _verbs = new AcceptVerbsAttribute(HttpVerbs.Post);
            _validator = new ValidateAntiForgeryTokenAttribute();
            Logger = Log.Logger;
            UserIdentity = userIdentity;
            AppSensor = appSensor;
        }

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null) throw new ArgumentNullException(nameof(filterContext));
            base.OnAuthorization(filterContext);

            var httpMethodOverride = filterContext.HttpContext.Request.GetHttpMethodOverride();
            if (_verbs.Verbs.Contains(httpMethodOverride, StringComparer.OrdinalIgnoreCase))
                _validator.OnAuthorization(filterContext);
        }
    }
}