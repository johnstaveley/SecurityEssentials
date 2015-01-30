using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SecurityEssentials.Model;
using SecurityEssentials.Core.Identity;
using SecurityEssentials.Core;
using SecurityEssentials.ViewModel;

namespace SecurityEssentials.Controllers
{
    public class AccountController : AntiForgeryControllerBase
    {

		private IUserManager UserManager;

        #region Constructor

        public AccountController()            
        {
			UserManager = new MyUserManager();
        }

        #endregion

        #region LogOff

        [HttpPost]
        [Authorize]
        public ActionResult LogOff()
        {
            UserManager.SignOut();
            Session.Abandon();
			return RedirectToAction("LogOn");
        }

        #endregion

        #region LogOn

        [AllowAnonymous]
        public ActionResult LogOn(string returnUrl)
        {
            if (Request.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.ReturnUrl = returnUrl;
            return View("LogOn");
        }

        [HttpPost]
        [AllowAnonymous]
        [AllowXRequestsEveryXSecondsAttribute(Name = "LogOn", Message = "You have performed this action more than {x} times in the last {n} seconds.", Requests = 3, Seconds = 60)]
        public async Task<ActionResult> LogOn(LogOn model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindAsync(model.UserName, model.Password);
                if (user.Success)
                {
                    await UserManager.SignInAsync(user.UserName, model.RememberMe);
                    return RedirectToLocal(returnUrl);
                }
                else
                {
					// SECURE: Increasing wait time (with random component) for each successive logon failure (instead of locking out)
					System.Threading.Thread.Sleep(500 + (user.FailedLogonAttemptCount * 200) + (new Random().Next(4) * 200));
                    ModelState.AddModelError("", "Invalid credentials or account is locked");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

		#region ChangePassword

		public ActionResult ChangePassword(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            ViewBag.ReturnUrl = Url.Action("ChangePassword");
            return View();
        }

        [HttpPost]
        [Authorize]
        [AllowXRequestsEveryXSecondsAttribute(Name = "ChangePassword", Message = "You have performed this action more than {x} times in the last {n} seconds.", Requests = 2, Seconds = 60)]
		public async Task<ActionResult> ChangePassword(ChangePassword model)
        {
			ViewBag.ReturnUrl = Url.Action("ChangePassword");
            var result = await UserManager.ChangePasswordAsync(Convert.ToInt32(User.Identity.GetUserId()), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
				// TODO: Email recipient with password change acknowledgement
				return RedirectToAction("ChangePassword", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            else
            {
                AddErrors(result);
            }
            return View(model);
        }

        #endregion

        #region Recover

        [AllowAnonymous]
        public ActionResult Recover()
        {
            return View("Recover");
        }

        [HttpPost]
        [AllowAnonymous]
        [AllowXRequestsEveryXSecondsAttribute(Name = "Recover", ContentName = "TooManyRequests", Requests = 2, Seconds = 60)]
        public async Task<ActionResult> Recover(Recover model)
        {
            if (ModelState.IsValid)
            {
                ViewBag.StatusMessage = "Your request has been processed. If the email you entered was valid and your account active then a reset email will be sent to your address";
                var user = await UserManager.FindByEmailAsync(model.UserName);
                if (user != null)
                {
                    if (user.Enabled == true)
                    {
                        await UserManager.GeneratePasswordResetTokenAsync(user.Id);
						// TODO: Send recovery email with link to recover password form
                    }
                }
            }

            return View(model);
        }

        [AllowAnonymous]
        public ActionResult RecoverPassword()
        {
            var passwordResetToken = Request["PasswordResetToken"] ?? "";
			using (var context = new SEContext())
			{
				var user = context.User.Include("SecurityQuestionLookupItem").Where(u => u.PasswordResetToken == passwordResetToken && u.PasswordResetExpiry > DateTime.Now).FirstOrDefault();
				if (user == null)
				{
					HandleErrorInfo error = new HandleErrorInfo(new ArgumentException("INFO: The password recovery token is not valid or has expired"), "Account", "RecoverPassword");
					return View("Error", error);
				}
				if (user.Enabled == false)
				{
					HandleErrorInfo error = new HandleErrorInfo(new InvalidOperationException("INFO: Your account is not currently approved or active"), "Account", "Recover");
					return View("Error", error);
				}
				RecoverPassword recoverPasswordModel = new RecoverPassword()
				{
					Id = user.Id,
					SecurityAnswer = "",
					SecurityQuestion = user.SecurityQuestionLookupItem.Description,
					PasswordResetToken = passwordResetToken,
					UserName = user.UserName
				};
				return View("RecoverPassword", recoverPasswordModel);
			}
        }

        [HttpPost]
        [AllowAnonymous]
        [AllowXRequestsEveryXSecondsAttribute(Name = "RecoverPassword", ContentName = "TooManyRequests", Requests = 2, Seconds = 60)]
        public async Task<ActionResult> RecoverPassword(RecoverPassword recoverPasswordModel)
        {
			using (var context = new SEContext())
			{
				var user = context.User.Where(u => u.Id == recoverPasswordModel.Id).FirstOrDefault();
				if (user == null)
				{
					HandleErrorInfo error = new HandleErrorInfo(new Exception("INFO: The user is not valid"), "Account", "RecoverPassword");
					return View("Error", error);
				}
				if (!(user.Enabled))
				{
					HandleErrorInfo error = new HandleErrorInfo(new Exception("INFO: Your account is not currently approved or active"), "Account", "Recover");
					return View("Error", error);
				}
				if (user.SecurityAnswer != recoverPasswordModel.SecurityAnswer)
				{
					ModelState.AddModelError("SecurityAnswer", "The security answer is incorrect");
					return View("RecoverPassword", recoverPasswordModel);
				}
				if (recoverPasswordModel.Password != recoverPasswordModel.ConfirmPassword)
				{
					ModelState.AddModelError("ConfirmPassword", "The passwords do not match");
					return View("RecoverPassword", recoverPasswordModel);
				}

				if (ModelState.IsValid)
				{
					var result = await UserManager.ChangePasswordFromTokenAsync(user.Id, recoverPasswordModel.PasswordResetToken, recoverPasswordModel.Password);
					if (result.Succeeded)
					{
						context.SaveChanges();
						await UserManager.SignInAsync(user.UserName, false);
						return View("RecoverPasswordSuccess");
					}
					else
					{
						AddErrors(result);
						return View("RecoverPassword", recoverPasswordModel);
					}
				}
				else
				{
					ModelState.AddModelError("", "Password change was not successful");
					return View("RecoverPassword", recoverPasswordModel);
				}
			}
        }

        #endregion

        #region Register

        [AllowAnonymous]
        public ActionResult Register()
        {
			var context = new SEContext();
			var securityQuestions = context.LookupItem.Where(l => l.LookupTypeId == CONSTS.LookupTypeId.SecurityQuestion && l.IsHidden == false).OrderBy(o => o.Ordinal).ToList();
			var registerViewModel = new RegisterViewModel("", "", new User(), securityQuestions);
			return View(registerViewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Register(FormCollection collection)
        {
			var user = new User();
			var password = collection["Password"].ToString();
			var confirmPassword = collection["ConfirmPassword"].ToString();
			if (ModelState.IsValid)
            {
				var propertiesToUpdate = new[]
                {
                    "FirstName", "LastName", "UserName", "SecurityQuestionLookupItemId", "SecurityAnswer"
                };
				if (TryUpdateModel(user, "User", propertiesToUpdate, collection))
				{
					var result = await UserManager.CreateAsync(user.UserName, user.FirstName, user.LastName, password, confirmPassword, 
						user.SecurityQuestionLookupItemId, user.SecurityAnswer);
					if (result.Succeeded || result.Errors.Any(e => e == "Username already registered"))
					{
						// TODO: Email the user to complete the email verification process or inform them of a duplicate registration and would they like to change their password
						return View("RegisterSuccess");
					}
					else
					{
						AddErrors(result);
					}
				}
            }

			var context = new SEContext();
			var securityQuestions = context.LookupItem.Where(l => l.LookupTypeId == CONSTS.LookupTypeId.SecurityQuestion && l.IsHidden == false).OrderBy(o => o.Ordinal).ToList();
			var registerViewModel = new RegisterViewModel(confirmPassword, password, user, securityQuestions);
			return View(registerViewModel);
		}

        #endregion

        #region Helper Functions

        private void AddErrors(SEIdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
            return RedirectToAction("Index", "Home");
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            Error
        }

        #endregion

    }
}