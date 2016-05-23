using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SecurityEssentials.Core;
using Microsoft.AspNet.Identity;
using SecurityEssentials.ViewModel;

namespace SecurityEssentials.Controllers
{
	public class SecurityController : AntiForgeryControllerBase
	{

#if DEBUG

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


        #region Edit

        public ActionResult OverPostingEdit(int id)
		{
			using (var context = new SEContext())
			{
				var users = context.User.Where(u => u.Id == id);
				var currentUser = Convert.ToInt32(User.Identity.GetUserId());
				var user = users.FirstOrDefault();
				return View(new UserViewModel(currentUser, User.IsInRole("Admin"), user));
			}
		}

		[HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OverPostingEdit(int id, FormCollection collection)
		{
			using (var context = new SEContext())
			{

				var users = context.User.Where(u => u.Id == id);
				if (users.ToList().Count == 0) return new HttpNotFoundResult();
				var user = users.FirstOrDefault();
				var currentUser = Convert.ToInt32(User.Identity.GetUserId());

				var propertiesToUpdate = new List<string>()
				//{
				//	"FirstName", "LastName", "TelNoHome", "TelNoMobile", "TelNoWork", "Title",
				//	"Town","Postcode", "SkypeName"
				//}
				;
				if (TryUpdateModel(user, "User", propertiesToUpdate.ToArray(), collection))
				{
						context.SaveChanges();
						ViewBag.Notification = "Details updated";
						return View("OverPostingEdit");
				}

				return View(new UserViewModel(currentUser, User.IsInRole("Admin"), user));
			}
		}

		#endregion
#endif
	}

}
