using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SecurityEssentials.Controllers;
using SecurityEssentials.Model;
using SecurityEssentials.ViewModel;

namespace SecurityEssentials.Unit.Tests.Controllers
{
    [TestClass]
    public class UserControllerTest : BaseControllerTest
    {
        private UserController _sut;

        [TestInitialize]
        public void Setup()
        {
            BaseSetup();
            _sut = new UserController(AppSensor, Context, UserIdentity)
            {
                Url = new UrlHelper(new RequestContext(HttpContext, new RouteData()), new RouteCollection())
            };
            _sut.ControllerContext = new ControllerContext(HttpContext, new RouteData(), _sut);
        }

        [TestCleanup]
        public void Teardown()
        {
            VerifyAllExpectations();
        }

        [TestMethod]
        public void Given_UserExists_When_DisableGet_Then_PartialViewReturned()
        {
            // Arrange

            // Act
            var result = _sut.Disable(TestUserId);

            // Assert
            AssertPartialViewResultReturned(result, "_Disable");
        }

        [TestMethod]
        public void Given_UserExistsAndCurrentUser_When_DisablePost_Then_UserDisabledAndSuccessJsonReturned()
        {
            // Arrange
            var collection = new NameValueCollection();

            // Act
            var result = _sut.Disable(TestUserId, new FormCollection(collection));

            // Assert
            var dataReturned = AssertJsonResultReturned(result);
            Assert.AreEqual(true, dataReturned["success"]);
            Assert.AreEqual("", dataReturned["message"]);
            var user = Context.User.First(u => u.Id == TestUserId);
            Assert.IsFalse(user.Enabled);
            Context.AssertWasCalled(a => a.SaveChanges());
        }

        [TestMethod]
        public void Given_UserExists_When_EditGet_Then_ViewReturned()
        {
            // Arrange
            UserIdentity.Expect(a => a.GetUserId(Arg<Controller>.Is.Anything)).Return(TestUserId);

            // Act
            var result = _sut.Edit(TestUserId);

            // Assert
            var viewModel = AssertViewResultReturnsType<UserViewModel>(result);
            Assert.AreEqual(TestUserId, viewModel.User.Id);
            Assert.IsTrue(viewModel.IsOwnProfile);
        }

        [TestMethod]
        public void Given_SubmissionCorrect_When_EditPost_Then_ViewReturned()
        {
            // Arrange
            var collection = new NameValueCollection {{"FirstName", "new"}, {"LastName", "name"}};

            UserIdentity.Expect(a => a.GetUserId(Arg<Controller>.Is.Anything)).Return(TestUserId);

            // Act
            var result = _sut.Edit(TestUserId, new FormCollection(collection));

            // Assert
            var viewResult = AssertViewResultReturned(result, "Edit");
            Context.AssertWasCalled(a => a.SaveChanges());
            Assert.AreEqual("Your account information has been changed", viewResult.ViewBag.StatusMessage);
        }

        [TestMethod]
        public void When_Index_Then_ViewReturned()
        {
            // Arrange

            // Act
            var result = _sut.Index();

            // Assert
            AssertViewResultReturnsType<UsersViewModel>(result);
        }

        [TestMethod]
        public void Given_UserExists_When_LogGet_Then_ViewReturned()
        {
            // Arrange
            UserIdentity.Expect(a => a.GetUserId(Arg<Controller>.Is.Anything)).Return(TestUserId);

            // Act
            var result = _sut.Log(TestUserId);

            // Assert
            var viewModel = AssertViewResultReturnsType<List<UserLog>>(result);
            Assert.AreEqual(2, viewModel.Count);
        }

        [TestMethod]
        public void Given_DataExists_When_Read_Then_JsonReturned()
        {
            // Arrange
            var requestItems = new NameValueCollection {{"sort[0][dir]", ""}, {"sort[0][field]", ""}};
            // TODO: Fix this as a.Form doesn't work
            HttpRequest.Stub(a => a.Form).Return(requestItems);

            // Act
            var result = _sut.Read();

            // Assert
            var dataReturned = AssertJsonResultReturned(result);
            // TODO: Validate it returns 1 item
        }
    }
}