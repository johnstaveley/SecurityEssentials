using SecurityEssentials.Core;
using SecurityEssentials.Core.Attributes;
using SecurityEssentials.Core.Constants;
using SecurityEssentials.Core.Identity;
using SecurityEssentials.Model;
using SecurityEssentials.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SecurityEssentials.Controllers
{

	[SeAuthorize]
    [NoCache]
	public class UserController : SecurityControllerBase
	{

		private readonly IAppConfiguration _configuration;
		private readonly ISeContext _context;
		private readonly IHttpCache _httpCache;
		private readonly IServices _services;
		private readonly IUserManager _userManager;

		public UserController(IAppSensor appSensor, IAppConfiguration configuration, ISeContext context, IHttpCache httpCache, IUserIdentity userIdentity, IUserManager userManager, IServices services) : base(userIdentity, appSensor)
		{
			_configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
			_context = context ?? throw new ArgumentNullException(nameof(context));
			_httpCache = httpCache ?? throw new ArgumentNullException(nameof(httpCache));
			_services = services ?? throw new ArgumentNullException(nameof(services));
			_userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
		}

		[HttpGet]
		[SeAuthorize(Roles = "Admin")]
		public ActionResult Delete(int id)
		{
			var isOwnProfile = UserIdentity.GetUserId(this) == id;
			ViewBag.Message = "";
			var user = _context.User
				.SingleOrDefault(a => a.Id == id && !isOwnProfile);
			if (user == null) return new HttpNotFoundResult();
			return View(user);
		}

		[HttpPost]
		[SeAuthorize(Roles = "Admin")]
		[ValidateAntiForgeryToken]
		public ActionResult Delete(int id, FormCollection collection)
		{
			var isOwnProfile = UserIdentity.GetUserId(this) == id;
			ViewBag.Message = "";
			var user = _context.User
				.Include("PreviousPasswords")
				.Include("UserLogs")
				.Include("UserRoles")
				.SingleOrDefault(a => a.Id == id && !isOwnProfile);
			if (user == null) return new HttpNotFoundResult();
			if (!user.CanBeDeleted)
			{
				ViewBag.Message = "This user has data associated with it and cannot be deleted";
				return View("Delete", user);
			}
			Logger.Warning($"User {user.UserName} was deleted by user {UserIdentity.GetUserName(this)}");
			user.PreviousPasswords.ToList().ForEach(a => { _context.SetDeleted(a); });
			user.UserLogs.ToList().ForEach(a => { _context.SetDeleted(a); });
			user.UserRoles.ToList().ForEach(a => { _context.SetDeleted(a); });
			_context.SetDeleted(user);
			_context.SaveChanges();
			return RedirectToAction("Index", "User");
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		/// <param name="id">The unique identifier for the user</param>
		/// <remarks>GET: /User/Disable/5</remarks>
		[SeAuthorize(Roles = "Admin")]
		[HttpGet]
		public ActionResult Disable(int id)
		{
			User user = _context.User.FirstOrDefault(u => u.Id == id);
			if (user == null)
			{
				var requester = UserIdentity.GetRequester(this);
				Logger.Information("Failed User Disable, user {id} did not exist by requester {@requester}", id, requester);
				return new HttpNotFoundResult();
			}
			return PartialView("_Disable", user);
		}

		/// <summary>
		///  
		/// </summary>
		/// <returns></returns>
		/// <remarks>POST: /User/Disable/5</remarks>
		[HttpPost, SeAuthorize(Roles = "Admin")]
		[ValidateAntiForgeryToken]
		public JsonResult Disable(int id, FormCollection collection)
		{
			if (id == 0) return Json(new { success = false, message = "unable to locate user id" });
			var requester = UserIdentity.GetRequester(this);
			User user = _context.User.FirstOrDefault(u => u.Id == id);
			if (user == null)
			{
				Logger.Information("Failed User Disable Post for id {id}, user did not exist by requester {@requester}", id, requester);
				return Json(new { success = false, message = "unable to locate user" });
			}
			if (user.Id == UserIdentity.GetUserId(this)) return Json(new { success = false, message = "You cannot disable your own account" });
			user.Enabled = false;
			_context.SaveChanges();
			Logger.Information("User Disable Post for id {id} suceeded, by requester {@requester}", id, requester);
			return Json(new { success = true, message = "" });
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="id">Unique identifier for the user</param>
		/// <returns></returns>
		[HttpGet]
		public ActionResult Edit(int id)
		{
			ViewBag.StatusMessage = "";
			var isAdmin = UserIdentity.IsUserInRole(this, "Admin");
			var currentUserId = UserIdentity.GetUserId(this);
			var users = _context.User
				.Include("UserRoles")
				.Where(u => u.Id == id);
			var user = users.Single();
			var requester = UserIdentity.GetRequester(this);
			// SECURE: Check user should have access to this account
			if (!isAdmin && currentUserId != user.Id)
			{
				Logger.Information("Failed User Edit Get, user modification was not permitted for access rights by requester {@requester}", requester);
				return new HttpNotFoundResult();
			}
			return View(new UserViewModel(currentUserId, isAdmin, user));
		}

		[HttpGet]
		public ActionResult EditCurrent()
		{
			return RedirectToAction("Edit", "User", new { Id = UserIdentity.GetUserId(this) });
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(int id, FormCollection collection)
		{

			var isAdmin = UserIdentity.IsUserInRole(this, "Admin");
			var currentUserId = UserIdentity.GetUserId(this);
			var isOwnProfile = currentUserId == id;
			var users = _context.User.Where(u => u.Id == id);
			if (users.ToList().Count == 0) return new HttpNotFoundResult();
			var user = users.Single();
			ViewBag.StatusMessage = "";
			var requester = UserIdentity.GetRequester(this);
			// SECURE: Check user should have access to this account
			if (!isAdmin && !isOwnProfile)
			{
				Logger.Information("Failed User Edit Post, user modification was not permitted for access rights by requester {@requester}", requester);
				return new HttpNotFoundResult();
			}
			ViewBag.StatusMessage = "";
			var previousUserName = user.UserName;
			var propertiesToUpdate = new List<string>
			{
				"User.FirstName", "User.LastName", "User.TelNoHome", "User.TelNoMobile", "User.TelNoWork", "User.Title",
				"User.Town","User.Postcode", "User.SkypeName"
			};
			var expectedFields = new List<string> { "IsOwnProfile", "IsCurrentUserAnAdmin", "User.Id" };
			if (isAdmin)
			{
				if (!isOwnProfile)
				{
					// Otherwise these fields will be disabled on the front page
					propertiesToUpdate.AddRange(new List<string> { "User.Approved", "User.EmailVerified", "User.Enabled" });
				}
				propertiesToUpdate.AddRange(new List<string> { "User.UserName" });
			}
			propertiesToUpdate.ForEach(a => expectedFields.Add(a));
			AppSensor.ValidateFormData(this, expectedFields);
			if (TryUpdateModel(user, "User", propertiesToUpdate.Select(a => a.Replace("User.", "")).ToArray(), collection))
			{
				if (_context.User.Any(a => a.Id != user.Id && user.UserName == a.UserName))
				{
					ModelState.AddModelError("User.UserName", "This username is already in use");
				}
				else
				{
					if (user.UserName != previousUserName)
					{
						user.UserLogs.Add(new UserLog
						{
							Description = $"Username/Email was changed from {previousUserName} by {UserIdentity.GetUserName(this)}"
						});
						string emailSubject = $"{_configuration.ApplicationName} - Change email address process completed";
						string emailBody = EmailTemplates.ChangeEmailAddressCompletedBodyText(user.FirstName, user.LastName, _configuration.ApplicationName, previousUserName, user.UserName);
						_services.SendEmail(_configuration.DefaultFromEmailAddress, new List<string> { user.UserName }, null, null, emailSubject, emailBody, true);
					}
					_context.SaveChanges();
					if (!isOwnProfile && isAdmin)
					{
						return RedirectToAction("Index", "User");
					}
					ViewBag.StatusMessage = "Your account information has been saved";
				}
			}
			else
			{
				AppSensor.InspectModelStateErrors(this);
			}

			return View("Edit", new UserViewModel(UserIdentity.GetUserId(this), isAdmin, user));

		}

		[SeAuthorize(Roles = "Admin")]
		[HttpGet]
		public ActionResult Index(int page = 1)
		{
			return View("Index", new UsersViewModel(UserIdentity.GetUserId(this)));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="id">Unique identifier for the user</param>
		/// <returns></returns>
		[HttpGet]
		public ActionResult Log(int id)
		{
			var isAdmin = UserIdentity.IsUserInRole(this, "Admin");
			var currentUserId = UserIdentity.GetUserId(this);
			var users = _context.User
				.Include("UserLogs")
				.Where(u => u.Id == id && (u.Id == currentUserId || isAdmin));
			if (users.ToList().Count == 0) return new HttpNotFoundResult();
			var user = users.Single();
			var requester = UserIdentity.GetRequester(this);
			// SECURE: Check user should have access to this account
			if (!UserIdentity.IsUserInRole(this, "Admin") && UserIdentity.GetUserId(this) != user.Id)
			{
				Logger.Information("Failed User Log Get, access not permitted by requester {@requester}", requester);
				return new HttpNotFoundResult();
			}
			ViewBag.UserName = user.UserName;
			return View(new UserLogViewModel(user));

		}

		[SeAuthorize(Roles = "Admin")]
		[HttpGet]
		public JsonResult Read(int page = 0, int pageSize = 20, string searchText = "", [Bind(Prefix = "sort[0][dir]")] string sortOrder = null, [Bind(Prefix = "sort[0][field]")] string sortField = null)
		{
			string sortDirection = sortOrder == "desc" ? "Desc" : "Asc";

			var firstNameSearch = searchText;
			var lastNameSearch = searchText;
			bool isFullNameSearch = false;
			if (!string.IsNullOrEmpty(searchText))
			{
				searchText = searchText.Trim(' ');
				if (searchText.IndexOf(' ') > 0)
				{
					firstNameSearch = searchText.Substring(0, searchText.LastIndexOf(' ')).Trim(' ');
					lastNameSearch = searchText.Replace(firstNameSearch, "").Trim(' ');
					isFullNameSearch = true;
				}
			}

			var users = _context.User.Where(
				u => (searchText == "" ||
					  (
						  ((isFullNameSearch && (!string.IsNullOrEmpty(u.FirstName) && !string.IsNullOrEmpty(u.LastName) && u.FirstName.Contains(firstNameSearch) && u.LastName.Contains(lastNameSearch))) ||
						   (!isFullNameSearch && ((string.IsNullOrEmpty(searchText)) ||
												  (
													  (!string.IsNullOrEmpty(u.FirstName) && u.FirstName.Contains(firstNameSearch)) ||
													  (!string.IsNullOrEmpty(u.LastName) && u.LastName.Contains(lastNameSearch))
												  ))
						   ))
					  )
					));

			string sortClause = $"{sortField} {sortDirection}";
			if (string.IsNullOrEmpty(sortField) || sortField == "FullName")
			{
				// set default sorting of users
				sortClause = $"LastName {sortDirection}, FirstName {sortDirection}";
			}

			var recordCount = users.Count();

			var results = users
				.OrderBy(sortClause)
				.Skip(pageSize * (page - 1))
				.Take(pageSize).ToList()
				.Select(u => new
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

		/// <summary>
		/// Shows the view to Add the admin privilege to the specified user
		/// </summary>
		/// <param name="id">Unique identifier for the user to </param>
		/// <returns></returns>
		[HttpGet]
		[SeAuthorize(Roles = "Admin")]
		public ActionResult MakeAdmin(int id)
		{
			ViewBag.StatusMessage = "";
			var user = _context.User.Include("UserRoles").SingleOrDefault(u => u.Id == id);
			if (user == null) return new HttpNotFoundResult();
			if (user.UserRoles.Any(a => a.RoleId == Consts.Roles.Admin)) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			return View("MakeAdmin", user);
		}

		[HttpPost]
		[SeAuthorize(Roles = "Admin")]
		[ValidateAntiForgeryToken]
		public ActionResult MakeAdmin(int id, FormCollection collection)
		{

			var user = _context.User.Include("UserRoles").SingleOrDefault(u => u.Id == id);
			if (user == null) return new HttpNotFoundResult();
			if (user.UserRoles.Any(a => a.RoleId == Consts.Roles.Admin)) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			user.UserRoles.Add(new UserRole { RoleId = Consts.Roles.Admin });
			user.UserLogs.Add(new UserLog { Description = $"User was made a system admin by {UserIdentity.GetUserName(this)}" });
			_context.SaveChanges();
			return RedirectToAction("Edit", "User", new { id });
		}
		/// <summary>
		/// Shows the view to Removes the admin privilege from the specified user
		/// </summary>
		/// <param name="id">Unique identifier for the user to </param>
		/// <returns></returns>
		[HttpGet]
		[SeAuthorize(Roles = "Admin")]
		public ActionResult RemoveAdmin(int id)
		{
			ViewBag.StatusMessage = "";
			var user = _context.User.Include("UserRoles").SingleOrDefault(u => u.Id == id);
			if (user == null) return new HttpNotFoundResult();
			if (user.UserRoles.All(a => a.RoleId != Consts.Roles.Admin)) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			var isOwnProfile = id == UserIdentity.GetUserId(this);
			return View("RemoveAdmin", new RemoveRoleViewModel(user, isOwnProfile));
		}

		[HttpPost]
		[SeAuthorize(Roles = "Admin")]
		[ValidateAntiForgeryToken]
		public ActionResult RemoveAdmin(int id, FormCollection collection)
		{

			var user = _context.User.Include("UserRoles").SingleOrDefault(u => u.Id == id);
			if (user == null) return new HttpNotFoundResult();
			if (user.UserRoles.All(a => a.RoleId != Consts.Roles.Admin)) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			var userRole = user.UserRoles.Single(a => a.RoleId == Consts.Roles.Admin);
			var isOwnProfile = id == UserIdentity.GetUserId(this);
			user.UserLogs.Add(new UserLog { Description = $"User had administrator privileges removed by {UserIdentity.GetUserName(this)}" });
			_context.SetDeleted(userRole);
			_context.SaveChanges();
			if (isOwnProfile)
			{
				_userManager.SignOut();
				return RedirectToAction("Logon", "Account");
			}
			return RedirectToAction("Edit", "User", new { id });
		}


		/// <summary>
		/// Shows the view to Reset password of the specified user
		/// </summary>
		/// <param name="id">Unique identifier of the user</param>
		/// <returns></returns>
		[HttpGet]
		[SeAuthorize(Roles = "Admin")]
		public ActionResult ResetPassword(int id)
		{
			ViewBag.Message = null;
			var user = _context.User.SingleOrDefault(u => u.Id == id);
			if (user == null) return new HttpNotFoundResult();
			return View("ResetPassword", user);
		}

		[HttpPost]
		[SeAuthorize(Roles = "Admin")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ResetPassword(int id, FormCollection collection)
		{

			var user = _context.User.SingleOrDefault(u => u.Id == id);
			if (user == null) return new HttpNotFoundResult();
			var result = await _userManager.ResetPasswordAsync(id, UserIdentity.GetUserName(this));
			if (result.Succeeded)
			{
				_httpCache.SetCache($"MustChangePassword-{user.Id}", "");
				return RedirectToAction("Edit", "User", new { id });
			}
			else
			{
				ViewBag.Message = "An error occurred whilst trying to perform this action";
				return View("ResetPassword", user);
			}
		}

	}
}
