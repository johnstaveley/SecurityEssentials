using System;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecurityEssentials.Controllers;
using Rhino.Mocks;
using SecurityEssentials.Core;
using SecurityEssentials.Core.Identity;
using SecurityEssentials.Model;
using System.Collections.Generic;
using SecurityEssentials.ViewModel;
using System.Threading.Tasks;
using System.Web.Routing;

namespace SecurityEssentials.Unit.Tests.Controllers
{
	[TestClass]
    public class AccountControllerTest : BaseControllerTest
    {

        private AccountController _sut;
        private IAppConfiguration _configuration;
        private IEncryption _encryption;
        private IFormsAuth _formsAuth;
        private IUserManager _userManager;
        private IRecaptcha _recaptcha;
        private string _returnUrl = null;
        private IServices _services;

        [TestInitialize]
        public void Setup()
        {
            base.BaseSetup();
            _configuration = MockRepository.GenerateStub<IAppConfiguration>();
            _configuration.Stub(a => a.HasRecaptcha).Return(true);
            _encryption = MockRepository.GenerateMock<IEncryption>();
            _formsAuth = MockRepository.GenerateMock<IFormsAuth>();
            _userManager = MockRepository.GenerateMock<IUserManager>();
            _recaptcha = MockRepository.GenerateMock<IRecaptcha>();
            _services = MockRepository.GenerateMock<IServices>();
            _sut = new AccountController(_configuration, _encryption, _formsAuth, _context, _userManager, _recaptcha, _services, _userIdentity);
            _httpRequest.Stub(x => x.Url).Return(new Uri("http://localhost/a", UriKind.Absolute));
            _sut.Url = new UrlHelper(new RequestContext(_httpContext, new RouteData()), new RouteCollection());
            _sut.ControllerContext = new ControllerContext(_httpContext, new RouteData(), _sut);
        }

        [TestCleanup]
        public void Teardown()
        {
            base.VerifyAllExpectations();
            _encryption.VerifyAllExpectations();
            _userManager.VerifyAllExpectations();
            _recaptcha.VerifyAllExpectations();
            _services.VerifyAllExpectations();
        }

        [TestMethod]
        public void Given_PreviousUserActivity_When_UserLands_Then_LastActivityDisplayed()
        {
            // Arrange
            _userIdentity.Expect(u => u.GetUserId(Arg<Controller>.Is.Anything)).Return(5);

            // Act
            var result = _sut.Landing();

            // Assert
            var data = AssertViewResultReturnsType<LandingViewModel>(result);
            Assert.AreEqual(_lastAccountActivity.ToLocalTime().ToString("dd/MM/yyyy HH:mm"), data.LastAccountActivity);
            Assert.AreEqual(_firstName, data.FirstName);
            
        }

        [TestMethod]
        public void Given_AccountNotFound_When_UserLands_Then_HttpNotFound()
        {
            // Arrange
            _userIdentity.Expect(u => u.GetUserId(Arg<Controller>.Is.Anything)).Return(4);

            // Act
            var result = _sut.Landing();

            // Assert
            Assert.IsNotNull(result, "No result was returned from controller");
            Assert.IsInstanceOfType(result, typeof(HttpNotFoundResult), "Not HttpNotFoundResult returned from controller");

        }

        [TestMethod]
        public async Task Given_CredentialsCorrect_When_Logon_Then_RedirectsToLandingPage()
        {
            // Arrange
            LogOn logon = new LogOn() { UserName = "joeblogs", Password = "password", RememberMe = false };
            UserManagerAttemptsLoginWithResult(true);
            _userManager.Expect(a => a.LogOnAsync(Arg<string>.Is.Anything, Arg<bool>.Is.Anything))
                .Return(Task.FromResult(0));

            // Act
            var result = await _sut.LogOn(logon, _returnUrl );

            // Assert
            AssertRedirectToActionReturned(result, "Landing", "Account");
        }

		/// <summary>
		/// SECURITY: Ensures OWASP Unvalidated redirects vulnerability does not exist
		/// </summary>
		/// <returns></returns>
		[TestMethod]
		public async Task Given_CredentialsCorrectAndExternalReturnUrlProvided_When_Logon_Then_RedirectsToLandingPage()
		{
			// Arrange
			LogOn logon = new LogOn() { UserName = "joeblogs", Password = "password", RememberMe = false };
			_returnUrl = "http://www.ebay.co.uk/";
			UserManagerAttemptsLoginWithResult(true);
			_userManager.Expect(a => a.LogOnAsync(Arg<string>.Is.Anything, Arg<bool>.Is.Anything))
				.Return(Task.FromResult(0));

			// Act
			var result = await _sut.LogOn(logon, _returnUrl);

			// Assert
			AssertRedirectToActionReturned(result, "Landing", "Account");
		}

