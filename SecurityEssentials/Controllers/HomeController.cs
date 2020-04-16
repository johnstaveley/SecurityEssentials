using SecurityEssentials.Core;
using SecurityEssentials.Core.Identity;
using System.Web.Mvc;

namespace SecurityEssentials.Controllers
{
	public class HomeController : SecurityControllerBase
	{
		
		public HomeController(IUserIdentity userIdentity, IAppSensor appSensor) : base (userIdentity, appSensor)
		{

		}

		[HttpGet]
		public ActionResult Index()
		{
			ViewBag.Message = "Security Essentials";
			return View("Index");
		}

		[HttpGet]
		public ActionResult About()
		{
			ViewBag.Message = "";
			return View("About");
		}

		[HttpGet]
		public ActionResult Contact()
		{
			ViewBag.Message = "Your contact page.";
			return View("Contact");
		}

	}
}
