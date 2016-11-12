using System.Collections.Specialized;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecurityEssentials.Controllers;
using Rhino.Mocks;
using SecurityEssentials.Model;
using System.Collections.Generic;
using SecurityEssentials.ViewModel;
using System.Web.Routing;

namespace SecurityEssentials.Unit.Tests.Controllers
{
	[TestClass]
    public class UserControllerTest : BaseControllerTest
    {

        private UserController _sut;

        [TestInitialize]
        public void Setup()
        {
            base.BaseSetup();
            _sut = new UserController(_context, _userIdentity);
            _sut.Url = new UrlHelper(new RequestContext(_httpContext, new RouteData()), new RouteCollection());
            _sut.ControllerContext = new ControllerContext(_httpContext, new RouteData(), _sut);
        }

        [TestCleanup]
        public void Teardown()
        {
            base.VerifyAllExpectations();
        }

        [TestMethod]
        public void Given_UserExists_When_DisableGet_Then_PartialViewReturned()
        {

            // Arrange

            // Act
            var result = _sut.Disable(_testUserId);

            // Assert
            AssertPartialViewResultReturned(result, "_Disable");

        }

        [TestMethod]
        public void Given_UserExistsAndCurrentUser_When_DisablePost_Then_UserDisabledAndSuccessJsonReturned()
        {

            // Arrange
            var collection = new NameValueCollection();

            // Act
            var result = _sut.Disable(_testUserId, new FormCollection(collection));

            // Assert
            var dataReturned = AssertJsonResultReturned(result);
            Assert.AreEqual(true, dataReturned["success"]);
            Assert.AreEqual("", dataReturned["message"]);
            var user = _context.User.Where(u => u.Id == _testUserId).First();
            Assert.IsFalse(user.Enabled);
            _context.AssertWasCalled(a => a.SaveChanges());

        }

        [TestMethod]
        public void Given_UserExists_When_EditGet_Then_ViewReturned()
        {

            // Arrange
            _userIdentity.Expect(a => a.GetUserId(Arg<Controller>.Is.Anything)).Return(_testUserId);

            // Act
            var result = _sut.Edit(_testUserId);

            // Assert
            var viewModel = AssertViewResultReturnsType<UserViewModel>(result);
            Assert.AreEqual(_testUserId, viewModel.User.Id);
            Assert.IsTrue(viewModel.IsOwnProfile);

        }

        [TestMethod]
        public void Given_SubmissionCorrect_When_EditPost_Then_ViewReturned()
        {

            // Arrange
            var collection = new NameValueCollection();
            collection.Add("FirstName", "new");
            collection.Add("LastName", "name");

            _userIdentity.Expect(a => a.GetUserId(Arg<Controller>.Is.Anything)).Return(_testUserId);

            // Act
            var result = _sut.Edit(_testUserId, new FormCollection(collection));

            // Assert
			var viewResult = AssertViewResultReturned(result, "Edit");
            _context.AssertWasCalled(a => a.SaveChanges());
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
            _userIdentity.Expect(a => a.GetUserId(Arg<Controller>.Is.Anything)).Return(_testUserId);

            // Act
            var result = _sut.Log(_testUserId);

            // Assert
            var viewModel = AssertViewResultReturnsType<List<UserLog>>(result);
            Assert.AreEqual(2, viewModel.Count);
        }

        [TestMethod]
        public void Given_DataExists_When_Read_Then_JsonReturned()
        {

            // Arrange
            var requestItems = new NameValueCollection();
            requestItems.Add("sort[0][dir]", "");
            requestItems.Add("sort[0][field]", "");     
            // TODO: Fix this as a.Form doesn't work
            _httpRequest.Stub(a => a.Form).Return(requestItems);

            // Act
            var result = _sut.Read();

            // Assert
            var dataReturned = AssertJsonResultReturned(result);
            // TODO: Validate it returns 1 item
        }

    }
}
