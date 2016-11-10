using System.Web.Mvc;

namespace SecurityEssentials.Controllers
{
	public class HomeController : AntiForgeryControllerBase
	{
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
