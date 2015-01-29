using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SecurityEssentials.Core;

namespace SecurityEssentials.Controllers
{
	public class SecurityController : AntiForgeryControllerBase
	{

		public ActionResult InformationDisclosure()
		{
			SEContext context = new SEContext();
			var user = context.User.Where(u => u.Id == 38).FirstOrDefault();
			user.LastName = "Bill";
			return View(user); // Will never get here
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
