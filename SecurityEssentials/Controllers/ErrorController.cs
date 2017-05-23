using SecurityEssentials.Core;
using SecurityEssentials.Core.Identity;
using System.Web.Mvc;

namespace SecurityEssentials.Controllers
{
	public class ErrorController : SecurityControllerBase
    {
		
		public ErrorController(IUserIdentity userIdentity, IAppSensor appSensor) : base (userIdentity, appSensor)
		{
			
		}

		// GET: Error
		public ActionResult NotFound()
        {
			ActionResult result;

			object model = Request.Url?.PathAndQuery;

			if (!Request.IsAjaxRequest())
			{
				result = View("NotFound", model);
			}
			else
			{
				result = PartialView("_NotFound", model);
			}
			Response.StatusCode = 404;
			Response.TrySkipIisCustomErrors = true;
			var appSensorDetectionPoint = Core.Constants.AppSensorDetectionPointKind.Re1;
			// TODO: Determine if path exists, if so RE2, otherwise RE1
			Requester requester = UserIdentity.GetRequester(this, appSensorDetectionPoint);
			var currentExecutionFilePath = Request.CurrentExecutionFilePath;
			Logger.Information("Unknown route {currentExecutionFilePath} accessed by user {@requester}", currentExecutionFilePath, requester);
			return result;
		}

		// GET: Error
		public ActionResult Forbidden()
		{
			ActionResult result;

			object model = Request.Url?.PathAndQuery;

			if (!Request.IsAjaxRequest())
			{
				result = View("Forbidden", model);
			}
			else
			{
				result = PartialView("_Forbidden", model);
			}
			Response.StatusCode = 403;
			Response.TrySkipIisCustomErrors = true;
			Requester requester = UserIdentity.GetRequester(this);
			var currentExecutionFilePath = Request.CurrentExecutionFilePath;
			if (Server.GetLastError() is HttpAntiForgeryException)
			{
				Logger.Information("Forbidden request, attempted CSRF {currentExecutionFilePath} accessed by user {@requester}", currentExecutionFilePath, requester);
			}
			else
			{
				Logger.Information("Forbidden request {currentExecutionFilePath} accessed by user {@requester}", currentExecutionFilePath, requester);
			}
			return result;
		}

		// GET: Error
		public ActionResult Index()
		{
			ActionResult result;

			object model = Request.Url?.PathAndQuery;

			if (!Request.IsAjaxRequest())
			{
				result = View("Index", model);
			}
			else
			{
				result = PartialView("_Index", model);
			}
			Response.StatusCode = 500;
			Response.TrySkipIisCustomErrors = true;
			Requester requestor = UserIdentity.GetRequester(this);
			Logger.Error(Server.GetLastError(), "Error occurred by user {@requestor}", requestor);
			return result;
		}

	}
}