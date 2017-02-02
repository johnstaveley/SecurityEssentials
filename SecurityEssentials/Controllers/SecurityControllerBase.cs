using SecurityEssentials.Core;
using SecurityEssentials.Core.Attributes;
using SecurityEssentials.Core.Identity;
using Serilog;
using System;
using System.Linq;
using System.Web.Mvc;

namespace SecurityEssentials.Controllers
{
	[ExceptionHandler]
    public abstract class SecurityControllerBase : Controller
    {

        private readonly ValidateAntiForgeryTokenAttribute _validator;
        private readonly AcceptVerbsAttribute _verbs;
		public ILogger Logger;
		protected IUserIdentity _userIdentity;
		protected IAppSensor _appSensor;

        protected SecurityControllerBase() : this(new UserIdentity(), new AppSensor())
        {
			// TODO: replace with DI container of choice
        }

        protected SecurityControllerBase(IUserIdentity userIdentity, IAppSensor appSensor)
        {
			if (appSensor == null) throw new ArgumentNullException("appSensor");
			if (userIdentity == null) throw new ArgumentNullException("userIdentity");
            _verbs = new AcceptVerbsAttribute(HttpVerbs.Post);
            _validator = new ValidateAntiForgeryTokenAttribute();
			Logger = Log.Logger;
			_userIdentity = userIdentity;
			_appSensor = appSensor;

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

	}
}