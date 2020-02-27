using NUnit.Framework;
using Rhino.Mocks;
using SecurityEssentials.Controllers;
using SecurityEssentials.Core;
using SecurityEssentials.Core.Attributes;
using SecurityEssentials.Core.Constants;
using SecurityEssentials.Core.Identity;
using SecurityEssentials.Model;
using SecurityEssentials.ViewModel;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;

namespace SecurityEssentials.Unit.Tests.Controllers
{
    [TestFixture]
    public class UserControllerTest : BaseControllerTest
    {

        private IAppConfiguration _configuration;
        private IAppSensor _appSensor;
        private IHttpCache _httpCache;
        private IServices _services;
        private UserController _sut;
        private IUserManager _userManager;

        [SetUp]
        public void Setup()
        {
            BaseSetup();
            _appSensor = MockRepository.GenerateMock<IAppSensor>();
            _configuration = MockRepository.GenerateStub<IAppConfiguration>();
            _httpCache = MockRepository.GenerateMock<IHttpCache>();
            _services = MockRepository.GenerateMock<IServices>();
            _userManager = MockRepository.GenerateMock<IUserManager>();
            _sut = new UserController(_appSensor, _configuration, Context, _httpCache, UserIdentity, _userManager, _services)
            {
                Url = new UrlHelper(new RequestContext(HttpContext, new RouteData()), new RouteCollection())
            };
            _sut.ControllerContext = new ControllerContext(HttpContext, new RouteData(), _sut);
        }

        [TearDown]
        public void Teardown()
        {
            VerifyAllExpectations();
            _appSensor.VerifyAllExpectations();
            _httpCache.VerifyAllExpectations();
            _services.VerifyAllExpectations();
            _userManager.VerifyAllExpectations();
        }

        [Test]
        public void When_ControllerCreated_Then_IsDecoratedWithSeAuthorize()
        {
            var type = _sut.GetType();
            var attributes = type.GetCustomAttributes(typeof(SeAuthorizeAttribute), true);
            Assert.IsTrue(attributes.Any(), "No Authorize Attribute found");
        }

        [Test]
        public void When_ControllerCreated_Then_IsDecoratedWithNoCache()
        {
            var type = _sut.GetType();
            var attributes = type.GetCustomAttributes(typeof(NoCacheAttribute), true);
            Assert.IsTrue(attributes.Any(), "No NoCache Attribute found");
        }

        [Test]
        public void Given_UserDoesNotExist_When_DeleteGet_Then_NotFoundReturned()
        {

            // Arrange

            // Act
            var result = _sut.Delete(666);

            // Assert
            AssertNotFoundReturned(result);

        }

        [Test]
        public void Given_UserExists_When_DeleteGet_Then_ViewReturned()
        {

            // Arrange

            // Act
            var result = _sut.Delete(TestUserId);

            // Assert
            AssertViewResultReturnsType<User>(result);

        }

        [Test]
        public void Given_UserDoesNotExist_When_DeleteUserPost_Then_NotFoundReturned()
        {

            // Arrange

            // Act
            var result = _sut.Delete(666, null);

            // Assert
            AssertNotFoundReturned(result);

        }

        [Test, Ignore("Wait for test case where user cannot be deleted")]
        public void Given_UserHasExistingData_When_DeleteUserPost_Then_ErrorMessageReturned()
        {

            // Arrange
            var user = Context.User.Single(a => a.Id == TestUserId);
            // TODO: Mark user as one that cannot be deleted i.e. has audits

            // Act
            var result = _sut.Delete(TestUserId, null);

            // Assert
            var viewModel = AssertViewResultReturned(result, "Delete");
            Assert.That(viewModel.ViewBag.Message, Is.EqualTo("This user has data associated with it and cannot be deleted"));
        }

        [Test]
        public void Given_UserExists_When_DeleteUserPost_Then_RedirectToActionReturned()
        {

            // Arrange
            var user = Context.User.Single(a => a.Id == TestUserId);
            user.PreviousPasswords.Add(new PreviousPassword());
            user.UserLogs.Add(new UserLog());
            user.UserRoles.Add(new UserRole());

            // Act
            var result = _sut.Delete(TestUserId, null);

            // Assert
            AssertRedirectToActionReturned(result, "Index", "User");
        }


