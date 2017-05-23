using System.Web.Mvc;

namespace SecurityEssentials.Controllers
{
	public class WebPageContentController : Controller
	{

		[AllowAnonymous]
		public ViewResult TooManyRequests()
		{
			return View();
		}
	}
}