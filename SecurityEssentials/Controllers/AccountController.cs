using SecurityEssentials.Core;
using SecurityEssentials.Core.Attributes;
using SecurityEssentials.Core.Constants;
using SecurityEssentials.Core.Identity;
using SecurityEssentials.Model;
using SecurityEssentials.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SecurityEssentials.Controllers
{
	public class AccountController : SecurityControllerBase
    {

	    private readonly IAppConfiguration _configuration;
	    private readonly IEncryption _encryption;
	    private readonly IFormsAuth _formsAuth;
	    private readonly IHttpCache _httpCache;
	    private readonly IRecaptcha _recaptcha;
	    private readonly IServices _services;
	    private readonly ISeContext _context;
	    private readonly IUserManager _userManager;	

        public AccountController(IAppSensor appSensor, IAppConfiguration configuration, IEncryption encryption, IFormsAuth formsAuth, ISeContext context, IHttpCache httpCache, IUserManager userManager, IRecaptcha recaptcha, IServices services, IUserIdentity userIdentity) : base(userIdentity, appSensor)
        {
			_configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
	        _context = context ?? throw new ArgumentNullException(nameof(context));
	        _encryption = encryption ?? throw new ArgumentNullException(nameof(encryption));
	        _formsAuth = formsAuth ?? throw new ArgumentNullException(nameof(formsAuth));
	        _httpCache = httpCache ?? throw new ArgumentNullException(nameof(httpCache));
	        _recaptcha = recaptcha ?? throw new ArgumentNullException(nameof(recaptcha));
	        _services = services ?? throw new ArgumentNullException(nameof(services));
	        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
		}

		[HttpPost]
		[SeAuthorize]
		[ValidateAntiForgeryToken]
		public ActionResult LogOff()
		{
			_formsAuth.SignOut();
			_userManager.SignOut();
			Logger.Debug("Entered Account Logoff Post");
			Session.Abandon();
			return RedirectToAction("LogOn", "Account");
		}

		[AllowAnonymous]
		[HttpGet]
        public ActionResult LogOn(string returnUrl)
        {
			if (Request.IsAuthenticated)
            {
                return RedirectToAction("Landing", "Account");
            }
            ViewBag.ReturnUrl = returnUrl;
            return View("LogOn");
        }

	    [HttpPost]
	    [AllowAnonymous]
	    [ValidateAntiForgeryToken]
	    [AllowXRequestsEveryXSeconds(Name = "LogOn", ContentName = "TooManyRequests", Requests = 3, Seconds = 60)]
	    public async Task<ActionResult> LogOnAsync(LogOnViewModel model, string returnUrl)
	    {

		    Requester requester = UserIdentity.GetRequester(this);
			var userName = model.UserName;
		    AppSensor.ValidateFormData(this, new List<string> { "UserName", "Password" });
			if (ModelState.IsValid)
		    {
			    var logonResult = await _userManager.TryLogOnAsync(model.UserName, model.Password);
			    if (logonResult.Success)
			    {
				    var userId = await _userManager.LogOnAsync(logonResult.UserName, model.RememberMe);
				    Logger.Information("Successful Account Logon Post for username {userName}", userName, model.UserName);
				    if (logonResult.MustChangePassword)
				    {
					    _httpCache.SetCache($"MustChangePassword-{userId}", "");
				    }
				    return RedirectToLocal(returnUrl);
			    }
			    else
			    {
				    // SECURE: Increasing wait time (with random component) for each successive logon failure (instead of locking out)
				    _services.Wait(500 + (logonResult.FailedLogonAttemptCount * 200) + (new Random().Next(4) * 200));
				    ModelState.AddModelError("", "Invalid credentials or the account is locked");
				    requester.AppSensorDetectionPoint = AppSensorDetectionPointKind.Ae1;
					Logger.Information("Failed Account Logon Post for username {userName} attempt by requester {@requester}", userName, requester);
				    if (logonResult.IsCommonUserName)
				    {
					    requester.AppSensorDetectionPoint = AppSensorDetectionPointKind.Ae12;
					    Logger.Information("Failed Account Logon Post Common username {userName} attempt by requester {@requester}", userName, requester);
					}
			    }
		    }
			else
			{
				AppSensor.InspectModelStateErrors(this);
			}

			// If we got this far, something failed, redisplay form
			return View("LogOn", model);
	    }

	    [HttpGet]
	    public ActionResult ChangeEmailAddress()
	    {
		    var userId = UserIdentity.GetUserId(this);
		    var users = _context.User.Where(u => u.Id == userId);
		    if (users.ToList().Count == 0) return new HttpNotFoundResult();
		    var user = users.Single();
		    // SECURE: Check user should have access to this account
		    if (!UserIdentity.IsUserInRole(this, "Admin") && UserIdentity.GetUserId(this) != user.Id) return new HttpNotFoundResult();
		    return View(new ChangeEmailAddressViewModel(user.UserName, user.NewEmailAddress, user.NewEmailAddressRequestExpiryDateUtc));
	    }

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		[AllowXRequestsEveryXSeconds(Name = "ChangePassword", ContentName = "TooManyRequests", Requests = 2, Seconds = 60)]
		public async Task<ActionResult> ChangeEmailAddressAsync(ChangeEmailAddressViewModel model)
		{
			var userId = UserIdentity.GetUserId(this);
			var user = _context.User.FirstOrDefault(u => u.Id == userId && u.Enabled && u.EmailVerified && u.Approved);
			AppSensor.ValidateFormData(this, new List<string> { "NewEmailAddress", "Password" });
			if (user == null) return HttpNotFound();
			if (ModelState.IsValid)
			{
				if (_context.User.Any(a => a.Id != user.Id && model.NewEmailAddress == a.UserName))
				{
					ModelState.AddModelError("NewEmailAddress", "This email address is already in use");
					Logger.Information("Failed Account ChangeEmailAddress Post, Username already exists");
				}
				else
				{
					var logonResult = await _userManager.TryLogOnAsync(UserIdentity.GetUserName(this), model.Password);
					if (logonResult.Success)
					{
						user.NewEmailAddressToken = Guid.NewGuid().ToString().Replace("-", "");
						user.NewEmailAddressRequestExpiryDateUtc = DateTime.UtcNow.AddMinutes(15);
						user.NewEmailAddress = model.NewEmailAddress;
						// Send change username with link to recover password form
						string emailBody = EmailTemplates.ChangeEmailAddressPendingBodyText(user.FirstName, user.LastName,
							_configuration.ApplicationName, _configuration.WebsiteBaseUrl, user.NewEmailAddressToken);
						string emailSubject = $"{_configuration.ApplicationName} - Complete the change email address process";
						_services.SendEmail(_configuration.DefaultFromEmailAddress, new List<string>() { user.UserName }, null, null,
							emailSubject, emailBody, true);
						user.UserLogs.Add(new UserLog
						{
							Description = $"Change email address request started to change from {user.UserName} to {user.NewEmailAddress}"
						});
						_context.SaveChanges();
						return View("ChangeEmailAddressPending");
					}
					else
					{
						Logger.Information("Failed Account ChangeEmailAddress Post, Password incorrect by requester {@requester}",
							UserIdentity.GetRequester(this, AppSensorDetectionPointKind.Ae1));
						ModelState.AddModelError("Password", "The password is not correct");
					}
				}
			}
			else
			{
				AppSensor.InspectModelStateErrors(this);
			}
			return View("ChangeEmailAddress", new ChangeEmailAddressViewModel(user.UserName, user.NewEmailAddress, user.NewEmailAddressRequestExpiryDateUtc));

		}

	    [AllowAnonymous]
	    [HttpGet]
	    public async Task<ActionResult> ChangeEmailAddressConfirmAsync()
	    {
		    var newEmaiLAddressToken = Request.QueryString["NewEmailAddressToken"] ?? "";
		    var user = _context.User.FirstOrDefault(u => u.NewEmailAddressToken == newEmaiLAddressToken && u.NewEmailAddressRequestExpiryDateUtc > DateTime.UtcNow);
		    if (user == null)
		    {
			    HandleErrorInfo error = new HandleErrorInfo(new ArgumentException("INFO: The new user name token is not valid or has expired"), "Account", "ChangeEmailAddressConfirmAsync");
			    Logger.Information("Failed Account ChangeEmailAddressConfirm Get, The new user name token is not valid or has expired");
			    return View("Error", error);
		    }
		    if (user.Enabled == false)
		    {
			    HandleErrorInfo error = new HandleErrorInfo(new InvalidOperationException("INFO: Your account is not currently approved or active"), "Account", "ChangeEmailAddressConfirmAsync");
			    Logger.Information("Failed Account ChangeEmailAddressConfirm Get, Account is not currently approved or active");
			    return View("Error", error);
		    }
		    user.UserLogs.Add(new UserLog { Description = $"Change email address request confirmed to change from {user.UserName} to {user.NewEmailAddress}" });
		    string emailSubject = $"{_configuration.ApplicationName} - Change email address process completed";
		    string emailBody = EmailTemplates.ChangeEmailAddressCompletedBodyText(user.FirstName, user.LastName, _configuration.ApplicationName, user.UserName, user.NewEmailAddress);
		    _services.SendEmail(_configuration.DefaultFromEmailAddress, new List<string> { user.UserName }, null, null, emailSubject, emailBody, true);
		    user.UserName = user.NewEmailAddress;
		    user.NewEmailAddress = null;
		    user.NewEmailAddressRequestExpiryDateUtc = null;
		    user.NewEmailAddressToken = null;
		    emailBody = $"A request has been completed to change your {_configuration.ApplicationName} username/email address to {user.UserName}. This email address can now be used to log into the application.";
		    _services.SendEmail(_configuration.DefaultFromEmailAddress, new List<string> { user.UserName }, null, null, emailSubject, emailBody, true);
		    await _context.SaveChangesAsync();
		    _userManager.SignOut();
		    return View("ChangeEmailAddressSuccess");
	    }

		[AllowAnonymous]
		[HttpGet]
		public ActionResult ChangePasswordSuccess()
        {
			ViewBag.ReturnUrl = Url.Action("ChangePassword");
            var model = new ChangePasswordViewModel
            {
                HasRecaptcha = _configuration.HasRecaptcha
            };
            return View("ChangePasswordSuccess", model);

		}

	    [SeAuthorize]
	    [HttpGet]
	    public ActionResult ChangePassword()
	    {
		    var reason = Request.QueryString["Reason"] ?? "";
		    var model = new ChangePasswordViewModel
		    {
			    HasRecaptcha = _configuration.HasRecaptcha,
			    MustChangePassword = reason == "MustChangePassword"
		    };
		    return View(model);
	    }

		[HttpPost]
        [SeAuthorize]
        [ValidateAntiForgeryToken]
		[AllowXRequestsEveryXSeconds(Name = "ChangePassword", ContentName = "TooManyRequests", Requests = 2, Seconds = 60)]
		public async Task<ActionResult> ChangePasswordAsync(ChangePasswordViewModel model)
        {
            ViewBag.ReturnUrl = Url.Action("ChangePassword");
			var requester = UserIdentity.GetRequester(this);
			var recaptchaSuccess = true;
            if (_configuration.HasRecaptcha)
            {
				recaptchaSuccess = _recaptcha.ValidateRecaptcha(this);
            }
			AppSensor.ValidateFormData(this, new List<string> { "ConfirmPassword", "OldPassword", "NewPassword" });
			if (recaptchaSuccess)
            {
				var userId = UserIdentity.GetUserId(this);
	            var user = _context.User.FirstOrDefault(u => u.Id == userId);
	            if (user != null)
	            {
		            var result = await _userManager.ChangePasswordAsync(UserIdentity.GetUserId(this), model.OldPassword, model.NewPassword);
		            if (result.Succeeded)
		            {
			            // Email recipient with password change acknowledgement
			            string emailBody = EmailTemplates.ChangePasswordCompletedBodyText(user.FirstName, user.LastName, _configuration.ApplicationName);
			            string emailSubject = $"{_configuration.ApplicationName} - Password change confirmation";
			            _services.SendEmail(_configuration.DefaultFromEmailAddress, new List<string> { user.UserName }, null, null, emailSubject, emailBody, true);
			            _context.SaveChanges();
			            _formsAuth.SignOut();
			            _userManager.SignOut();
			            _httpCache.RemoveFromCache($"MustChangePassword-{userId}");
			            Session.Abandon();
			            Logger.Debug("Account Logoff due to password change");
			            return View("ChangePasswordSuccess");
					}
                    else
                    {
						Logger.Information("Failed Account ChangePassword Post by requester {@requester}", requester);
						AddErrors(result);
                    }
                }
                else
                {
                    return HttpNotFound();
                }
            }
			else
			{
				Logger.Information("Failed Account Change Password Post Recaptcha failed by requester {@requester}", requester);
			}
			return View("ChangePassword", model);
        }

        [AllowAnonymous]
		[HttpGet]
        [AllowXRequestsEveryXSeconds(Name = "EmailVerify", ContentName = "TooManyRequests", Requests = 2, Seconds = 60)]
        public async Task<ActionResult> EmailVerifyAsync()
        {
            var emailVerificationToken = Request.QueryString["EmailVerficationToken"] ?? "";
            var user = _context.User.SingleOrDefault(u => u.EmailConfirmationToken == emailVerificationToken);
			var requester = UserIdentity.GetRequester(this);
			if (user == null)
            {
                HandleErrorInfo error = new HandleErrorInfo(new ArgumentException("INFO: The email verification token is not valid or has expired"), "Account", "EmailVerifyAsync");
				Logger.Information("Failed Acccount EmailVerify Get for token {emailVerificationToken} by requester {@requester}", emailVerificationToken, requester);
				return View("Error", error);
            }
            user.EmailVerified = true;
            user.EmailConfirmationToken = null;
            user.UserLogs.Add(new UserLog { Description = "User Verified Email Address" });
            await _context.SaveChangesAsync();
            return View("EmailVerificationSuccess");
        }

        [AllowAnonymous]
		[HttpGet]
        public ActionResult Recover()
        {
            var model = new RecoverViewModel
            {
                HasRecaptcha = _configuration.HasRecaptcha
            };
            return View("Recover", model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
		[AllowXRequestsEveryXSeconds(Name = "ChangePassword", ContentName = "TooManyRequests", Requests = 2, Seconds = 60)]
		public async Task<ActionResult> RecoverAsync(RecoverViewModel model)
        {
			var requester = UserIdentity.GetRequester(this);
			var userName = model.UserName;
			AppSensor.ValidateFormData(this, new List<string> { "UserName" });
			if (ModelState.IsValid)
            {
                var user = _context.User.SingleOrDefault(u => u.UserName == model.UserName && u.Enabled && u.EmailVerified && u.Approved);
				if (_configuration.HasRecaptcha)
                {
                    if (!_recaptcha.ValidateRecaptcha(this))
                    {
						Logger.Information("Failed Account Recover Post Recaptcha failed by requester {@requester}", requester);
						return View("Recover", model);
                    }
                }
				if (user != null)
                {
					user.PasswordResetToken = Guid.NewGuid().ToString().Replace("-", "");
                    user.PasswordResetExpiryDateUtc = DateTime.UtcNow.AddMinutes(15);
                    // Send recovery email with link to recover password form
                    string emailBody = EmailTemplates.ChangePasswordPendingBodyText(user.FirstName, user.LastName, _configuration.ApplicationName, _configuration.WebsiteBaseUrl, user.PasswordResetToken);
                    string emailSubject = $"{_configuration.ApplicationName} - Complete the password recovery process";
                    _services.SendEmail(_configuration.DefaultFromEmailAddress, new List<string>() { user.UserName }, null, null, emailSubject, emailBody, true);
                    user.UserLogs.Add(new UserLog { Description = "Password reset link generated and sent" });
                    await _context.SaveChangesAsync();
                }
				else
				{
					Logger.Information("Failed Account Recover Post UserName {userName} by requester {@requester}", userName, requester);
					return View("RecoverSuccess");
				}
			}
			else
			{
				AppSensor.InspectModelStateErrors(this);
			}
			return View("RecoverSuccess");

        }

        [AllowAnonymous]
		[HttpGet]
		[AllowXRequestsEveryXSeconds(Name = "RecoverPassword", ContentName = "TooManyRequests", Requests = 2, Seconds = 60)]
		public ActionResult RecoverPassword()
        {
            var passwordResetToken = Request.QueryString["PasswordResetToken"] ?? "";
			var requester = UserIdentity.GetRequester(this);

			var user = _context.User.Include("SecurityQuestionLookupItem").SingleOrDefault(u => u.PasswordResetToken == passwordResetToken && u.PasswordResetExpiryDateUtc > DateTime.UtcNow);
            if (user == null)
            {
                HandleErrorInfo error = new HandleErrorInfo(new ArgumentException("INFO: The password recovery token is not valid or has expired"), "Account", "RecoverPassword");
				Logger.Information("Failed Account RecoverPassword Get, recovery token {passwordResetToken} is not valid or expired by requester {@requester}", passwordResetToken, requester);
				return View("Error", error);
            }
            if (user.Enabled == false)
            {
				var userName = user.UserName;
				HandleErrorInfo error = new HandleErrorInfo(new InvalidOperationException("INFO: Your account is not currently approved or active"), "Account", "Recover");
				Logger.Information("Failed Account RecoverPassword Get, account {userName} not approved or active by requester {@requester}", userName, requester);
				return View("Error", error);
            }
            RecoverPasswordViewModel recoverPasswordModel = new RecoverPasswordViewModel()
            {
                Id = user.Id,
                HasRecaptcha = _configuration.HasRecaptcha,
                SecurityAnswer = "",
                SecurityQuestion = user.SecurityQuestionLookupItem.Description,
                PasswordResetToken = passwordResetToken,
                UserName = user.UserName
            };
            return View("RecoverPassword", recoverPasswordModel);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [AllowXRequestsEveryXSecondsAttribute(Name = "RecoverPassword", ContentName = "TooManyRequests", Requests = 2, Seconds = 60)]
        public async Task<ActionResult> RecoverPasswordAsync(RecoverPasswordViewModel recoverPasswordModel)
        {
            var user = _context.User.SingleOrDefault(u => u.Id == recoverPasswordModel.Id);
			var id = recoverPasswordModel.Id;
			var requester = UserIdentity.GetRequester(this);
			AppSensor.ValidateFormData(this, new List<string> { "UserName", "SecurityAnswer", "Password", "ConfirmPassword", "Id", "PasswordResetToken" });
			if (user == null)
            {
                HandleErrorInfo error = new HandleErrorInfo(new Exception("INFO: The user is either not valid, not approved or not active"), "Account", "RecoverPassword");
				Logger.Information("Failed Account RecoverPassword Post, recover attempted for a user id {id} which does not exist by requester {@requester}", id, requester);
				return View("Error", error);
            }
            if (!(user.Enabled))
            {
                HandleErrorInfo error = new HandleErrorInfo(new Exception("INFO: The user is either not valid, not approved or not active"), "Account", "Recover");
				Logger.Information("Failed Account RecoverPassword Post, account user id {id} is not approved or active by requester {@requester}", id, requester);
				return View("Error", error);
            }
	        string decryptedSecurityAnswer;
	        _encryption.Decrypt(_configuration.EncryptionPassword, user.SecurityAnswerSalt, _configuration.EncryptionIterationCount, user.SecurityAnswer, out decryptedSecurityAnswer);
	        if (recoverPasswordModel.SecurityAnswer != decryptedSecurityAnswer)
            {
				ModelState.AddModelError("SecurityAnswer", "The security answer is incorrect");
				Logger.Information("Failed Account RecoverPassword Post, security answer is incorrect by requester {@requester}", requester);
				return View("RecoverPassword", recoverPasswordModel);
            }
            if (recoverPasswordModel.Password != recoverPasswordModel.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "The passwords do not match");
				Logger.Information("Failed Account RecoverPassword Post, passwords do not match by requester {@requester}", requester);
				return View("RecoverPassword", recoverPasswordModel);
            }
            var recaptchaSuccess = true;
            if (_configuration.HasRecaptcha)
            {
				recaptchaSuccess = _recaptcha.ValidateRecaptcha(this);
            }
			if (recaptchaSuccess)
			{
				if (ModelState.IsValid)
				{
					var result = await _userManager.ChangePasswordFromTokenAsync(user.Id, recoverPasswordModel.PasswordResetToken, recoverPasswordModel.Password);
					if (result.Succeeded)
					{
						_context.SaveChanges();
						_httpCache.RemoveFromCache(string.Concat("MustChangePassword-", user.Id));
						await _userManager.LogOnAsync(user.UserName, false);
						Logger.Information("Successful RecoverPassword Post by requester {@requester}", requester);
						return View("RecoverPasswordSuccess");
					}
					else
					{
						AddErrors(result);
						Logger.Information("Failed Account RecoverPassword Post, change password async failed by requester {@requester}", requester);
						return View("RecoverPassword", recoverPasswordModel);
					}
				}
				else
				{
					ModelState.AddModelError("", "Password change was not successful");
					AppSensor.InspectModelStateErrors(this);
				}
			}
			else
			{
				Logger.Information("Failed Account Recover Password Post Recaptcha failed by requester {@requester}", requester);
			}
			return View("RecoverPassword", recoverPasswordModel);

		}

		[SeAuthorize]
		[HttpGet]
        public ActionResult ChangeSecurityInformation()
        {
            var securityQuestions = _context.LookupItem.Where(l => l.LookupTypeId == Consts.LookupTypeId.SecurityQuestion && l.IsHidden == false).OrderBy(o => o.Ordinal).ToList();
            var changeSecurityInformationViewModel = new ChangeSecurityInformationViewModel("", _configuration.HasRecaptcha, securityQuestions);
            return View(changeSecurityInformationViewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [AllowXRequestsEveryXSeconds(Name = "ChangeSecurityInformation", ContentName = "TooManyRequests", Requests = 2, Seconds = 60)]
        public async Task<ActionResult> ChangeSecurityInformationAsync(ChangeSecurityInformationViewModel model)
        {
            string errorMessage = "";
			var requester = UserIdentity.GetRequester(this);
			AppSensor.ValidateFormData(this, new List<string> { "SecurityQuestionLookupItemId", "SecurityAnswer", "SecurityAnswerConfirm", "Password" });
			if (ModelState.IsValid)
            {
                var recaptchaSuccess = true;
                if (_configuration.HasRecaptcha)
                {
					recaptchaSuccess = _recaptcha.ValidateRecaptcha(this);
                }
                var logonResult = await _userManager.TryLogOnAsync(UserIdentity.GetUserName(this), model.Password);
				if (recaptchaSuccess)
				{
					if (logonResult.Success)
					{
						if (model.SecurityAnswer == model.SecurityAnswerConfirm)
						{
							var user = _context.User.First(u => u.UserName == logonResult.UserName);
							string encryptedSecurityAnswer;
							string encryptedSecurityAnswerSalt;
							_encryption.Encrypt(_configuration.EncryptionPassword, _configuration.EncryptionIterationCount, model.SecurityAnswer, out encryptedSecurityAnswerSalt, out encryptedSecurityAnswer);
							user.SecurityAnswer = encryptedSecurityAnswer;
							user.SecurityAnswerSalt = encryptedSecurityAnswerSalt;
							user.SecurityQuestionLookupItemId = model.SecurityQuestionLookupItemId;
							user.UserLogs.Add(new UserLog { Description = "User Changed Security Information" });
							await _context.SaveChangesAsync();

							// Email the user to complete the email verification process or inform them of a duplicate registration and would they like to change their password
							string emailSubject = string.Format("{0} - Security Information Changed", _configuration.ApplicationName);
							string emailBody = EmailTemplates.ChangeSecurityInformationCompletedBodyText(user.FirstName, user.LastName, _configuration.ApplicationName);
							_services.SendEmail(_configuration.DefaultFromEmailAddress, new List<string>() { logonResult.UserName }, null, null, emailSubject, emailBody, true);
							return View("ChangeSecurityInformationSuccess");
						}
						else
						{
							Logger.Information("Failed Account ChangeSecurityInformation Post, security answers do not match by requester {@requester}", requester);
							errorMessage = "The security question answers do not match";
						}
					}
					else
					{
						Logger.Information("Failed Account ChangeSecurityInformation Post, security information incorrect or account locked out by requester {@requester}", requester);
						errorMessage = "Security information incorrect or account locked out";
					}
				}
				else
				{
					AppSensor.InspectModelStateErrors(this);
				}
			}
            var securityQuestions = _context.LookupItem.Where(l => l.LookupTypeId == Consts.LookupTypeId.SecurityQuestion && l.IsHidden == false).OrderBy(o => o.Ordinal).ToList();
            var changeSecurityInformationViewModel = new ChangeSecurityInformationViewModel(errorMessage, _configuration.HasRecaptcha, securityQuestions);
            return View("ChangeSecurityInformation", changeSecurityInformationViewModel);


		}

        [AllowAnonymous]
		[HttpGet]
        public ActionResult Register()
        {
            var securityQuestions = _context.LookupItem.Where(l => l.LookupTypeId == Consts.LookupTypeId.SecurityQuestion && l.IsHidden == false).OrderBy(o => o.Ordinal).ToList();
            var registerViewModel = new RegisterViewModel("", _configuration.HasRecaptcha, "", new User(), securityQuestions);
            return View(registerViewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [AllowXRequestsEveryXSeconds(Name = "Register", ContentName = "TooManyRequests", Requests = 2, Seconds = 60)]
        public async Task<ActionResult> RegisterAsync(FormCollection collection)
        {
	        var user = new User();
	        var password = collection["Password"];
	        var confirmPassword = collection["ConfirmPassword"];
			AppSensor.ValidateFormData(this, new List<string> { "Password", "ConfirmPassword", "User.FirstName", "User.LastName", "User.UserName", "User.SecurityQuestionLookupItemId", "User.SecurityAnswer" });
			var requester = UserIdentity.GetRequester(this);
			if (ModelState.IsValid)
            {
                var propertiesToUpdate = new[]
            {
                    "FirstName", "LastName", "UserName", "SecurityQuestionLookupItemId", "SecurityAnswer"
                };
                if (TryUpdateModel(user, "User", propertiesToUpdate, collection))
                {
                    var recaptchaSuccess = true;
                    if (_configuration.HasRecaptcha)
                    {
						recaptchaSuccess = _recaptcha.ValidateRecaptcha(this);
                    }
					var userName = user.UserName;
                    if (recaptchaSuccess)
                    {
                        var result = await _userManager.CreateAsync(user.UserName, user.FirstName, user.LastName, password, confirmPassword,
                            user.SecurityQuestionLookupItemId, user.SecurityAnswer);
                        if (result.Succeeded || result.Errors.Any(e => e == "Username already registered"))
                        {
							user = _context.User.First(u => u.UserName == user.UserName);
							// Email the user to complete the email verification process or inform them of a duplicate registration and would they like to change their password
	                        string emailBody;
	                        string emailSubject;
                            if (result.Succeeded)
                            {
	                            emailSubject = $"{_configuration.ApplicationName} - Complete your registration";
                                emailBody = EmailTemplates.RegistrationPendingBodyText(user.FirstName, user.LastName, _configuration.ApplicationName, _configuration.WebsiteBaseUrl, user.EmailConfirmationToken);
	                            Logger.Information("Successful Account Register Post for username {userName} by requester {@requester}", userName, requester);
                            }
							else
                            {
	                            emailSubject = $"{_configuration.ApplicationName} - Duplicate Registration";
                                emailBody = EmailTemplates.RegistrationDuplicatedBodyText(user.FirstName, user.LastName, _configuration.ApplicationName, _configuration.WebsiteBaseUrl);
	                            Logger.Information("Duplicate Account Register Post for username {userName} by requester {@requester}", userName, requester);
							}

							_services.SendEmail(_configuration.DefaultFromEmailAddress, new List<string> { user.UserName }, null, null, emailSubject, emailBody, true);
							return View("RegisterSuccess");
                        }
                        else
                        {
							var errorMessage = result.Errors.FirstOrDefault();
							Logger.Information("Failed Account Register Post of username {userName} with message {errorMessage}, user creation failed by requester {@requester}", userName, errorMessage, requester);
							AddErrors(result);
                        }
                    }
					else
					{
						Logger.Information("Failed Account Register Post Recaptcha failed by requester {@requester}", requester);
					}
				}
            }
			else
			{
				AppSensor.InspectModelStateErrors(this);
			}
			var securityQuestions = _context.LookupItem.Where(l => l.LookupTypeId == Consts.LookupTypeId.SecurityQuestion && l.IsHidden == false).OrderBy(o => o.Ordinal).ToList();
            var registerViewModel = new RegisterViewModel(confirmPassword, _configuration.HasRecaptcha, password, user, securityQuestions);
            return View("Register", registerViewModel);

        }

        [HttpGet]
		public ActionResult Landing()
        {
            var currentUserId = UserIdentity.GetUserId(this);
            var users = _context.User.Where(u => u.Id == currentUserId);
	        // Usually means user is not logged on
	        if (users.ToList().Count == 0) return RedirectToAction("Index", "Home");
	        var user = users.Single();
            var activityLogs = user.UserLogs.OrderByDescending(d => d.CreatedDateUtc);
            UserLog lastAccountActivity = null;
            if (activityLogs.ToList().Count > 1)
            {
                lastAccountActivity = activityLogs.Skip(1).FirstOrDefault();
            }
            return View(new LandingViewModel(user.FirstName, lastAccountActivity, currentUserId));
        }

        #region Helper Functions

        private void AddErrors(SeIdentityResult result)
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
				if (!string.IsNullOrEmpty(returnUrl))
				{
					var requester = UserIdentity.GetRequester(this);
					Logger.Information("Logon redirect attempted to redirect to external site {returnUrl}, by requester {@requester}", returnUrl, requester);
				}
				return RedirectToAction("Landing", "Account");
            }
        }
		
        #endregion

    }
}