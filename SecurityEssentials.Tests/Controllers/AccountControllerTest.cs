using System;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecurityEssentials.Controllers;
using Rhino.Mocks;
using SecurityEssentials.Core;
using SecurityEssentials.Core.Identity;
using SecurityEssentials.Unit.Tests.TestDbSet;
using SecurityEssentials.Model;
using System.Collections.Generic;
using SecurityEssentials.ViewModel;
using System.Threading.Tasks;
using System.Web.Routing;
using System.Web;

namespace SecurityEssentials.Unit.Tests.Controllers
{
    [TestClass]
    public class AccountControllerTest
    {

        private AccountController _sut;
        private IAppConfiguration _configuration;
        private IEncryption _encryption;
        private ISEContext _context;
        private HttpContextBase _httpContext;
        private HttpRequestBase _httpRequest;
        private IUserManager _userManager;
        private IRecaptcha _recaptcha;
        private string _returnUrl = null;
        private IServices _services;
        private IUserIdentity _userIdentity;
        private DateTime _lastAccountActivity;
        private string _firstName = "Bob";
        private string _testUserName = "testuserName@test.com";
        private int _testUserId = 5;
        private string _encryptedSecurityAnswer = "encryptedSecurityAnswer";

        [TestInitialize]
        public void Setup()
        {
            _lastAccountActivity = DateTime.Parse("2016-05-10");
            _configuration = MockRepository.GenerateStub<IAppConfiguration>();
            _configuration.Stub(a => a.HasRecaptcha).Return(true);
            _context = MockRepository.GenerateStub<ISEContext>();
            _context.LookupItem = new TestDbSet<LookupItem>();
            _context.User = new TestDbSet<User>();
            _context.User.Add(new User()
            {
                Id = _testUserId,
                Enabled = true,
                Approved = true,
                EmailVerified = true,
                FirstName = _firstName,
                UserName = _testUserName,
                EmailConfirmationToken = "test1",
                SecurityQuestionLookupItemId = 1,
                SecurityQuestionLookupItem = new LookupItem() { Id = 1, Description = "test question" },
                SecurityAnswer = _encryptedSecurityAnswer,
                UserLogs = new List<UserLog>() {
                new UserLog() { Id = 2, DateCreated = DateTime.Parse("2016-06-10"), Description = "did stuff" },
                new UserLog() { Id = 1, DateCreated = _lastAccountActivity, Description = "did old stuff" }
            } });
            _context.Stub(a => a.SaveChangesAsync()).Return(Task.FromResult(0));
            _encryption = MockRepository.GenerateMock<IEncryption>();
            _userManager = MockRepository.GenerateMock<IUserManager>();
            _recaptcha = MockRepository.GenerateMock<IRecaptcha>();
            _services = MockRepository.GenerateMock<IServices>();
            _userIdentity = MockRepository.GenerateMock<IUserIdentity>();
            _sut = new AccountController(_configuration, _encryption, _context, _userManager, _recaptcha, _services, _userIdentity);
            _httpContext = MockRepository.GenerateMock<HttpContextBase>();
            _httpRequest = MockRepository.GenerateMock<HttpRequestBase>();
            _httpRequest.Stub(x => x.Url).Return(new Uri("http://localhost/a", UriKind.Absolute));
            _sut.Url = new UrlHelper(new RequestContext(_httpContext, new RouteData()), new RouteCollection());
            _httpContext.Stub(c => c.Request).Return(_httpRequest);
            _sut.ControllerContext = new ControllerContext(_httpContext, new RouteData(), _sut);
        }

        [TestCleanup]
        public void Teardown()
        {
            _encryption.VerifyAllExpectations();
            _httpContext.VerifyAllExpectations();
            _httpRequest.VerifyAllExpectations();
            _userManager.VerifyAllExpectations();
            _recaptcha.VerifyAllExpectations();
            _services.VerifyAllExpectations();
            _userIdentity.VerifyAllExpectations();
        }

        [TestMethod]
        public void GIVEN_PreviousUserActivity_WHEN_UserLands_THEN_LastActivityDisplayed()
        {
            // Arrange
            _userIdentity.Expect(u => u.GetUserId(Arg<Controller>.Is.Anything)).Return(5);

            // Act
            var result = _sut.Landing();

            // Assert
            var data = AssertViewResultReturnsType<LandingViewModel>(result);
            Assert.AreEqual(_lastAccountActivity.ToString("dd/MM/yyyy HH:mm"), data.LastAccountActivity);
            Assert.AreEqual(_firstName, data.FirstName);
            
        }

