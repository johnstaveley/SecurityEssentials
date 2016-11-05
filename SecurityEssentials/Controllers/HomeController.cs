using System.Web.Mvc;

namespace SecurityEssentials.Controllers
{
	public class HomeController : AntiForgeryControllerBase
	{
		public ActionResult Index()
		{
			ViewBag.Message = "Security Essentials";
			Logger.Debug("Entered Get Home.Index");

			return View("Index");
		}

		public ActionResult About()
		{
			ViewBag.Message = "";
			Logger.Debug("Entered Get Home.About");
			return View("About");
		}

		public ActionResult Contact()
		{
			ViewBag.Message = "Your contact page.";
			Logger.Debug("Entered Get Home.Contact");
			return View("Contact");
		}

	}
}
