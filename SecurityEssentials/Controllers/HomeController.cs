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

			return View();
		}

		public ActionResult Landing()
		{
			var currentUserId = Convert.ToInt32(User.Identity.GetUserId());
			using (var context = new SEContext())
			{
				var users = context.User.Where(u => u.Id == currentUserId);
				if (users.ToList().Count == 0) return new HttpNotFoundResult();
				var user = users.FirstOrDefault();
				var activityLogs = user.UserLogs.OrderByDescending(d => d.DateCreated);
				UserLog lastAccountActivity = null;
				if (activityLogs.ToList().Count > 1)
				{
					lastAccountActivity = activityLogs.Skip(1).FirstOrDefault();
				}
				return View(new LandingViewModel(user.FirstName, lastAccountActivity, currentUserId));
			}
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

	}
}