        [TestMethod]
        public void GIVEN_AccountNotFound_WHEN_UserLands_THEN_HttpNotFound()
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
        public async Task GIVEN_CredentialsCorrect_WHEN_Logon_THEN_RedirectsToLandingPage()
        {
            // Arrange
            LogOn logon = new LogOn() { UserName = "joeblogs", Password = "password", RememberMe = false };
            UserManagerAttemptsLoginWithResult(true);
            _userManager.Expect(a => a.SignInAsync(Arg<string>.Is.Anything, Arg<bool>.Is.Anything))
                .Return(Task.FromResult(0));

            // Act
            var result = await _sut.LogOn(logon, _returnUrl );

            // Assert
            AssertRedirectToActionReturned(result, "Landing", "Account");
        }

        [TestMethod]
        public async Task GIVEN_CredentialsIncorrect_WHEN_Logon_THEN_ErrorViewReturned()
        {
            // Arrange
            LogOn logon = new LogOn() { UserName = "joeblogs", Password = "password1", RememberMe = false };
            UserManagerAttemptsLoginWithResult(false);

            // Act
            var result = await _sut.LogOn(logon, _returnUrl);

            // Assert
            AssertViewResultWithError(result, "Invalid credentials or account is locked");
            _userManager.AssertWasNotCalled(a => a.SignInAsync(Arg<string>.Is.Anything, Arg<bool>.Is.Anything));
        }

        [TestMethod]
        public void GIVEN_UserAuthenticated_WHEN_LogonViewRequested_THEN_RedirectToLandingPage()
        {
            // Arrange
            _httpRequest.Stub(x => x.IsAuthenticated).Return(true);

            // Act
            var result = _sut.LogOn(_returnUrl);

            // Assert
            AssertRedirectToActionReturned(result, "Landing", "Account");

        }

        [TestMethod]
        public void GIVEN_UserIsNotAuthenticated_WHEN_LogonViewRequested_THEN_ShowsView()
        {
            // Arrange

            // Act
            var result = _sut.LogOn(_returnUrl);

            // Assert
            AssertViewResultReturned(result, "LogOn");

        }

        [TestMethod]
        public void GIVEN_SuccessCodeProvided_WHEN_ChangePassword_THEN_ShowsViewWithMessage()
        {
            // Arrange
            var message = AccountController.ManageMessageId.ChangePasswordSuccess;

            // Act
            var result = _sut.ChangePassword(message);

            // Assert
            var model = AssertViewResultReturnsType<ChangePasswordViewModel>(result);
            Assert.IsTrue(model.HasRecaptcha);
            var viewResult = (ViewResult) result;
            Assert.AreEqual("Your password has been changed.", viewResult.ViewBag.StatusMessage);

        }

        [TestMethod]
        public async Task GIVEN_CorrectInformationProvided_WHEN_ChangePassword_THEN_SavesRedirectsAndEmails()
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
            AssertRedirectToActionReturned(result, SecurityEssentials.Controllers.AccountController.ManageMessageId.ChangePasswordSuccess, "ChangePassword");
            _services.AssertWasCalled(a => a.SendEmail(Arg<string>.Is.Anything, Arg<List<string>>.Is.Anything, Arg<List<string>>.Is.Anything, 
                Arg<List<string>>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<bool>.Is.Anything));
            _context.AssertWasCalled(a => a.SaveChanges());

        }

        [TestMethod]
        public async Task WHEN_ChangeSecurityInformationGet_THEN_ViewReturned()
        {
            // Arrange

            // Act
            var result = await _sut.ChangeSecurityInformation();

            // Assert
            var model = AssertViewResultReturnsType<ChangeSecurityInformationViewModel>(result);
            Assert.IsTrue(model.HasRecaptcha);
        }

        [TestMethod]
        public async Task WHEN_ChangeSecurityInformation_THEN_ViewReturned()
        {
            // Arrange
            var model = new ChangeSecurityInformationViewModel()
            {
                SecurityAnswer = "a",
                SecurityAnswerConfirm = "a"
            };
            _userManager.Expect(a => a.TrySignInAsync(Arg<string>.Is.Anything, Arg<string>.Is.Anything))
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
        public async Task GIVEN_RequestVerificationToken_WHEN_EmailVerify_THEN_UserStatusUpdated()
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
        public void WHEN_RecoverGet_THEN_ViewReturned()
        {
            // Arrange

            // Act
            var result = _sut.Recover();

            // Assert
            var model = AssertViewResultReturnsType<RecoverViewModel>(result);
            Assert.IsTrue(model.HasRecaptcha);

        }

        [TestMethod]
        public async Task WHEN_RegisterGet_THEN_ViewReturned()
        {
            // Arrange

            // Act
            var result = await _sut.Register();

            // Assert
            var model = AssertViewResultReturnsType<RegisterViewModel>(result);
            Assert.IsTrue(model.HasRecaptcha);
        }

        [TestMethod]
        public async Task GIVEN_ValidSubmissionData_WHEN_Recover_THEN_SavesEmailsAndSuccessViewReturned()
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
            _context.AssertWasCalled(a => a.SaveChanges());           

        }

