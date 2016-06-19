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

    }
}