        [Test]
        public void Given_UserExists_When_EditGet_Then_ViewReturned()
        {

            // Arrange
            ExpectActingUserIsAdmin(false);
            ExpectGetUserId();

            // Act
            var result = _sut.Edit(TestUserId);

            // Assert
            var viewModel = AssertViewResultReturnsType<UserViewModel>(result);
            Assert.AreEqual(TestUserId, viewModel.User.Id);
            Assert.IsTrue(viewModel.IsOwnProfile);

        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Given_SubmissionCorrect_When_EditPost_Then_ViewReturned(bool isAdmin)
        {

            // Arrange
            ExpectGetUserId();
            ExpectActingUserIsAdmin(isAdmin);
            ExpectGetRequester();
            var collection = new NameValueCollection { { "User.UserName", "new@new.test.net" }, { "User.FirstName", "new" }, { "User.LastName", "name" } };

            // Act
            var result = _sut.Edit(TestUserId, new FormCollection(collection));

            // Assert
            Context.AssertWasCalled(a => a.SaveChanges());
            var modifiedUser = Context.User.Single(u => u.Id == TestUserId);
            if (isAdmin)
            {
                _services.AssertWasCalled(a => a.SendEmail(Arg<string>.Is.Anything, Arg<List<string>>.Matches(b => b.Contains("new@new.test.net")), Arg<List<string>>.Is.Anything, Arg<List<string>>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<bool>.Is.Anything));
                Assert.That(modifiedUser.UserName, Is.EqualTo("new@new.test.net"));
            }
            else
            {
                Assert.That(modifiedUser.UserName, Is.Not.EqualTo("new@new.test.net"));
            }
            var viewResult = AssertViewResultReturned(result, "Edit");
            Assert.AreEqual("Your account information has been saved", viewResult.ViewBag.StatusMessage);

        }

        [Test]
        public void Given_UserNameAlreadyInUse_When_EditPost_Then_ErrorReturned()
        {

            // Arrange
            ExpectGetUserId();
            ExpectActingUserIsAdmin(true);
            Context.User.Add(new User { Id = 52, UserName = "bob@test.net" });
            var collection = new NameValueCollection { { "User.UserName", "bob@test.net" }, { "User.FirstName", "new" }, { "User.LastName", "name" } };

            // Act
            var result = _sut.Edit(TestUserId, new FormCollection(collection));

            // Assert
            Context.AssertWasNotCalled(a => a.SaveChanges());
            AssertViewResultWithError(result, "This username is already in use");

        }

        [Test]
        public void Given_UserLoggedOn_When_EditCurrentGet_Then_RedirectToActionForCurrentUserReturned()
        {

            // Arrange
            UserIdentity.Expect(a => a.GetUserId(Arg<Controller>.Is.Anything)).Return(TestUserId);

            // Act
            var result = _sut.EditCurrent();

            // Assert
            AssertRedirectToActionReturned(result, TestUserId, "Edit", "User");

        }

        [Test]
        public void When_Index_Then_ViewReturned()
        {

            // Arrange

            // Act
            var result = _sut.Index();

            // Assert
            AssertViewResultReturned(result, "Index");

        }

        [Test]
        public void Given_UserExists_When_LogGet_Then_ViewReturned()
        {

            // Arrange
            ExpectGetUserId();

            // Act
            var result = _sut.Log(TestUserId);

            // Assert
            var viewModel = AssertViewResultReturnsType<UserLogViewModel>(result);
            Assert.AreEqual(2, viewModel.UserLogs.Count);
        }

        [Test]
        public void Given_UserExists_When_MakeAdminGet_Then_ViewReturned()
        {

            // Arrange

            // Act
            var result = _sut.MakeAdmin(TestUserId);

            // Assert
            var viewModel = AssertViewResultReturnsType<User>(result);
            Assert.AreEqual(TestUserId, viewModel.Id);
        }

        [Test]
        public void Given_UserNotFound_When_MakeAdminGet_Then_NotFoundReturned()
        {

            // Arrange

            // Act
            var result = _sut.MakeAdmin(666);

            // Assert
            AssertNotFoundReturned(result);
        }

        [Test]
        public void Given_UserIsAdmin_When_MakeAdminGet_Then_BadRequestReturned()
        {

            // Arrange
            var user = Context.User.Single(a => a.Id == TestUserId);
            user.UserRoles.Add(new UserRole { RoleId = Consts.Roles.Admin });

            // Act
            var result = _sut.MakeAdmin(TestUserId);

            // Assert
            AssertBadRequestReturned(result);
        }

        [Test]
        public void Given_UserNotFound_When_MakeAdminPost_Then_NotFoundReturned()
        {

            // Arrange

            // Act
            var result = _sut.MakeAdmin(666, null);

            // Assert
            AssertNotFoundReturned(result);
        }

        [Test]
        public void Given_UserIsAdmin_When_MakeAdminPost_Then_BadRequestReturned()
        {

            // Arrange
            var user = Context.User.Single(a => a.Id == TestUserId);
            user.UserRoles.Add(new UserRole { RoleId = Consts.Roles.Admin });

            // Act
            var result = _sut.MakeAdmin(TestUserId, null);

            // Assert
            AssertBadRequestReturned(result);
        }

        [Test]
        public void Given_UserExists_When_MakeAdminPost_Then_ViewReturned()
        {

            // Arrange

            // Act
            var result = _sut.MakeAdmin(TestUserId, null);

            // Assert
            AssertRedirectToActionReturned(result, TestUserId, "Edit", "User");
            Context.AssertWasCalled(a => a.SaveChanges());
            var user = Context.User.Single(a => a.Id == TestUserId);
            Assert.That(user.UserRoles.Count, Is.EqualTo(1), "User Role has not been added");
            Assert.That(user.UserRoles.Any(a => a.RoleId == Consts.Roles.Admin), Is.True);
            Assert.That(user.UserLogs.Count, Is.EqualTo(3), "User Log has not been added");
            Assert.IsTrue(user.UserLogs.Any(a => a.Description.Contains("made a system admin")));

        }

        [Test]
        [TestCase(null, 1, 5, null, null, 5, "User A")] // Page 1, standard sort
        [TestCase(null, 2, 5, null, null, 4, "User F")] // Page 2, standard sort
        [TestCase(null, 1, 5, "FullName", "desc", 5, "User I")] // reverse sort on field, page 1
        [TestCase(null, 2, 5, "FullName", "desc", 4, "User D")] // reverse sort on field, page 2
        [TestCase("G", 1, 5, null, null, 1, "User G")] // Search for G
        [TestCase("User F", 1, 5, null, null, 1, "User F")] // Search for full name User F
        public void Given_AdminUserAndDataExists_When_Read_Then_JsonReturned(string searchName, int pageNumber, int numberOfRowsInPage, string sortIndex, string sortDirection, int expectedNumberOfRows, string expectedFirstFullName)
        {
            // Arrange
            var user = Context.User.First();
            Context.User.Remove(user);
            Context.User.Add(new User { Id = 1, FirstName = "User", LastName = "A" });
            Context.User.Add(new User { Id = 2, FirstName = "User", LastName = "B" });
            Context.User.Add(new User { Id = 3, FirstName = "User", LastName = "C" });
            Context.User.Add(new User { Id = 4, FirstName = "User", LastName = "D" });
            Context.User.Add(new User { Id = 5, FirstName = "User", LastName = "E" });
            Context.User.Add(new User { Id = 6, FirstName = "User", LastName = "F" });
            Context.User.Add(new User { Id = 7, FirstName = "User", LastName = "G" });
            Context.User.Add(new User { Id = 8, FirstName = "User", LastName = "H" });
            Context.User.Add(new User { Id = 9, FirstName = "User", LastName = "I" });

            // Act
            var result = _sut.Read(pageNumber, numberOfRowsInPage, searchName, sortDirection, sortIndex);

            // Assert
            var model = AssertJsonResultReturned(result);
            Assert.That(model["data"].Length, Is.EqualTo(expectedNumberOfRows), "User rows returned did not match");
            if (expectedNumberOfRows > 0)
            {
                Assert.That(model["data"][0]["FullName"], Is.EqualTo(expectedFirstFullName));
            }
        }

        [Test]
        public void Given_AdminUserExists_When_RemoveAdminGet_Then_ViewReturned()
        {

            // Arrange
            var user = Context.User.Single(a => a.Id == TestUserId);
            user.UserRoles.Add(new UserRole { RoleId = Consts.Roles.Admin });
            ExpectGetUserId();

            // Act
            var result = _sut.RemoveAdmin(TestUserId);

            // Assert
            var viewModel = AssertViewResultReturnsType<RemoveRoleViewModel>(result);
            Assert.AreEqual(TestUserId, viewModel.User.Id);
        }

        [Test]
        public void Given_UserNotFound_When_RemoveAdminGet_Then_NotFoundReturned()
        {

            // Arrange

            // Act
            var result = _sut.RemoveAdmin(666);

            // Assert
            AssertNotFoundReturned(result);
        }

        [Test]
        public void Given_UserIsNotAdmin_When_RemoveAdminGet_Then_BadRequestReturned()
        {

            // Arrange

            // Act
            var result = _sut.RemoveAdmin(TestUserId);

            // Assert
            AssertBadRequestReturned(result);
        }

        [Test]
        public void Given_UserNotFound_When_RemoveAdminPost_Then_NotFoundReturned()
        {

            // Arrange

            // Act
            var result = _sut.RemoveAdmin(666, null);

            // Assert
            AssertNotFoundReturned(result);
        }

        [Test]
        public void Given_UserIsNotAdmin_When_RemoveAdminPost_Then_BadRequestReturned()
        {

            // Arrange

            // Act
            var result = _sut.RemoveAdmin(TestUserId, null);

            // Assert
            AssertBadRequestReturned(result);
        }

        [Test]
        public void Given_AdminUserExists_When_RemoveAdminPost_Then_ViewReturned()
        {

            // Arrange
            var user = Context.User.Single(a => a.Id == TestUserId);
            user.UserRoles.Add(new UserRole { RoleId = Consts.Roles.Admin });

            // Act
            var result = _sut.RemoveAdmin(TestUserId, null);

            // Assert
            AssertRedirectToActionReturned(result, TestUserId, "Edit", "User");
            AssertAdminUserRemoved(user);
        }

        [Test]
        public void Given_RemoveAdminFromOwnProfile_When_RemoveAdminPost_Then_LoggedOut()
        {

            // Arrange
            var user = Context.User.Single(a => a.Id == TestUserId);
            user.UserRoles.Add(new UserRole { RoleId = Consts.Roles.Admin });
            ExpectGetUserId();


            // Act
            var result = _sut.RemoveAdmin(TestUserId, null);

            // Assert
            AssertRedirectToActionReturned(result, "Logon", "Account");
            AssertAdminUserRemoved(user);
            _userManager.AssertWasCalled(a => a.SignOut());
        }

        [Test]
        public void Given_AdminExists_When_ResetPasswordGet_Then_ViewReturned()
        {

            // Arrange

            // Act
            var result = _sut.ResetPassword(TestUserId);

            // Assert
            var viewModel = AssertViewResultReturnsType<User>(result);
            Assert.AreEqual(TestUserId, viewModel.Id);
        }

        [Test]
        public void Given_UserNotFound_When_ResetPasswordGet_Then_NotFoundReturned()
        {

            // Arrange

            // Act
            var result = _sut.ResetPassword(666);

            // Assert
            AssertNotFoundReturned(result);
        }

        [Test]
        public async Task Given_UserNotFound_When_ResetPasswordPost_Then_NotFoundReturned()
        {

            // Arrange

            // Act
            var result = await _sut.ResetPassword(666, null);

            // Assert
            AssertNotFoundReturned(result);
        }

        [Test]
        public async Task Given_UserManagerErrors_When_ResetPasswordPost_Then_ViewReturned()
        {

            // Arrange
            _userManager.Expect(a => a.ResetPasswordAsync(TestUserId, TestUserName)).Return(Task.FromResult(new SeIdentityResult("Error")));
            ExpectGetUserName(TestUserName);

            // Act
            var result = await _sut.ResetPassword(TestUserId, null);

            // Assert
            var viewResult = AssertViewResultReturned(result, "ResetPassword");
            Assert.NotNull(viewResult.ViewBag.Message, "ViewBag Message Return should be set");
            Assert.That(viewResult.ViewBag.Message, Is.EqualTo("An error occurred whilst trying to perform this action"));
        }

        [Test]
        public async Task Given_UserManagerSucceeds_When_ResetPasswordPost_Then_RedirectToActionReturned()
        {

            // Arrange
            _userManager.Expect(a => a.ResetPasswordAsync(TestUserId, TestUserName)).Return(Task.FromResult(new SeIdentityResult()));
            ExpectGetUserName(TestUserName);

            // Act
            var result = await _sut.ResetPassword(TestUserId, null);

            // Assert
            AssertRedirectToActionReturned(result, TestUserId, "Edit", "User");
        }

        private void AssertAdminUserRemoved(User user)
        {
            Context.AssertWasCalled(a => a.SaveChanges());
            Context.AssertWasCalled(a => a.SetDeleted(Arg<UserRole>.Matches(b => b.RoleId == Consts.Roles.Admin)));
            Assert.That(user.UserLogs.Count, Is.EqualTo(3), "User Log has not been added");
            Assert.IsTrue(user.UserLogs.Any(a => a.Description.Contains("had administrator privileges removed")));
        }


    }
}
