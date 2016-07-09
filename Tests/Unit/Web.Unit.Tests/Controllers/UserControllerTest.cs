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
        public void GIVEN_UserExists_WHEN_DisableGet_THEN_PartialViewReturned()
        {

            // Arrange

            // Act
            var result = _sut.Disable(_testUserId);

            // Assert
            AssertPartialViewResultReturned(result, "_Disable");

        }

        [TestMethod]
        public void GIVEN_UserExistsAndCurrentUser_WHEN_DisablePost_THEN_UserDisabledAndSuccessJsonReturned()
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
        public void GIVEN_UserExists_WHEN_ChangeEmailAddressGet_THEN_ViewReturned()
        {

            // Arrange
            _userIdentity.Expect(a => a.GetUserId(Arg<Controller>.Is.Anything)).Return(_testUserId);


            // Act
            var result = _sut.ChangeEmailAddress(_testUserId);

            // Assert
            var viewModel = AssertViewResultReturnsType <UserViewModel>(result);
            Assert.AreEqual(_testUserId, viewModel.User.Id);
            Assert.IsTrue(viewModel.IsOwnProfile);

        }

        [TestMethod]
        public void GIVEN_UserExists_WHEN_EditGet_THEN_ViewReturned()
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
        public void GIVEN_SubmissionCorrect_WHEN_EditPost_THEN_ViewReturned()
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
        public void WHEN_Index_THEN_ViewReturned()
        {

            // Arrange

            // Act
            var result = _sut.Index();

            // Assert
            AssertViewResultReturnsType<UsersViewModel>(result);

        }

        [TestMethod]
        public void GIVEN_UserExists_WHEN_LogGet_THEN_ViewReturned()
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
        public void GIVEN_DataExists_WHEN_Read_THEN_JsonReturned()
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
