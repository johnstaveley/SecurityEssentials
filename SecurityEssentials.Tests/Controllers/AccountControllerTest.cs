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

namespace SecurityEssentials.Unit.Tests.Controllers
{
    [TestClass]
    public class AccountControllerTest
    {

        private AccountController _sut;
        private IAppConfiguration _configuration;
        private ISEContext _context;
        private IUserManager _userManager;
        private IRecaptcha _recaptcha;
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
            //_context.User = 
            _userManager = MockRepository.GenerateMock<IUserManager>();
            _recaptcha = MockRepository.GenerateMock<IRecaptcha>();
            _services = MockRepository.GenerateMock<IServices>();
            _userIdentity = MockRepository.GenerateMock<IUserIdentity>();
            _sut = new AccountController(_configuration, _context, _userManager, _recaptcha, _services, _userIdentity);
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
        public void GIVEN_CredentialsCorrect_WHEN_Logon_THEN_RedirectsToLandingPage()
        {
            // Arrange
            _userIdentity.Expect(u => u.GetUserId(Arg<Controller>.Is.Anything)).Return(4);

            // Act
            var result = _sut.Landing();

            // Assert
            Assert.IsNotNull(result, "No result was returned from controller");
            Assert.IsInstanceOfType(result, typeof(HttpNotFoundResult), "Not HttpNotFoundResult returned from controller");

        }


        public T AssertViewResultReturnsType<T>(ActionResult actionResult)
        {
            Assert.IsNotNull(actionResult, "No result was returned from controller");
            Assert.IsInstanceOfType(actionResult, typeof(ViewResult), "Not ViewResult returned from controller");
            var viewResult = (ViewResult) actionResult;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(T), "Not expected type returned as data");
            return (T)viewResult.ViewData.Model;

        }
    }
}
