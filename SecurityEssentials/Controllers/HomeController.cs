using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using SecurityEssentials.Core;
using SecurityEssentials.Model;
using SecurityEssentials.ViewModel;

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
