using SecurityEssentials.Core.Attributes;
using Serilog;
using System;
using System.Linq;
using System.Web.Mvc;

namespace SecurityEssentials.Controllers
{
	[ExceptionHandler]
    public abstract class AntiForgeryControllerBase : Controller
    {

        #region AntiForgeryControllerBase

        private readonly ValidateAntiForgeryTokenAttribute _validator;
        private readonly AcceptVerbsAttribute _verbs;
		public ILogger Logger;

        #endregion

        #region Constructor

        protected AntiForgeryControllerBase() : this(HttpVerbs.Post)
        {
        }

        protected AntiForgeryControllerBase(HttpVerbs verbs)
        {
            _verbs = new AcceptVerbsAttribute(verbs);
            _validator = new ValidateAntiForgeryTokenAttribute();
			Logger = Log.Logger;
        }

        #endregion

        #region OnAuthorization

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

        #endregion

    }
}