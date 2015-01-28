using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SecurityEssentials.Models;
using SecurityEssentials.ViewModel;

namespace SecurityEssentials.Controllers
{
    public class UserController : AntiForgeryControllerBase
    {

		#region Edit

		/// <summary>
		/// 
		/// </summary>
		/// <param name="id">Unique identifier for the user</param>
		/// <returns></returns>
		public ActionResult Edit(int id)
		{
			using (var context = new UsersContext())
			{
				var users = context.User.Where(u => u.Id == id);
				if (users == null) return new HttpNotFoundResult();
				var user = users.FirstOrDefault();

				var role = context.User.Where(u => u.Id == id)
					.FirstOrDefault()
					.UserRoles.ToList();

				return View(new UserViewModel(Convert.ToInt32(User.Identity.Name), user));
			}
		}

		[HttpPost]
		public ActionResult Edit(int id, FormCollection collection)
		{
			using (var context = new UsersContext())
			{

			var users = context.User.Where(u => u.Id == id);
			if (users == null) return new HttpNotFoundResult();
			var user = users.FirstOrDefault();

			var propertiesToUpdate = new[]
                {
                    "Email", "FirstName", "LastName", "TelNoHome", "TelNoMobile", "TelNoWork", "Title",
                    "UserName", "Town","Postcode","SkypeName"
                };

			if (TryUpdateModel(user, "User", propertiesToUpdate, collection))
			{
				context.SaveChanges();
				return RedirectToAction("Index", "User");
			}

			return View(new UserViewModel(Convert.ToInt32(User.Identity.Name), user));
			}
		}

		#endregion





    }
}
