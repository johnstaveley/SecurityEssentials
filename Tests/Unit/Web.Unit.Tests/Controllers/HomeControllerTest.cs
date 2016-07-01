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
    public class HomeControllerTest : BaseControllerTest
    {

        private HomeController _sut;

        [TestInitialize]
        public void Setup()
        {
            base.BaseSetup();
            _sut = new HomeController();
            _sut.Url = new UrlHelper(new RequestContext(_httpContext, new RouteData()), new RouteCollection());
            _sut.ControllerContext = new ControllerContext(_httpContext, new RouteData(), _sut);
        }

        [TestCleanup]
        public void Teardown()
        {
            base.VerifyAllExpectations();
        }

        [TestMethod]
        public void WHEN_About_THEN_ViewReturned()
        {

            // Arrange

            // Act
            var result = _sut.About();

            // Assert
            AssertViewResultReturned(result, "About");
        }

        [TestMethod]
        public void WHEN_Contact_THEN_ViewReturned()
        {

            // Arrange

            // Act
            var result = _sut.Contact();

            // Assert
            var viewResult = AssertViewResultReturned(result, "Contact");
            Assert.AreEqual("Your contact page.", viewResult.ViewData["Message"]);

        }

        [TestMethod]
        public void WHEN_Index_THEN_ViewReturned()
        {

            // Arrange

            // Act
            var result = _sut.Index();

            // Assert
            var viewResult = AssertViewResultReturned(result, "Index");
            Assert.AreEqual("Security Essentials", viewResult.ViewData["Message"]);

        }

    }
}
