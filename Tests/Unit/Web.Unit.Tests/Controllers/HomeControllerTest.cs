using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecurityEssentials.Controllers;

namespace SecurityEssentials.Unit.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest : BaseControllerTest
    {
        private HomeController _sut;

        [TestInitialize]
        public void Setup()
        {
            BaseSetup();
            _sut = new HomeController
            {
                Url = new UrlHelper(new RequestContext(_httpContext, new RouteData()), new RouteCollection())
            };
            _sut.ControllerContext = new ControllerContext(_httpContext, new RouteData(), _sut);
        }

        [TestCleanup]
        public void Teardown()
        {
            VerifyAllExpectations();
        }

        [TestMethod]
        public void When_About_Then_ViewReturned()
        {
            // Arrange

            // Act
            var result = _sut.About();

            // Assert
            AssertViewResultReturned(result, "About");
        }

        [TestMethod]
        public void When_Contact_Then_ViewReturned()
        {
            // Arrange

            // Act
            var result = _sut.Contact();

            // Assert
            var viewResult = AssertViewResultReturned(result, "Contact");
            Assert.AreEqual("Your contact page.", viewResult.ViewData["Message"]);
        }

        [TestMethod]
        public void When_Index_Then_ViewReturned()
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