		[TestMethod]
        public async Task Given_CredentialsIncorrect_When_Logon_Then_ErrorViewReturned()
        {
            // Arrange
            LogOn logon = new LogOn() { UserName = "joeblogs", Password = "password1", RememberMe = false };
            UserManagerAttemptsLoginWithResult(false);
            _services.Expect(a => a.Wait(Arg<int>.Is.Anything));

            // Act
            var result = await _sut.LogOn(logon, _returnUrl);

            // Assert
            AssertViewResultWithError(result, "Invalid credentials or the account is locked");
            _userManager.AssertWasNotCalled(a => a.LogOnAsync(Arg<string>.Is.Anything, Arg<bool>.Is.Anything));
            _services.AssertWasCalled(a => a.Wait(Arg<int>.Matches(b => b >= 500)));
        }

        [TestMethod]
        public void Given_UserAuthenticated_When_LogonViewRequested_Then_RedirectToLandingPage()
        {
            // Arrange
            _httpRequest.Stub(x => x.IsAuthenticated).Return(true);

            // Act
            var result = _sut.LogOn(_returnUrl);

            // Assert
            AssertRedirectToActionReturned(result, "Landing", "Account");

        }

        [TestMethod]
        public void Given_UserIsNotAuthenticated_When_LogonViewRequested_Then_ShowsView()
        {
            // Arrange

            // Act
            var result = _sut.LogOn(_returnUrl);

            // Assert
            AssertViewResultReturned(result, "LogOn");

        }


		[TestMethod]
		public void Given_UserExists_When_ChangeEmailAddressGet_Then_ViewReturned()
		{

			// Arrange
			_userIdentity.Expect(a => a.GetUserId(Arg<Controller>.Is.Anything)).Return(_testUserId);

			// Act
			var result = _sut.ChangeEmailAddress();

			// Assert
			var viewModel = AssertViewResultReturnsType<ChangeEmailAddressViewModel>(result);
			Assert.AreEqual(_testUserName, viewModel.EmailAddress);
			Assert.IsFalse(viewModel.IsFormLocked);

		}

		[TestMethod]
		public async Task Given_ValidSubmissionData_When_ChangeEmailAddress_Then_SavesEmailsAndPendingViewReturned()
		{
			// Arrange
			var model = new ChangeEmailAddressViewModel(_testUserName, null, null);
			model.NewEmailAddress = "joe@bloggs.com";
			_userIdentity.Expect(e => e.GetUserId(Arg<Controller>.Is.Anything)).Return(_testUserId);
			_userManager.Expect(a => a.TryLogOnAsync(Arg<string>.Is.Anything, Arg<string>.Is.Anything)).Return(Task.FromResult(new LogonResult() { Success = true }));

			// Act
			var result = await _sut.ChangeEmailAddress(model);

			// Assert
			AssertViewResultReturned(result, "ChangeEmailAddressPending");
			_services.AssertWasCalled(a => a.SendEmail(Arg<string>.Is.Anything, Arg<List<string>>.Is.Anything, Arg<List<string>>.Is.Anything,
				Arg<List<string>>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<bool>.Is.Anything));
			_context.AssertWasCalled(a => a.SaveChanges());
			var user = _context.User.Include("UserLogs").Where(a => a.Id == _testUserId).First();
			Assert.IsFalse(string.IsNullOrEmpty(user.NewEmailAddressToken));
			Assert.IsNotNull(user.NewEmailAddressRequestExpiryDate);
			Assert.IsFalse(string.IsNullOrEmpty(user.NewEmailAddress));

		}

		[TestMethod]
		public async Task Given_ValidChangeEmailAddressToken_When_ChangeEmailAddressConfirmGet_Then_SuccessViewShown()
		{
			// Arrange
			var requestItems = new NameValueCollection();
			var newEmaiLAddress = "samuel@pepys.com";
			var changeEmailAddressToken = "testchangetoken1";
			requestItems.Add("NewEmailAddressToken", changeEmailAddressToken);
			_httpRequest.Stub(a => a.QueryString).Return(requestItems);
			var user = _context.User.Where(u => u.Id == _testUserId).First();
			user.NewEmailAddressToken = changeEmailAddressToken;
			user.NewEmailAddressRequestExpiryDate = DateTime.UtcNow.AddMinutes(15);
			user.NewEmailAddress = newEmaiLAddress;

			// Act
			var result = await _sut.ChangeEmailAddressConfirm();

			// Assert
			AssertViewResultReturned(result, "ChangeEmailAddressSuccess");
			Assert.IsNull(user.NewEmailAddressRequestExpiryDate);
			Assert.IsNull(user.NewEmailAddress);
			Assert.IsNull(user.NewEmailAddressToken);
			Assert.AreEqual(newEmaiLAddress, user.UserName);
			Assert.AreEqual(3, user.UserLogs.Count, "Account activity was not logged");
			Assert.IsTrue(user.UserLogs.Any(a => a.Description.Contains(user.UserName)));
			_services.AssertWasCalled(a => a.SendEmail(Arg<string>.Is.Anything, Arg<List<string>>.Is.Anything, Arg<List<string>>.Is.Anything,
				Arg<List<string>>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<bool>.Is.Anything), options => options.Repeat.Times(2));
			_userManager.AssertWasCalled(a => a.SignOut());
			_context.AssertWasCalled(a => a.SaveChangesAsync());
		}	

