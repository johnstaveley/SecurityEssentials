using NUnit.Framework;
using Rhino.Mocks;
using SecurityEssentials.Controllers;
using SecurityEssentials.Core;
using SecurityEssentials.Core.Attributes;
using SecurityEssentials.Core.Identity;
using SecurityEssentials.Model;
using SecurityEssentials.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;

namespace SecurityEssentials.Unit.Tests.Controllers
{
	[TestFixture]
	public class AccountControllerTest : BaseControllerTest
	{

		private AccountController _sut;
		private IAppConfiguration _configuration;
		private IAppSensor _appSensor;
		private IEncryption _encryption;
		private IFormsAuth _formsAuth;
		private IHttpCache _httpCache;
		private IUserManager _userManager;
		private IRecaptcha _recaptcha;
		private string _returnUrl;
		private IServices _services;

		[SetUp]
		public void Setup()
		{
			BaseSetup();
			_appSensor = MockRepository.GenerateMock<IAppSensor>();
			_configuration = MockRepository.GenerateStub<IAppConfiguration>();
			_configuration.Stub(a => a.HasRecaptcha).Return(true);
			_encryption = MockRepository.GenerateMock<IEncryption>();
			_formsAuth = MockRepository.GenerateMock<IFormsAuth>();
			_httpCache = MockRepository.GenerateMock<IHttpCache>();
			_userManager = MockRepository.GenerateMock<IUserManager>();
			_recaptcha = MockRepository.GenerateMock<IRecaptcha>();
			_services = MockRepository.GenerateMock<IServices>();
			_sut = new AccountController(_appSensor, _configuration, _encryption, _formsAuth, Context, _httpCache, _userManager, _recaptcha, _services, UserIdentity);
			HttpRequest.Stub(x => x.Url).Return(new Uri("http://localhost/a", UriKind.Absolute));
			_sut.Url = MockRepository.GenerateMock<UrlHelper>();
			_sut.ControllerContext = new ControllerContext(HttpContext, new RouteData(), _sut);
		}

		[TearDown]
		public void Teardown()
		{
			VerifyAllExpectations();
			_appSensor.VerifyAllExpectations();
			_encryption.VerifyAllExpectations();
			_httpCache.VerifyAllExpectations();
			_userManager.VerifyAllExpectations();
			_recaptcha.VerifyAllExpectations();
			_services.VerifyAllExpectations();
		}

        [Test]
        public void When_ControllerCreated_Then_IsDecoratedWithNoCache()
        {
            var type = _sut.GetType();
            var attributes = type.GetCustomAttributes(typeof(NoCacheAttribute), true);
            Assert.IsTrue(attributes.Any(), "No NoCache Attribute found");
        }

		[Test]
		public void Given_PreviousUserActivity_When_LandingGet_Then_LastActivityDisplayed()
		{
			// Arrange
			UserIdentity.Expect(u => u.GetUserId(Arg<Controller>.Is.Anything)).Return(5);

			// Act
			var result = _sut.Landing();

			// Assert
			var data = AssertViewResultReturnsType<LandingViewModel>(result);
			Assert.AreEqual(LastAccountActivity.ToLocalTime().ToString("dd/MM/yyyy HH:mm"), data.LastAccountActivity);
			Assert.AreEqual(TestFirstName, data.FirstName);

		}		

