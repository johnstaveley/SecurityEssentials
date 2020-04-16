using SecurityEssentials.Core.Attributes;
using System.Web.Mvc;

namespace SecurityEssentials.Controllers
{
	[NoCache]
	public class WebPageContentController : Controller
	{

		[AllowAnonymous]
		[HttpGet]
		public ViewResult TooManyRequests()
		{
			return View();
		}
	}
}