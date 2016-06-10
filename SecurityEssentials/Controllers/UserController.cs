using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SecurityEssentials.Model;
using SecurityEssentials.Core;
using SecurityEssentials.ViewModel;
using Microsoft.AspNet.Identity;

namespace SecurityEssentials.Controllers
{

	[Authorize]
	public class UserController : AntiForgeryControllerBase
	{

		#region Declarations

		private ISEContext _context { get; set; }

        #endregion

        #region Constructor

        public UserController() : this(new SEContext())
        {

        }


		public UserController(ISEContext context)
		{
            if (context == null) throw new ArgumentNullException("context");

            _context = context;
		}

		#endregion

		#region Disable

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		/// <param name="id">The unique identifier for the user</param>
		/// <remarks>GET: /User/Disable/5</remarks>
		[Authorize(Roles = "Admin")]
        public ActionResult Disable(int id)
		{
			User user = _context.User.Where(u => u.Id == id).FirstOrDefault();
			if (user == null) return new HttpNotFoundResult();
			return PartialView("_Disable", user);
		}

		/// <summary>
		///  
		/// </summary>
		/// <returns></returns>
		/// <param name="id">The unique identifier of the User to disable</param>
		/// <remarks>POST: /User/Disable/5</remarks>
		[HttpPost, Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public JsonResult Disable(int id, FormCollection collection)
		{
			if (id == 0) return Json(new { success = false, message = "unable to locate user id" });
			User user = _context.User.Where(u => u.Id == id).FirstOrDefault();
			if (user == null) return Json(new { success = false, message = "unable to locate user" });
			if (user.Id == Convert.ToInt32(User.Identity.GetUserId())) return Json(new { success = false, message = "You cannot disable your own account" });
			user.Enabled = false;
			_context.SaveChanges();
			return Json(new { success = true, message = "" });
		}

		#endregion

		#region ChangeEmailAddress

		/// <summary>
		/// 
		/// </summary>
		/// <param name="id">Unique identifier for the user</param>
		/// <returns></returns>
		public ActionResult ChangeEmailAddress(int id)
		{
			using (var context = new SEContext())
			{
				var users = context.User.Where(u => u.Id == id);
				var currentUser = Convert.ToInt32(User.Identity.GetUserId());
				if (users.ToList().Count == 0) return new HttpNotFoundResult();
				var user = users.FirstOrDefault();
				// SECURE: Check user should have access to this account
				if (!User.IsInRole("Admin") && currentUser != user.Id) return new HttpNotFoundResult();
				return View(new UserViewModel(currentUser, User.IsInRole("Admin"), user));
			}
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
				var currentUser = Convert.ToInt32(User.Identity.GetUserId());
				if (users.ToList().Count == 0) return new HttpNotFoundResult();
				var user = users.FirstOrDefault();
				// SECURE: Check user should have access to this account
				if (!User.IsInRole("Admin") && currentUser != user.Id) return new HttpNotFoundResult();
				return View(new UserViewModel(currentUser, User.IsInRole("Admin"), user));
			}
		}

		[HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, FormCollection collection)
		{
			using (var context = new SEContext())
			{

				var users = context.User.Where(u => u.Id == id);
				if (users.ToList().Count == 0) return new HttpNotFoundResult();
				var user = users.FirstOrDefault();
				var currentUser = Convert.ToInt32(User.Identity.GetUserId());
				// SECURE: Check user should have access to this account
				if (!User.IsInRole("Admin") && currentUser != user.Id) return new HttpNotFoundResult();

				var propertiesToUpdate = new List<string>()
                {
                    "FirstName", "LastName", "TelNoHome", "TelNoMobile", "TelNoWork", "Title",
                    "Town","Postcode", "SkypeName"
                };
				if (User.IsInRole("Admin"))
				{
					propertiesToUpdate.Add("Approved");
					propertiesToUpdate.Add("Enabled");
					propertiesToUpdate.Add("UserName");
				}
				if (TryUpdateModel(user, "User", propertiesToUpdate.ToArray(), collection))
				{
					if (user.Id == currentUser && user.Enabled == false)
					{
						ModelState.AddModelError("", "You cannot disable your own user account");
					}
					else
					{
						context.SaveChanges();
						return RedirectToAction("Index", "User");
					}
				}

				return View(new UserViewModel(currentUser, User.IsInRole("Admin"), user ));
			}
		}

		#endregion

		#region Index

		[Authorize(Roles = "Admin")]
		public ActionResult Index(int page = 1)
		{
			return View(new UsersViewModel(Convert.ToInt32(User.Identity.GetUserId())));
		}

		#endregion

		#region Log

		/// <summary>
		/// 
		/// </summary>
		/// <param name="id">Unique identifier for the user</param>
		/// <returns></returns>
		public ActionResult Log(int id)
		{
			using (var context = new SEContext())
			{
				var users = context.User.Where(u => u.Id == id);
				var currentUser = Convert.ToInt32(User.Identity.GetUserId());
				if (users.ToList().Count == 0) return new HttpNotFoundResult();
				var user = users.FirstOrDefault();
				// SECURE: Check user should have access to this account
				if (!User.IsInRole("Admin") && currentUser != user.Id) return new HttpNotFoundResult();
				ViewBag.UserName = user.UserName;
				return View(user.UserLogs.OrderByDescending(ul => ul.DateCreated).Take(10).ToList());
			}
		}

		#endregion

		#region Read

		[Authorize(Roles = "Admin")]
		public JsonResult Read(int page = 0, int pageSize = 20, string searchText = "")
		{
			string sortDirection = Request["sort[0][dir]"];
			string sortField = Request["sort[0][field]"];

			var users = _context.User.Where(
				u => (searchText == "" ||
						(
						(!string.IsNullOrEmpty(u.FirstName) && u.FirstName.Contains(searchText)) ||
						(!string.IsNullOrEmpty(u.LastName) && u.LastName.Contains(searchText)) ||
						(!string.IsNullOrEmpty(u.UserName) && u.UserName.Contains(searchText))
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
				u.TelNoMobile,
				u.Enabled,
				u.Approved
			});

			return Json(new { total = recordCount, data = results }, JsonRequestBehavior.AllowGet);

		}

		#endregion

	}
}