        [TestMethod]
        public async Task GIVEN_ValidSubmissionData_WHEN_RecoverPassword_THEN_SavesEmailsAndSuccessViewReturned()
        {
            // Arrange
            var model = new RecoverPasswordViewModel()
            {
                Id = _testUserId
            };
            _encryption.Expect(e => e.Encrypt(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<int>.Is.Anything, Arg<string>.Is.Anything, out Arg<string>.Out(_encryptedSecurityAnswer).Dummy)).Return(false);
            _userManager.Expect(a => a.ChangePasswordFromTokenAsync(Arg<int>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything)).Return(Task.FromResult(new SEIdentityResult()));
            _userManager.Expect(a => a.SignInAsync(Arg<string>.Is.Anything, Arg<bool>.Is.Anything)).Return(Task.FromResult(0));

            // Act
            var result = await _sut.RecoverPassword(model);

            // Assert
            AssertViewResultReturned(result, "RecoverPasswordSuccess");
            _context.AssertWasCalled(a => a.SaveChanges());

        }

		[TestMethod]
		public async Task GIVEN_ValidSubmissionData_WHEN_Register_THEN_UserIsEmailedConfirmation()
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
        public async Task GIVEN_ValidSubmissionData_WHEN_RecoverPasswordGet_THEN_ViewShown()
        {
            // Arrange
            var requestItems = new NameValueCollection();
            var passwordResetToken = "testreset1";
            requestItems.Add("PasswordResetToken", passwordResetToken);
            _httpRequest.Stub(a => a.QueryString).Return(requestItems);
            var user = _context.User.Where(u => u.Id == _testUserId).First();
            user.PasswordResetToken = passwordResetToken;
            user.PasswordResetExpiry = DateTime.Now.AddMinutes(15);

            // Act
            var result = await _sut.RecoverPassword();

            // Assert
            var model = AssertViewResultReturnsType<RecoverPasswordViewModel>(result);
            Assert.AreEqual(passwordResetToken, model.PasswordResetToken);
            Assert.AreEqual(_testUserId, model.Id);
            Assert.AreEqual("test question", model.SecurityQuestion);


        }

        private void UserManagerAttemptsLoginWithResult(bool isSuccess)
        {

            _userManager.Expect(a => a.TrySignInAsync(Arg<string>.Is.Anything, Arg<string>.Is.Anything))
                .Return(Task.FromResult<LogonResult>(new LogonResult() { Success = isSuccess }));
        }

        public void AssertViewResultWithError(ActionResult actionResult, string errorValue)
        {
            Assert.IsNotNull(actionResult, "No result was returned from controller");
            Assert.IsInstanceOfType(actionResult, typeof(ViewResult), "Not ViewResult returned from controller");
            var viewResult = (ViewResult) actionResult;
            Assert.IsNotNull(viewResult.ViewData.ModelState);
            Assert.IsFalse(viewResult.ViewData.ModelState.ToList().Count == 0);
            var error = viewResult.ViewData.ModelState.ToList()[0].Value.Errors.ToList();
            Assert.AreEqual(errorValue, error[0].ErrorMessage);

        }

        public T AssertViewResultReturnsType<T>(ActionResult actionResult)
        {
            Assert.IsNotNull(actionResult, "No result was returned from controller");
            Assert.IsInstanceOfType(actionResult, typeof(ViewResult), "Not ViewResult returned from controller");
            var viewResult = (ViewResult) actionResult;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(T), "Not expected type returned as data");
            return (T)viewResult.ViewData.Model;

        }

        public void AssertViewResultReturned(ActionResult actionResult, string viewName)
        {
            Assert.IsNotNull(actionResult, "No result was returned from controller");
            Assert.IsInstanceOfType(actionResult, typeof(ViewResult), "Not ViewResult returned from controller");
            var viewResult = (ViewResult)actionResult;
            if (!string.IsNullOrEmpty(viewName))
            {
                Assert.AreEqual(viewName, viewResult.ViewName);
            }

        }

        public void AssertRedirectToActionReturned(ActionResult result, object action, string controller)
        {
            Assert.IsNotNull(result, "No result was returned from controller");
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult), "Not RedirectToRouteResult returned from controller");
            var redirectResult = (RedirectToRouteResult)result;
            Assert.AreEqual(action, redirectResult.RouteValues.Values.ToList()[0]);
            Assert.AreEqual(controller, redirectResult.RouteValues.Values.ToList()[1]);

        }

    }
}