		[Test]
		public void Given_AccountNotFound_When_LandingGet_Then_RedirectToLogon()
		{
			// Arrange
			UserIdentity.Expect(u => u.GetUserId(Arg<Controller>.Is.Anything)).Return(4);

			// Act
			var result = _sut.Landing();

			// Assert
			Assert.IsNotNull(result, "No result was returned from controller");
			AssertRedirectToActionReturned(result, "Index", "Home");

		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public async Task Given_CredentialsCorrect_When_LogonPost_Then_RedirectsToLandingPage(bool mustChangePassword)
		{
			// Arrange
			var logon = new LogOnViewModel { UserName = "joeblogs", Password = "password", RememberMe = false };
			UserManagerAttemptsLoginWithResult(true, mustChangePassword);
			_userManager.Expect(a => a.LogOnAsync(Arg<string>.Is.Anything, Arg<bool>.Is.Anything))
				.Return(Task.FromResult(1));

			// Act
			var result = await _sut.LogOnAsync(logon, _returnUrl);

			// Assert
			AssertRedirectToActionReturned(result, "Landing", "Account");
			if (mustChangePassword)
			{
				_httpCache.AssertWasCalled(a => a.SetCache(Arg<string>.Is.Equal("MustChangePassword-1"), Arg<string>.Is.Equal("")));
			}
		}

		[Test]
		public async Task Given_RedirectToLogOff_When_LogonPost_Then_RedirectsToLandingPageAsync()
		{
			// Arrange
			_returnUrl = "/Account/LogOff";
			var logon = new LogOnViewModel { UserName = "joeblogs", Password = "password", RememberMe = false };
			UserManagerAttemptsLoginWithResult(true);
			_userManager.Expect(a => a.LogOnAsync(Arg<string>.Is.Anything, Arg<bool>.Is.Anything))
				.Return(Task.FromResult(0));

			// Act
			var result = await _sut.LogOnAsync(logon, _returnUrl);

			// Assert
			AssertRedirectToActionReturned(result, "Landing", "Account");
		}

		/// <summary>
		/// SECURITY: Ensures OWASP Unvalidated redirects vulnerability does not exist
		/// </summary>
		/// <returns></returns>
		[Test]
		public async Task Given_CredentialsCorrectAndExternalReturnUrlProvided_When_LogonPost_Then_RedirectsToLandingPage()
		{
			// Arrange
			var logon = new LogOnViewModel { UserName = "joeblogs", Password = "password", RememberMe = false };
			_returnUrl = "http://www.ebay.co.uk/";
			UserManagerAttemptsLoginWithResult(true);
			_userManager.Expect(a => a.LogOnAsync(Arg<string>.Is.Anything, Arg<bool>.Is.Anything))
				.Return(Task.FromResult(0));

			// Act
			var result = await _sut.LogOnAsync(logon, _returnUrl);

			// Assert
			AssertRedirectToActionReturned(result, "Landing", "Account");
		}

		[Test]
		public async Task Given_CredentialsIncorrect_When_LogonPost_Then_ErrorViewReturned()
		{
			// Arrange
			var logon = new LogOnViewModel { UserName = "joeblogs", Password = "password1", RememberMe = false };
			UserManagerAttemptsLoginWithResult(false);
			ExpectGetRequester();
			_services.Expect(a => a.Wait(Arg<int>.Is.Anything));

			// Act
			var result = await _sut.LogOnAsync(logon, _returnUrl);

			// Assert
			AssertViewResultWithError(result, "Invalid credentials or the account is locked");
			_userManager.AssertWasNotCalled(a => a.LogOnAsync(Arg<string>.Is.Anything, Arg<bool>.Is.Anything));
			_services.AssertWasCalled(a => a.Wait(Arg<int>.Matches(b => b >= 500)));
		}

		[Test]
		public void Given_UserAuthenticated_When_LogonGet_Then_RedirectToLandingPage()
		{
			// Arrange
			HttpRequest.Stub(x => x.IsAuthenticated).Return(true);

			// Act
			var result = _sut.LogOn(_returnUrl);

			// Assert
			AssertRedirectToActionReturned(result, "Landing", "Account");

		}

		[Test]
		public void Given_UserIsNotAuthenticated_When_LogonGet_Then_ShowsView()
		{
			// Arrange

			// Act
			var result = _sut.LogOn(_returnUrl);

			// Assert
			AssertViewResultReturned(result, "LogOn");

		}


		[Test]
		public void Given_UserExists_When_ChangeEmailAddressGet_Then_ViewReturned()
		{

			// Arrange
			UserIdentity.Expect(a => a.GetUserId(Arg<Controller>.Is.Anything)).Return(TestUserId);

			// Act
			var result = _sut.ChangeEmailAddress();

			// Assert
			var viewModel = AssertViewResultReturnsType<ChangeEmailAddressViewModel>(result);
			Assert.AreEqual(TestUserName, viewModel.EmailAddress);
			Assert.IsFalse(viewModel.IsFormLocked);

		}

		[Test]
		public async Task Given_ValidSubmissionData_When_ChangeEmailAddressPost_Then_SavesEmailsAndPendingViewReturned()
		{
			// Arrange
			var model = new ChangeEmailAddressViewModel(TestUserName, null, null) { NewEmailAddress = "joe@bloggs.com" };
			UserIdentity.Expect(e => e.GetUserId(Arg<Controller>.Is.Anything)).Return(TestUserId);
			_userManager.Expect(a => a.TryLogOnAsync(Arg<string>.Is.Anything, Arg<string>.Is.Anything)).Return(Task.FromResult(new LogonResult { Success = true }));

			// Act
			var result = await _sut.ChangeEmailAddressAsync(model);

			// Assert
			AssertViewResultReturned(result, "ChangeEmailAddressPending");
			_services.AssertWasCalled(a => a.SendEmail(Arg<string>.Is.Anything, Arg<List<string>>.Is.Anything, Arg<List<string>>.Is.Anything, Arg<List<string>>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<bool>.Is.Anything));
			Context.AssertWasCalled(a => a.SaveChanges());
			var user = Context.User.Include("UserLogs").First(a => a.Id == TestUserId);
			Assert.IsFalse(string.IsNullOrEmpty(user.NewEmailAddressToken));
			Assert.IsNotNull(user.NewEmailAddressRequestExpiryDateUtc);
			Assert.IsFalse(string.IsNullOrEmpty(user.NewEmailAddress));

		}

		[Test]
		public async Task Given_EmailAddressInUse_When_ChangeEmailAddressPost_Then_SavesEmailsAndPendingViewReturned()
		{
			// Arrange
			Context.User.Add(new User { UserName = "joe@bloggs.com" });
			var model = new ChangeEmailAddressViewModel(TestUserName, null, null) { NewEmailAddress = "joe@bloggs.com" };
			UserIdentity.Expect(e => e.GetUserId(Arg<Controller>.Is.Anything)).Return(TestUserId);

			// Act
			var result = await _sut.ChangeEmailAddressAsync(model);

			// Assert
			AssertViewResultReturned(result, "ChangeEmailAddress");
			AssertViewResultWithError(result, "This email address is already in use");
			Context.AssertWasNotCalled(a => a.SaveChanges());
			var user = Context.User.First(a => a.Id == TestUserId);
			Assert.IsTrue(string.IsNullOrEmpty(user.NewEmailAddress));

		}

		[Test]
		public async Task Given_ValidChangeEmailAddressToken_When_ChangeEmailAddressConfirmGet_Then_SuccessViewShown()
		{
			// Arrange
			var requestItems = new NameValueCollection();
			var newEmaiLAddress = "samuel@pepys.com";
			var changeEmailAddressToken = "testchangetoken1";
			requestItems.Add("NewEmailAddressToken", changeEmailAddressToken);
			HttpRequest.Stub(a => a.QueryString).Return(requestItems);
			var user = Context.User.First(u => u.Id == TestUserId);
			user.NewEmailAddressToken = changeEmailAddressToken;
			user.NewEmailAddressRequestExpiryDateUtc = DateTime.UtcNow.AddMinutes(15);
			user.NewEmailAddress = newEmaiLAddress;

			// Act
			var result = await _sut.ChangeEmailAddressConfirmAsync();

			// Assert
			AssertViewResultReturned(result, "ChangeEmailAddressSuccess");
			Assert.IsNull(user.NewEmailAddressRequestExpiryDateUtc);
			Assert.IsNull(user.NewEmailAddress);
			Assert.IsNull(user.NewEmailAddressToken);
			Assert.AreEqual(newEmaiLAddress, user.UserName);
			Assert.AreEqual(3, user.UserLogs.Count, "Account activity was not logged");
			Assert.IsTrue(user.UserLogs.Any(a => a.Description.Contains(user.UserName)));
			_services.AssertWasCalled(a => a.SendEmail(Arg<string>.Is.Anything, Arg<List<string>>.Is.Anything, Arg<List<string>>.Is.Anything,
				Arg<List<string>>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<bool>.Is.Anything), options => options.Repeat.Times(2));
			_userManager.AssertWasCalled(a => a.SignOut());
			Context.AssertWasCalled(a => a.SaveChangesAsync());
		}

		[Test]
		public async Task Given_CorrectInformationProvided_When_ChangePassword_Then_SavesRedirectsAndEmails()
		{
			// Arrange
			var model = new ChangePasswordViewModel
			{
				OldPassword = "password",
				NewPassword = "pAssword1",
				ConfirmPassword = "pAssword1"
			};
			_userManager.Expect(a => a.ChangePasswordAsync(Arg<int>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything))
				.Return(Task.FromResult(new SeIdentityResult()));
			UserIdentity.Expect(a => a.GetUserId(Arg<Controller>.Is.Anything)).Return(5);
			_recaptcha.Expect(a => a.ValidateRecaptcha(Arg<Controller>.Is.Anything)).Return(true);

			// Act
			var result = await _sut.ChangePasswordAsync(model);

			// Assert
			AssertViewResultReturned(result, "ChangePasswordSuccess");
			_services.AssertWasCalled(a => a.SendEmail(Arg<string>.Is.Anything, Arg<List<string>>.Is.Anything, Arg<List<string>>.Is.Anything,
				Arg<List<string>>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<bool>.Is.Anything));
			_formsAuth.AssertWasCalled(a => a.SignOut());
			Context.AssertWasCalled(a => a.SaveChanges());
			_httpCache.AssertWasCalled(a => a.RemoveFromCache(Arg<string>.Is.Equal("MustChangePassword-5")));

		}

		public async Task Given_PasswordsDontMatch_When_ChangePassword_Then_ViewReturnedWithError()
		{
			// Arrange
			var model = new ChangePasswordViewModel
			{
				OldPassword = "password",
				NewPassword = "pAssword1",
				ConfirmPassword = "pAssword1"
			};
			_userManager.Expect(a => a.ChangePasswordAsync(Arg<int>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything))
				.Return(Task.FromResult(new SeIdentityResult(new List<string> { "Passwords don't match" })));
			UserIdentity.Expect(a => a.GetUserId(Arg<Controller>.Is.Anything)).Return(5);
			_recaptcha.Expect(a => a.ValidateRecaptcha(Arg<Controller>.Is.Anything)).Return(true);

			// Act
			var result = await _sut.ChangePasswordAsync(model);

			// Assert
			AssertViewResultReturned(result, "ChangePassword");
			Context.AssertWasNotCalled(a => a.SaveChanges());
			AssertViewResultWithError(result, "Passwords don't match");

		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public void Given_SuccessCodeProvided_When_ChangePassword_Then_ShowsViewWithMessage(bool mustChangePassword)
		{
			// Arrange
			if (mustChangePassword)
			{
				StubQueryString("Reason", "MustChangePassword");
			}
			else
			{
				HttpRequest.Stub(a => a.QueryString).Return(new NameValueCollection());
			}

			// Act
			var result = _sut.ChangePassword();

			// Assert
			var model = AssertViewResultReturnsType<ChangePasswordViewModel>(result);
			Assert.IsTrue(model.HasRecaptcha);
			Assert.That(model.MustChangePassword, Is.EqualTo(mustChangePassword));
		}

		[Test]
		public void When_ChangePasswordSuccess_Then_ShowsViewWithMessage()
		{
			// Arrange
			_sut.Url.Expect(a => a.Action(Arg<string>.Is.Equal("ChangePassword"))).Return("~/ChangePassword");

			// Act
			var result = _sut.ChangePasswordSuccess();

			// Assert
			var model = AssertViewResultReturnsType<ChangePasswordViewModel>(result);
			Assert.IsTrue(model.HasRecaptcha);
			var viewResult = AssertViewResultReturned(result, "ChangePasswordSuccess");
			Assert.That(viewResult.ViewData["ReturnUrl"], Is.EqualTo("~/ChangePassword"));

		}

		[Test]
		public void When_ChangeSecurityInformationGet_Then_ViewReturned()
		{
			// Arrange

			// Act
			var result = _sut.ChangeSecurityInformation();

			// Assert
			var model = AssertViewResultReturnsType<ChangeSecurityInformationViewModel>(result);
			Assert.IsTrue(model.HasRecaptcha);
		}

		[Test]
		public async Task Given_ValidSubmission_When_ChangeSecurityInformation_Then_ViewReturned()
		{
			// Arrange
			var model = new ChangeSecurityInformationViewModel
			{
				SecurityAnswer = "a",
				SecurityAnswerConfirm = "a"
			};
			_userManager.Expect(a => a.TryLogOnAsync(Arg<string>.Is.Anything, Arg<string>.Is.Anything))
				.Return(Task.FromResult(new LogonResult { Success = true, UserName = TestUserName }));
			_encryption.Expect(e => e.Encrypt(Arg<string>.Is.Anything, Arg<int>.Is.Anything, Arg<string>.Is.Anything, out Arg<string>.Out(EncryptedSecurityAnswerSalt).Dummy, out Arg<string>.Out(EncryptedSecurityAnswer).Dummy)).Return(false);
			_recaptcha.Expect(a => a.ValidateRecaptcha(Arg<Controller>.Is.Anything)).Return(true);


			// Act
			var result = await _sut.ChangeSecurityInformationAsync(model);

			// Assert
			AssertViewResultReturned(result, "ChangeSecurityInformationSuccess");
			Context.AssertWasCalled(a => a.SaveChangesAsync());
			_services.AssertWasCalled(a => a.SendEmail(Arg<string>.Is.Anything, Arg<List<string>>.Is.Anything, Arg<List<string>>.Is.Anything, Arg<List<string>>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<bool>.Is.Anything));

		}

		[Test]
		public async Task Given_SecurityAnswersDontMatch_When_ChangeSecurityInformation_Then_ViewWithErrorReturned()
		{
			// Arrange
			var model = new ChangeSecurityInformationViewModel
			{
				SecurityAnswer = "securityanswer",
				SecurityAnswerConfirm = "this doesnt match"
			};
			_userManager.Expect(a => a.TryLogOnAsync(Arg<string>.Is.Anything, Arg<string>.Is.Anything))
				.Return(Task.FromResult(new LogonResult { Success = true, UserName = TestUserName }));
			_recaptcha.Expect(a => a.ValidateRecaptcha(Arg<Controller>.Is.Anything)).Return(true);

			// Act
			var result = await _sut.ChangeSecurityInformationAsync(model);

			// Assert
			AssertViewResultReturned(result, "ChangeSecurityInformation");
			Context.AssertWasNotCalled(a => a.SaveChangesAsync());
			var resultModel = AssertViewResultReturnsType<ChangeSecurityInformationViewModel>(result);
			Assert.That(resultModel.ErrorMessage, Contains.Substring("The security question answers do not match"));

		}

		[Test]
		public async Task Given_AccountDetailsIncorrect_When_ChangeSecurityInformation_Then_ViewWithErrorReturned()
		{
			// Arrange
			var model = new ChangeSecurityInformationViewModel
			{
				SecurityAnswer = "a",
				SecurityAnswerConfirm = "a"
			};
			_userManager.Expect(a => a.TryLogOnAsync(Arg<string>.Is.Anything, Arg<string>.Is.Anything))
				.Return(Task.FromResult(new LogonResult { Success = false, FailedLogonAttemptCount = 1 }));
			_recaptcha.Expect(a => a.ValidateRecaptcha(Arg<Controller>.Is.Anything)).Return(true);

			// Act
			var result = await _sut.ChangeSecurityInformationAsync(model);

			// Assert
			AssertViewResultReturned(result, "ChangeSecurityInformation");
			Context.AssertWasNotCalled(a => a.SaveChangesAsync());
			var resultModel = AssertViewResultReturnsType<ChangeSecurityInformationViewModel>(result);
			Assert.That(resultModel.ErrorMessage, Contains.Substring("Security information incorrect or account locked out"));

		}

		[Test]
		public async Task Given_RequestVerificationToken_When_EmailVerify_Then_UserStatusUpdated()
		{
			// Arrange
			StubQueryString("EmailVerficationToken", "test1");

			// Act
			var result = await _sut.EmailVerifyAsync();

			// Assert
			AssertViewResultReturned(result, "EmailVerificationSuccess");
			Context.AssertWasCalled(a => a.SaveChangesAsync());
			_services.AssertWasNotCalled(a => a.SendEmail(Arg<string>.Is.Anything, Arg<List<string>>.Is.Anything, Arg<List<string>>.Is.Anything, Arg<List<string>>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<bool>.Is.Anything));
		}


		[Test]
		public void When_RecoverGet_Then_ViewReturned()
		{
			// Arrange

			// Act
			var result = _sut.Recover();

			// Assert
			var model = AssertViewResultReturnsType<RecoverViewModel>(result);
			Assert.IsTrue(model.HasRecaptcha);

		}

		[Test]
		public void When_RegisterGet_Then_ViewReturned()
		{
			// Arrange

			// Act
			var result = _sut.Register();

			// Assert
			var model = AssertViewResultReturnsType<RegisterViewModel>(result);
			Assert.IsTrue(model.HasRecaptcha);
		}

		[Test]
		public async Task Given_ValidSubmissionData_When_RecoverPost_Then_SavesEmailsAndSuccessViewReturned()
		{
			// Arrange
			RecoverViewModel model = new RecoverViewModel
			{
				UserName = TestUserName
			};
			_recaptcha.Expect(a => a.ValidateRecaptcha(Arg<Controller>.Is.Anything)).Return(true);

			// Act
			var result = await _sut.RecoverAsync(model);

			// Assert
			AssertViewResultReturned(result, "RecoverSuccess");
			_services.AssertWasCalled(a => a.SendEmail(Arg<string>.Is.Anything, Arg<List<string>>.Is.Anything, Arg<List<string>>.Is.Anything,
				Arg<List<string>>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<bool>.Is.Anything));
			Context.AssertWasCalled(a => a.SaveChangesAsync());

		}

		[Test]
		public async Task Given_ValidSubmissionData_When_RegisterPost_Then_UserIsEmailedConfirmation()
		{
			// Arrange
			var collection = GetRegisterFields();
			_userManager.Expect(a => a.CreateAsync(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<int>.Is.Anything, Arg<string>.Is.Anything)).Return(Task.FromResult(new SeIdentityResult()));
			_recaptcha.Expect(a => a.ValidateRecaptcha(Arg<Controller>.Is.Anything)).Return(true);

			// Act
			var result = await _sut.RegisterAsync(new FormCollection(collection));

			// Assert
			AssertViewResultReturned(result, "RegisterSuccess");
			_services.AssertWasCalled(a => a.SendEmail(Arg<string>.Is.Anything, Arg<List<string>>.Is.Anything, Arg<List<string>>.Is.Anything,
				Arg<List<string>>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<bool>.Is.Anything));

		}		

		[Test]
		public void Given_ValidSubmissionData_When_RecoverPasswordGet_Then_ViewShown()
		{
			// Arrange
			var requestItems = new NameValueCollection();
			var passwordResetToken = "testreset1";
			requestItems.Add("PasswordResetToken", passwordResetToken);
			HttpRequest.Stub(a => a.QueryString).Return(requestItems);
			var user = Context.User.First(u => u.Id == TestUserId);
			user.PasswordResetToken = passwordResetToken;
			user.PasswordResetExpiryDateUtc = DateTime.UtcNow.AddMinutes(15);

			// Act
			var result = _sut.RecoverPassword();

			// Assert
			var model = AssertViewResultReturnsType<RecoverPasswordViewModel>(result);
			Assert.AreEqual(passwordResetToken, model.PasswordResetToken);
			Assert.AreEqual(TestUserId, model.Id);
			Assert.AreEqual("test question", model.SecurityQuestion);
		}

		[Test]
		[TestCase("bogus token", "token", 15)]
		[TestCase("token", "token", -5)]
		public void Given_UserSubmissionInvalid_When_RecoverPasswordGet_Then_ErrorViewShown(string tokenPassed, string tokenStored, int expiryMinutes)
		{
			// Arrange
			var requestItems = new NameValueCollection();
			var passwordResetToken = tokenPassed;
			requestItems.Add("PasswordResetToken", passwordResetToken);
			HttpRequest.Stub(a => a.QueryString).Return(requestItems);
			var user = Context.User.First(u => u.Id == TestUserId);
			user.PasswordResetToken = tokenStored;
			user.PasswordResetExpiryDateUtc = DateTime.UtcNow.AddMinutes(expiryMinutes);

			// Act
			var result = _sut.RecoverPassword();

			// Assert
			AssertViewResultReturned(result, "Error");
			var model = AssertViewResultReturnsType<HandleErrorInfo>(result);
			Assert.That(model.Exception.Message, Contains.Substring("password recovery token is not valid or has expired"));
		}

		[Test]
		public void Given_UserIsDisabled_When_RecoverPasswordGet_Then_ErrorViewShown()
		{
			// Arrange
			var requestItems = new NameValueCollection();
			var passwordResetToken = "testreset1";
			requestItems.Add("PasswordResetToken", passwordResetToken);
			HttpRequest.Stub(a => a.QueryString).Return(requestItems);
			var user = Context.User.First(u => u.Id == TestUserId);
			user.PasswordResetToken = passwordResetToken;
			user.PasswordResetExpiryDateUtc = DateTime.UtcNow.AddMinutes(15);
			user.Enabled = false;

			// Act
			var result = _sut.RecoverPassword();

			// Assert
			AssertViewResultReturned(result, "Error");
			var model = AssertViewResultReturnsType<HandleErrorInfo>(result);
			Assert.That(model.Exception.Message, Contains.Substring("account is not currently approved or active"));
		}

		[Test]
		public async Task Given_ValidSubmissionData_When_RecoverPasswordPost_Then_SavesEmailsAndSuccessViewReturned()
		{
			// Arrange
			var model = new RecoverPasswordViewModel
			{
				Id = TestUserId,
				SecurityAnswer = "encryptedSecurityAnswer"
			};
			_encryption.Expect(e => e.Decrypt(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<int>.Is.Anything, Arg<string>.Is.Anything, out Arg<string>.Out(EncryptedSecurityAnswer).Dummy)).Return(false);
			_userManager.Expect(a => a.ChangePasswordFromTokenAsync(Arg<int>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything)).Return(Task.FromResult(new SeIdentityResult()));
			_userManager.Expect(a => a.LogOnAsync(Arg<string>.Is.Anything, Arg<bool>.Is.Anything)).Return(Task.FromResult(0));
			_recaptcha.Expect(a => a.ValidateRecaptcha(Arg<Controller>.Is.Anything)).Return(true);

			// Act
			var result = await _sut.RecoverPasswordAsync(model);

			// Assert
			AssertViewResultReturned(result, "RecoverPasswordSuccess");
			Context.AssertWasCalled(a => a.SaveChanges());
			_httpCache.AssertWasCalled(a => a.RemoveFromCache(Arg<string>.Is.Equal("MustChangePassword-5")));

		}

		[Test]
		public async Task Given_ResetPasswordFromTokenFails_When_RecoverPasswordPost_Then_ReturnsError()
		{
			// Arrange
			var model = new RecoverPasswordViewModel
			{
				Id = TestUserId,
				SecurityAnswer = "encryptedSecurityAnswer"
			};
			_encryption.Expect(e => e.Decrypt(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<int>.Is.Anything, Arg<string>.Is.Anything, out Arg<string>.Out(EncryptedSecurityAnswer).Dummy)).Return(false);
			_userManager.Expect(a => a.ChangePasswordFromTokenAsync(Arg<int>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything)).Return(Task.FromResult(new SeIdentityResult(new List<string> { "some stuff went wrong" })));
			_recaptcha.Expect(a => a.ValidateRecaptcha(Arg<Controller>.Is.Anything)).Return(true);

			// Act
			var result = await _sut.RecoverPasswordAsync(model);

			// Assert
			AssertViewResultReturned(result, "RecoverPassword");
			Context.AssertWasNotCalled(a => a.SaveChanges());
			AssertViewResultWithError(result, "some stuff went wrong");

		}

		[Test]
		public async Task Given_SecurityAnswerIncorrect_When_RecoverPasswordPost_Then_ReturnsError()
		{
			// Arrange
			var model = new RecoverPasswordViewModel
			{
				Id = TestUserId,
				SecurityAnswer = "encryptedSecurityAnswer",
				Password = "myPassword1234",
				ConfirmPassword = "This doesn't match1234"
			};
			_encryption.Expect(e => e.Decrypt(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<int>.Is.Anything, Arg<string>.Is.Anything, out Arg<string>.Out(EncryptedSecurityAnswer).Dummy)).Return(false);

			// Act
			var result = await _sut.RecoverPasswordAsync(model);

			// Assert
			AssertViewResultReturned(result, "RecoverPassword");
			Context.AssertWasNotCalled(a => a.SaveChanges());
			AssertViewResultWithError(result, "The passwords do not match");

		}

		[Test]
		public async Task Given_PasswordsDontmatch_When_RecoverPasswordPost_Then_ReturnsError()
		{
			// Arrange
			var model = new RecoverPasswordViewModel
			{
				Id = TestUserId,
				SecurityAnswer = "a whole load of rubbish"
			};
			_encryption.Expect(e => e.Decrypt(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<int>.Is.Anything, Arg<string>.Is.Anything, out Arg<string>.Out(EncryptedSecurityAnswer).Dummy)).Return(false);

			// Act
			var result = await _sut.RecoverPasswordAsync(model);

			// Assert
			AssertViewResultReturned(result, "RecoverPassword");
			Context.AssertWasNotCalled(a => a.SaveChanges());
			AssertViewResultWithError(result, "The security answer is incorrect");

		}

		[Test]
		public async Task Given_InvalidUser_When_RecoverPasswordPost_Then_ReturnsErrorView()
		{
			// Arrange
			var viewModel = new RecoverPasswordViewModel
			{
				Id = 666,
				SecurityAnswer = "encryptedSecurityAnswer"
			};

			// Act
			var result = await _sut.RecoverPasswordAsync(viewModel);

			// Assert
			AssertViewResultReturned(result, "Error");
			Context.AssertWasNotCalled(a => a.SaveChanges());
			var model = AssertViewResultReturnsType<HandleErrorInfo>(result);
			Assert.That(model.Exception.Message, Contains.Substring("The user is either not valid, not approved or not active"));

		}

		[Test]
		public async Task Given_UserDisabled_When_RecoverPasswordPost_Then_ReturnsErrorView()
		{
			// Arrange
			var viewModel = new RecoverPasswordViewModel
			{
				Id = TestUserId,
				SecurityAnswer = "encryptedSecurityAnswer"
			};
			var user = Context.User.First(u => u.Id == TestUserId);
			user.Enabled = false;

			// Act
			var result = await _sut.RecoverPasswordAsync(viewModel);

			// Assert
			AssertViewResultReturned(result, "Error");
			Context.AssertWasNotCalled(a => a.SaveChanges());
			var model = AssertViewResultReturnsType<HandleErrorInfo>(result);
			Assert.That(model.Exception.Message, Contains.Substring("The user is either not valid, not approved or not active"));

		}

		[Test]
		public void When_LogOff_Then_SessionAbandoned()
		{

			// Arrange
			_formsAuth.Expect(a => a.SignOut());
			HttpSession.Expect(h => h.Abandon());
			UserIdentity.Expect(a => a.RemoveAntiForgeryCookie(Arg<Controller>.Is.Anything));

			// Act
			var result = _sut.LogOff();

			// Assert
			AssertRedirectToActionReturned(result, "LogOn", "Account");
		}

		private void UserManagerAttemptsLoginWithResult(bool isSuccess, bool mustChangePassword = false)
		{
			_userManager.Expect(a => a.TryLogOnAsync(Arg<string>.Is.Anything, Arg<string>.Is.Anything))
				.Return(Task.FromResult(new LogonResult { MustChangePassword = mustChangePassword, Success = isSuccess }));
		}

		private NameValueCollection GetRegisterFields()
		{
			var collection = new NameValueCollection
			{
				{"Password", "password"},
				{"ConfirmPassword", "password"},
				{"User.UserName", TestUserName},
				{"User.FirstName", "First name"},
				{"User.LastName", "Last Name"},
				{"User.SecurityQuestionLookupItemId", "1"},
				{"User.SecurityAnswer", "Bloggs"}
			};
			return collection;

		}

	}
}