        [TestMethod]
        public async Task Given_CorrectInformationProvided_When_ChangePassword_Then_SavesRedirectsAndEmails()
        {
            // Arrange
            var model = new ViewModel.ChangePasswordViewModel()
            { 
             OldPassword = "password",
             NewPassword = "pAssword1",
             ConfirmPassword = "pAssword1"
            };
            _userManager.Expect(a => a.ChangePasswordAsync(Arg<int>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything))
                .Return(Task.FromResult(new SEIdentityResult() { }));
            _userIdentity.Expect(u => u.GetUserId(Arg<Controller>.Is.Anything)).Return(5);

            // Act
            var result = await _sut.ChangePassword(model);

            // Assert
			AssertViewResultReturned(result, "ChangePasswordSuccess");
            _services.AssertWasCalled(a => a.SendEmail(Arg<string>.Is.Anything, Arg<List<string>>.Is.Anything, Arg<List<string>>.Is.Anything, 
                Arg<List<string>>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<bool>.Is.Anything));
			_formsAuth.AssertWasCalled(a => a.SignOut());
            _context.AssertWasCalled(a => a.SaveChanges());

        }

		[TestMethod]
		public void Given_SuccessCodeProvided_When_ChangePassword_Then_ShowsViewWithMessage()
		{
			// Arrange

			// Act
			var result = _sut.ChangePassword();

			// Assert
			var model = AssertViewResultReturnsType<ChangePasswordViewModel>(result);
			Assert.IsTrue(model.HasRecaptcha);
			var viewResult = (ViewResult)result;

		}


		[TestMethod]
        public void When_ChangeSecurityInformationGet_Then_ViewReturned()
        {
            // Arrange

            // Act
            var result = _sut.ChangeSecurityInformation();

            // Assert
            var model = AssertViewResultReturnsType<ChangeSecurityInformationViewModel>(result);
            Assert.IsTrue(model.HasRecaptcha);
        }

        [TestMethod]
        public async Task When_ChangeSecurityInformation_Then_ViewReturned()
        {
            // Arrange
            var model = new ChangeSecurityInformationViewModel()
            {
                SecurityAnswer = "a",
                SecurityAnswerConfirm = "a"
            };
            _userManager.Expect(a => a.TryLogOnAsync(Arg<string>.Is.Anything, Arg<string>.Is.Anything))
                .Return(Task.FromResult(new LogonResult() {  Success = true, UserName = _testUserName }));
            _encryption.Expect(e => e.Encrypt(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<int>.Is.Anything, Arg<string>.Is.Anything, out Arg<string>.Out(_encryptedSecurityAnswer).Dummy)).Return(false);


            // Act
            var result = await _sut.ChangeSecurityInformation(model);

            // Assert
            AssertViewResultReturned(result, "ChangeSecurityInformationSuccess");
            _context.AssertWasCalled(a => a.SaveChangesAsync());
            _services.AssertWasCalled(a => a.SendEmail(Arg<string>.Is.Anything, Arg<List<string>>.Is.Anything, Arg<List<string>>.Is.Anything,
                Arg<List<string>>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<bool>.Is.Anything));            

        }

        [TestMethod]
        public async Task Given_RequestVerificationToken_When_EmailVerify_Then_UserStatusUpdated()
        {
            // Arrange
            var requestItems = new NameValueCollection();
            requestItems.Add("EmailVerficationToken", "test1");
            _httpRequest.Stub(a => a.QueryString).Return(requestItems);

            // Act
            var result = await _sut.EmailVerify();

            // Assert
            AssertViewResultReturned(result, "EmailVerificationSuccess");
            _context.AssertWasCalled(a => a.SaveChangesAsync());

        }

        [TestMethod]
        public void When_RecoverGet_Then_ViewReturned()
        {
            // Arrange

            // Act
            var result = _sut.Recover();

            // Assert
            var model = AssertViewResultReturnsType<RecoverViewModel>(result);
            Assert.IsTrue(model.HasRecaptcha);

        }

