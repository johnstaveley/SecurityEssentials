using SecurityEssentials.Core.Attributes;
using SecurityEssentials.Model;
using System.Web.Mvc;

namespace SecurityEssentials.Controllers
{

	public class SecurityController : Controller
	{

		[HttpPost]
		[AllowXRequestsEveryXSeconds(Name = "CspReporting", ContentName = "TooManyRequests", Requests = 10, Seconds = 60)]
		public JsonResult CspReporting(CspReport data)
		{
			Serilog.Log.Logger.Warning($"Content Security Policy Violation {data}");
			return Json(new {success = true});
		}
		
	}
}
