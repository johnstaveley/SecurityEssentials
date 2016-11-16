using SecurityEssentials.Core;
using SecurityEssentials.Core.Identity;
using System;
using System.Web.Mvc;

namespace SecurityEssentials.Controllers
{
	public class ErrorController : AntiForgeryControllerBase
    {

		public ErrorController() : this(new UserIdentity())
		{
			// TODO: Replace with your DI Framework of choice
		}

		public ErrorController(IUserIdentity userIdentity)
		{
			if (userIdentity == null) throw new ArgumentNullException("userIdentity");

			_userIdentity = userIdentity;
		}



		// GET: Error
		public ActionResult NotFound()
        {
			ActionResult result;

			object model = Request.Url.PathAndQuery;

			if (!Request.IsAjaxRequest())
			{
				result = View(model);
			}
			else
			{
				result = PartialView("_NotFound", model);
			}
			Response.StatusCode = 404;
			Response.TrySkipIisCustomErrors = true;
			var appSensorDetectionPoint = Core.Constants.AppSensorDetectionPointKind.RE1;
			// TODO: Determine if path exists, if so RE2, otherwise RE1
			Requester requestor = _userIdentity.GetRequester(this, appSensorDetectionPoint);			
			Logger.Information("Unknown route {CurrentExecutionFilePath} accessed by user {@requestor}", Request.CurrentExecutionFilePath, requestor);
			return result;
		}

		// GET: Error
		public ActionResult Index()
		{
			ActionResult result;

			object model = Request.Url.PathAndQuery;

			if (!Request.IsAjaxRequest())
			{
				result = View(model);
			}
			else
			{
				result = PartialView("_Index", model);
			}
			Response.StatusCode = 500;
			Response.TrySkipIisCustomErrors = true;
			Requester requestor = _userIdentity.GetRequester(this, null);
			Logger.Error(Server.GetLastError(), "Error occurred by user {@requestor}", requestor);
			return result;
		}

	}
}