        [TestMethod]
        public void When_RegisterGet_Then_ViewReturned()
        {
            // Arrange

            // Act
            var result = _sut.Register();

            // Assert
            var model = AssertViewResultReturnsType<RegisterViewModel>(result);
            Assert.IsTrue(model.HasRecaptcha);
        }

        [TestMethod]
        public async Task Given_ValidSubmissionData_When_Recover_Then_SavesEmailsAndSuccessViewReturned()
        {
            // Arrange
            RecoverViewModel model = new RecoverViewModel()
            {
                UserName = _testUserName
            };

            // Act
            var result = await _sut.Recover(model);

            // Assert
            AssertViewResultReturned(result, "RecoverSuccess");
            _services.AssertWasCalled(a => a.SendEmail(Arg<string>.Is.Anything, Arg<List<string>>.Is.Anything, Arg<List<string>>.Is.Anything,
                Arg<List<string>>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<bool>.Is.Anything));
            _context.AssertWasCalled(a => a.SaveChangesAsync());           

        }

        [TestMethod]
        public async Task Given_ValidSubmissionData_When_RecoverPassword_Then_SavesEmailsAndSuccessViewReturned()
        {
            // Arrange
            var model = new RecoverPasswordViewModel()
            {
                Id = _testUserId
            };
            _encryption.Expect(e => e.Encrypt(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<int>.Is.Anything, Arg<string>.Is.Anything, out Arg<string>.Out(_encryptedSecurityAnswer).Dummy)).Return(false);
            _userManager.Expect(a => a.ChangePasswordAsync(Arg<int>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything)).Return(Task.FromResult(new SEIdentityResult()));
            _userManager.Expect(a => a.LogOnAsync(Arg<string>.Is.Anything, Arg<bool>.Is.Anything)).Return(Task.FromResult(0));

            // Act
            var result = await _sut.RecoverPassword(model);

            // Assert
            AssertViewResultReturned(result, "RecoverPasswordSuccess");
            _context.AssertWasCalled(a => a.SaveChanges());

        }

		[TestMethod]
		public async Task Given_ValidSubmissionData_When_Register_Then_UserIsEmailedConfirmation()
		{
			// Arrange
			var collection = new NameValueCollection();
			collection.Add("Password", "password");
			collection.Add("ConfirmPassword", "password");
			collection.Add("User.UserName", _testUserName);
			collection.Add("User.FirstName", "First name");
			collection.Add("User.LastName", "Last Name");
			collection.Add("User.SecurityQuestionLookupItemId", "1");
			collection.Add("User.SecurityAnswer", "Bloggs");
			_userManager.Expect(a => a.CreateAsync(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<int>.Is.Anything, Arg<string>.Is.Anything)).Return(Task.FromResult(new SEIdentityResult()));

			// Act
			var result = await _sut.Register(new FormCollection(collection));

			// Assert
			AssertViewResultReturned(result, "RegisterSuccess");
			_services.AssertWasCalled(a => a.SendEmail(Arg<string>.Is.Anything, Arg<List<string>>.Is.Anything, Arg<List<string>>.Is.Anything,
				Arg<List<string>>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<bool>.Is.Anything));

		}

		[TestMethod]
        public void Given_ValidSubmissionData_When_RecoverPasswordGet_Then_ViewShown()
        {
            // Arrange
            var requestItems = new NameValueCollection();
            var passwordResetToken = "testreset1";
            requestItems.Add("PasswordResetToken", passwordResetToken);
            _httpRequest.Stub(a => a.QueryString).Return(requestItems);
            var user = _context.User.Where(u => u.Id == _testUserId).First();
            user.PasswordResetToken = passwordResetToken;
            user.PasswordResetExpiry = DateTime.UtcNow.AddMinutes(15);

            // Act
            var result = _sut.RecoverPassword();

            // Assert
            var model = AssertViewResultReturnsType<RecoverPasswordViewModel>(result);
            Assert.AreEqual(passwordResetToken, model.PasswordResetToken);
            Assert.AreEqual(_testUserId, model.Id);
            Assert.AreEqual("test question", model.SecurityQuestion);


        }

        [TestMethod]
        public void When_LogOff_Then_SessionAbandoned()
        {

            // Arrange
            _formsAuth.Expect(a => a.SignOut());
            _httpSession.Expect(h => h.Abandon());

            // Act
            var result = _sut.LogOff();

            // Assert
            AssertRedirectToActionReturned(result, "LogOn", "Account");
        }

        private void UserManagerAttemptsLoginWithResult(bool isSuccess)
        {

            _userManager.Expect(a => a.TryLogOnAsync(Arg<string>.Is.Anything, Arg<string>.Is.Anything))
                .Return(Task.FromResult<LogonResult>(new LogonResult() { Success = isSuccess }));
        }

    }
}
