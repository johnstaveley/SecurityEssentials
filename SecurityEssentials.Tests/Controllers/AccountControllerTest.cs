using System;
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
using Rhino.Mocks.Constraints;
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

        [TestInitialize]
        public void Setup()
        {
            _lastAccountActivity = DateTime.Parse("2016-05-10");
            _configuration = MockRepository.GenerateStub<IAppConfiguration>();
            _context = MockRepository.GenerateStub<ISEContext>();
            _context.User = new TestDbSet<User>();
            _context.User.Add(new User() { Id = 5, FirstName = _firstName, UserLogs = new List<UserLog>() {
                new UserLog() { Id = 2, DateCreated = DateTime.Parse("2016-06-10"), Description = "did stuff" },
                new UserLog() { Id = 1, DateCreated = _lastAccountActivity, Description = "did old stuff" }
            } });
            _userManager = MockRepository.GenerateMock<IUserManager>();
            _recaptcha = MockRepository.GenerateMock<IRecaptcha>();
            _services = MockRepository.GenerateMock<IServices>();
            _userIdentity = MockRepository.GenerateMock<IUserIdentity>();
            _sut = new AccountController(_configuration, _context, _userManager, _recaptcha, _services, _userIdentity);
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
            AssertViewResultReturned(result, null);
            var viewResult = (ViewResult) result;
            Assert.AreEqual("Your password has been changed.", viewResult.ViewBag.StatusMessage);

        }

        [TestMethod, Ignore]
        public async Task GIVEN_CorrectInformationProvided_WHEN_ChangePassword_THEN_SavesRedirectsAndEmails()
        {
            // Arrange
            var model = new ViewModel.ChangePassword()
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
            AssertRedirectToActionReturned(result, "ChangePasswordSuccess", "Account");
            _services.AssertWasCalled(a => a.SendEmail(Arg<string>.Is.Anything, Arg<List<string>>.Is.Anything, Arg<List<string>>.Is.Anything, 
                Arg<List<string>>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<bool>.Is.Anything));
            _context.AssertWasCalled(a => a.SaveChanges());

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

        public void AssertRedirectToActionReturned(ActionResult result, string action, string controller)
        {
            Assert.IsNotNull(result, "No result was returned from controller");
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult), "Not RedirectToRouteResult returned from controller");
            var redirectResult = (RedirectToRouteResult)result;
            Assert.AreEqual(action, redirectResult.RouteValues.Values.ToList()[0]);
            Assert.AreEqual(controller, redirectResult.RouteValues.Values.ToList()[1]);

        }

    }
}
