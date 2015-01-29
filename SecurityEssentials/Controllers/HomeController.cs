using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SecurityEssentials.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			ViewBag.Message = "Security Essentials";

			return View();
		}

		public ActionResult About()
		{
			ViewBag.Message = "";

			return View();
		}

		public ActionResult Contact()
		{
			ViewBag.Message = "Your contact page.";

			return View();
		}

		public ActionResult AntiXSS()
		{

			ViewBag.CssTest = "red";
			ViewBag.HtmlTest = "<b>If unencoded, this text should appear bold</b>";
			ViewBag.JavaScriptTest = "hello";
//			ViewBag.JavaScriptTest = "hello'); alert('hello";
			return View();
		}

	}
}
