using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SecurityEssentials.Model;
using SecurityEssentials.Core;
using SecurityEssentials.ViewModel;

namespace SecurityEssentials.Controllers
{
    public class UserController : AntiForgeryControllerBase
	{

		#region Declarations

		private SEContext context { get; set; }

		#endregion

		#region Constructor

		public UserController()
		{
			context = new SEContext();
		}

		#endregion

		#region Delete

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		/// <param name="id">The unique identifier for the user</param>
		/// <remarks>GET: /User/Disable/5</remarks>
		public ActionResult Disable(int id)
		{
			User user = context.User.Where(u => u.Id == id).FirstOrDefault();
			if (user == null) return new HttpNotFoundResult();
			return PartialView("_Disable", user);
		}

		/// <summary>
		///  
		/// </summary>
		/// <returns></returns>
		/// <param name="id">The unique identifier of the User to disable</param>
		/// <remarks>POST: /User/Disable/5</remarks>
		[HttpPost]
		public JsonResult Disable(int id, FormCollection collection)
		{
			if (id == 0) return Json(new { success = false, message = "unable to locate user id" });
			User user = context.User.Where(u => u.Id == id).FirstOrDefault();
			if (user == null) return Json(new { success = false, message = "unable to locate user" });
			if (user.UserName == User.Identity.Name) return Json(new { success = false, message = "You cannot disable your own account" });
			user.Enabled = false;
			context.SaveChanges();
			return Json(new { success = true, message = "" });
		}

		#endregion

		#region Edit

		/// <summary>
		/// 
		/// </summary>
		/// <param name="id">Unique identifier for the user</param>
		/// <returns></returns>
		public ActionResult Edit(int id)
		{
			using (var context = new SEContext())
			{
				var users = context.User.Where(u => u.Id == id);
				var currentUser = 1;
				//var currentUser = Convert.ToInt32(User.Identity.Name);
				if (users == null) return new HttpNotFoundResult();
				var user = users.FirstOrDefault();

				var role = context.User.Where(u => u.Id == id)
					.FirstOrDefault()
					.UserRoles.ToList();

				return View(new UserViewModel(currentUser, user));
			}
		}

		[HttpPost]
		public ActionResult Edit(int id, FormCollection collection)
		{
			using (var context = new SEContext())
			{

			var users = context.User.Where(u => u.Id == id);
			if (users == null) return new HttpNotFoundResult();
			var user = users.FirstOrDefault();
			var currentUser = 1;
			//var currentUser = Convert.ToInt32(User.Identity.Name);

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

			return View(new UserViewModel(currentUser, user));
			}
		}

		#endregion

		#region Index

		public ActionResult Index(int page = 1)
		{
			return View(new UsersViewModel());

		}

		#endregion

		#region Read

		public JsonResult Read(int page = 0, int pageSize = 20, string searchText = "")
		{
			string sortDirection = Request["sort[0][dir]"];
			string sortField = Request["sort[0][field]"];

			var users = context.User.Where(
				u => (searchText == "" ||
						(
						(!string.IsNullOrEmpty(u.FirstName) && u.FirstName.Contains(searchText)) ||
						(!string.IsNullOrEmpty(u.LastName) && u.LastName.Contains(searchText)) ||
						(!string.IsNullOrEmpty(u.Email) && u.Email.Contains(searchText))
						)
					));

			if (string.IsNullOrEmpty(sortField))
			{
				// set default sorting of users
				sortField = "LastName";
				sortDirection = "Ascending";
			}

			// apply sorting
			if (!string.IsNullOrWhiteSpace(sortField) && !string.IsNullOrWhiteSpace(sortDirection))
			{
				sortField = sortField.Replace("_", ".");
				users = users.OrderBy(string.Format("{0} {1}", sortField, sortDirection));
			}

			var recordCount = users.Count();

			var results = users.Skip(pageSize * (page - 1)).Take(pageSize).ToList().Select(u => new
			{
				u.Id,
				u.UserName,
				u.FullName,
				u.Email,
				u.TelNoMobile,
				u.Enabled
			});

			return Json(new { total = recordCount, data = results }, JsonRequestBehavior.AllowGet);

		}


		#endregion



    }
}
