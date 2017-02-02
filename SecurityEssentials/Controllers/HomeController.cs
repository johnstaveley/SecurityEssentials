using SecurityEssentials.Core;
using SecurityEssentials.Core.Identity;
using System.Web.Mvc;

namespace SecurityEssentials.Controllers
{
	public class HomeController : SecurityControllerBase
	{

		public HomeController() : this(new UserIdentity(), new AppSensor())
		{
			// TODO: Replace with your DI Framework of choice
		}

		public HomeController(IUserIdentity userIdentity, IAppSensor appSensor) : base (userIdentity, appSensor)
		{

		}

		public ActionResult Index()
		{
			ViewBag.Message = "Security Essentials";
			return View("Index");
		}

		public ActionResult About()
		{
			ViewBag.Message = "";
			return View("About");
		}

		public ActionResult Contact()
		{
			ViewBag.Message = "Your contact page.";
			return View("Contact");
		}

	}
}
