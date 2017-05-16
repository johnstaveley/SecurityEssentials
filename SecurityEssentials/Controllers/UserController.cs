using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SecurityEssentials.Core;
using SecurityEssentials.Core.Attributes;
using SecurityEssentials.Core.Identity;
using SecurityEssentials.ViewModel;

namespace SecurityEssentials.Controllers
{
    [SEAuthorize]
    public class UserController : SecurityControllerBase
    {
        public UserController()
            : this(new AppSensor(), new SEContext(), new UserIdentity())
        {
        }

        public UserController(IAppSensor appSensor, ISEContext context, IUserIdentity userIdentity) : base(userIdentity,
            appSensor)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            _context = context;
        }


        private ISEContext _context { get; }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        /// <param name="id">The unique identifier for the user</param>
        /// <remarks>GET: /User/Disable/5</remarks>
        [SEAuthorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Disable(int id)
        {
            var user = _context.User.FirstOrDefault(u => u.Id == id);
            if (user != null) return PartialView("_Disable", user);
            var requester = _userIdentity.GetRequester(this);
            Logger.Information("Failed User Disable, user {id} did not exist by requester {@requester}", id,
                requester);
            return new HttpNotFoundResult();
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        /// <param name="id">The unique identifier of the User to disable</param>
        /// <remarks>POST: /User/Disable/5</remarks>
        [HttpPost]
        [SEAuthorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public JsonResult Disable(int id, FormCollection collection)
        {
            if (id == 0) return Json(new {success = false, message = "unable to locate user id"});
            var requester = _userIdentity.GetRequester(this);
            var user = _context.User.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                Logger.Information("Failed User Disable Post for id {id}, user did not exist by requester {@requester}",
                    id, requester);
                return Json(new {success = false, message = "unable to locate user"});
            }
            if (user.Id == _userIdentity.GetUserId(this))
                return Json(new {success = false, message = "You cannot disable your own account"});
            user.Enabled = false;
            _context.SaveChanges();
            Logger.Information("User Disable Post for id {id} suceeded, by requester {@requester}", id, requester);
            return Json(new {success = true, message = ""});
        }

        /// <summary>
        /// </summary>
        /// <param name="id">Unique identifier for the user</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var users = _context.User.Where(u => u.Id == id);
            if (users.ToList().Count == 0) return new HttpNotFoundResult();
            var user = users.FirstOrDefault();
            var requester = _userIdentity.GetRequester(this);
            // SECURE: Check user should have access to this account
            if (!_userIdentity.IsUserInRole(this, "Admin") && _userIdentity.GetUserId(this) != user.Id)
            {
                Logger.Information(
                    "Failed User Edit Get, user modification was not permitted for access rights by requester {@requester}",
                    requester);
                return new HttpNotFoundResult();
            }
            return View(new UserViewModel(_userIdentity.GetUserId(this), _userIdentity.IsUserInRole(this, "Admin"),
                user));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, FormCollection collection)
        {
            var users = _context.User.Where(u => u.Id == id);
            if (users.ToList().Count == 0) return new HttpNotFoundResult();
            var user = users.FirstOrDefault();
            var isOwnProfile = user.Id == _userIdentity.GetUserId(this);
            ViewBag.StatusMessage = "";
            var requester = _userIdentity.GetRequester(this);
            // SECURE: Check user should have access to this account
            if (!_userIdentity.IsUserInRole(this, "Admin") && _userIdentity.GetUserId(this) != user.Id)
            {
                Logger.Information(
                    "Failed User Edit Post, user modification was not permitted for access rights by requester {@requester}",
                    requester);
                return new HttpNotFoundResult();
            }

            var propertiesToUpdate = new List<string>
            {
                "FirstName",
                "LastName",
                "TelNoHome",
                "TelNoMobile",
                "TelNoWork",
                "Title",
                "Town",
                "Postcode",
                "SkypeName"
            };
            var expectedFields = new List<string> {"IsOwnProfile", "IsAdministrator", "User.Id"};
            if (_userIdentity.IsUserInRole(this, "Admin") && !isOwnProfile)
                propertiesToUpdate.AddRange(new List<string> {"Approved", "EmailVerified", "Enabled", "UserName"});
            propertiesToUpdate.ForEach(a => expectedFields.Add($"User.{a}"));
            _appSensor.ValidateFormData(this, expectedFields);
            if (TryUpdateModel(user, "User", propertiesToUpdate.ToArray(), collection))
                if (isOwnProfile && (user.Enabled == false || user.EmailVerified == false))
                {
                    Logger.Information(
                        "Failed User Edit Post, account state change prohibited by requester {@requester}", requester);
                    ModelState.AddModelError("",
                        "You cannot disable or mark as email unverified, your own user account");
                }
                else
                {
                    _context.SaveChanges();
                    if (_userIdentity.IsUserInRole(this, "Admin"))
                        return RedirectToAction("Index", "User");
                    ViewBag.StatusMessage = "Your account information has been changed";
                }
            else
                _appSensor.InspectModelStateErrors(this);

            return View("Edit",
                new UserViewModel(_userIdentity.GetUserId(this), _userIdentity.IsUserInRole(this, "Admin"), user));
        }

        [SEAuthorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Index(int page = 1)
        {
            return View(new UsersViewModel(_userIdentity.GetUserId(this)));
        }

        /// <summary>
        /// </summary>
        /// <param name="id">Unique identifier for the user</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Log(int id)
        {
            var users = _context.User.Where(u => u.Id == id);
            if (users.ToList().Count == 0) return new HttpNotFoundResult();
            var user = users.FirstOrDefault();
            var requester = _userIdentity.GetRequester(this);
            // SECURE: Check user should have access to this account
            if (!_userIdentity.IsUserInRole(this, "Admin") && _userIdentity.GetUserId(this) != user.Id)
            {
                Logger.Information("Failed User Log Get, access not permitted by requester {@requester}", requester);
                return new HttpNotFoundResult();
            }
            ViewBag.UserName = user.UserName;
            return View(user.UserLogs.OrderByDescending(ul => ul.DateCreated).Take(10).ToList());
        }

        [SEAuthorize(Roles = "Admin")]
        [HttpGet]
        public JsonResult Read(int page = 0, int pageSize = 20, string searchText = "")
        {
            var sortDirection = Request["sort[0][dir]"];
            var sortField = Request["sort[0][field]"];

            var users = _context.User.Where(
                u => searchText == "" || !string.IsNullOrEmpty(u.FirstName) && u.FirstName.Contains(searchText) ||
                     !string.IsNullOrEmpty(u.LastName) && u.LastName.Contains(searchText) ||
                     !string.IsNullOrEmpty(u.UserName) && u.UserName.Contains(searchText));

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
                users = users.OrderBy($"{sortField} {sortDirection}");
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

            return Json(new {total = recordCount, data = results}, JsonRequestBehavior.AllowGet);
        }
    }
}