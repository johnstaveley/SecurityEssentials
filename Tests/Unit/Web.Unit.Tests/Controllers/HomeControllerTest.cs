using SecurityEssentials.Controllers;
using System.Web.Mvc;
using System.Web.Routing;
using NUnit.Framework;
using Rhino.Mocks;
using SecurityEssentials.Core;

namespace SecurityEssentials.Unit.Tests.Controllers
{
	[TestFixture]
    public class HomeControllerTest : BaseControllerTest
	{
		private IAppSensor _appSensor;
		private HomeController _sut;

        [SetUp]
        public void Setup()
        {
            BaseSetup();
	        _appSensor = MockRepository.GenerateMock<IAppSensor>();
			_sut = new HomeController(UserIdentity, _appSensor)
	        {
		        Url = new UrlHelper(new RequestContext(HttpContext, new RouteData()), new RouteCollection())
	        };
	        _sut.ControllerContext = new ControllerContext(HttpContext, new RouteData(), _sut);
        }

        [TearDown]
        public void Teardown()
        {
            VerifyAllExpectations();
        }

        [Test]
        public void When_About_Then_ViewReturned()
        {

            // Arrange

            // Act
            var result = _sut.About();

            // Assert
            AssertViewResultReturned(result, "About");
        }

        [Test]
        public void When_Contact_Then_ViewReturned()
        {

            // Arrange

            // Act
            var result = _sut.Contact();

            // Assert
            var viewResult = AssertViewResultReturned(result, "Contact");
            Assert.AreEqual("Your contact page.", viewResult.ViewData["Message"]);

        }

        [Test]
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
