using SecurityEssentials.Core;
using SecurityEssentials.Core.Attributes;
using SecurityEssentials.Core.Identity;
using Serilog;
using System;
using System.Web.Mvc;

namespace SecurityEssentials.Controllers
{
	[ExceptionHandler, AccountManagement]
	public abstract class SecurityControllerBase : Controller
	{

		public ILogger Logger { get; set; }
		protected IUserIdentity UserIdentity { get; set; }
		protected IAppSensor AppSensor { get; set; }

		protected SecurityControllerBase(IUserIdentity userIdentity, IAppSensor appSensor)
		{
			Logger = Log.Logger;
			UserIdentity = userIdentity ?? throw new ArgumentNullException(nameof(userIdentity));
			AppSensor = appSensor ?? throw new ArgumentNullException(nameof(appSensor));

		}

		protected override void OnAuthorization(AuthorizationContext filterContext)
		{
			if (filterContext == null) throw new ArgumentNullException(nameof(filterContext));
			base.OnAuthorization(filterContext);
		}

	}
}