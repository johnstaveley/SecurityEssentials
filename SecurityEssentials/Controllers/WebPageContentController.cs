using SecurityEssentials.Core.Attributes;
using System.Web.Mvc;

namespace SecurityEssentials.Controllers
{
	[NoCache]
	public class WebPageContentController : Controller
	{

		[AllowAnonymous]
		public ViewResult TooManyRequests()
		{
			return View();
		}
	}
}