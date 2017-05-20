using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecurityEssentials.Controllers;
using SecurityEssentials.Core.Attributes;

namespace SecurityEssentials.Unit.Tests.Controllers
{
    /// <summary>
    ///     Examples to show that the authorize attribute and roles can be tested for in .Net
    /// </summary>
    [TestClass]
    public class AuthorizeTest : BaseControllerTest
    {
        private UserController _sut;

        [TestInitialize]
        public void Setup()
        {
            BaseSetup();
            _sut = new UserController(AppSensor, Context, UserIdentity);
            _sut.ControllerContext = new ControllerContext(HttpContext, new RouteData(), _sut);
        }

        [TestCleanup]
        public void Teardown()
        {
            VerifyAllExpectations();
        }

        [TestMethod]
        public void Controller_Then_IsDecoratedWithAuthorize()
        {
            var type = _sut.GetType();
            var attributes = type.GetCustomAttributes(typeof(SEAuthorizeAttribute), true);
            Assert.IsTrue(attributes.Any(), "No Authorize Attribute found");
        }

        [TestMethod]
        public void DisableGet_Then_IsDecoratedWithAuthorizeAndAdminRole()
        {
            var type = _sut.GetType();
            var methodInfo = type.GetMethod("Disable", new[] {typeof(int)});
            var attributes = methodInfo.GetCustomAttributes(typeof(SEAuthorizeAttribute), true);
            Assert.IsTrue(attributes.Any(), "No Authorize Attribute found");
            Assert.IsTrue(((SEAuthorizeAttribute) attributes.First()).Roles.Contains("Admin"),
                "No Admin role found on attribute");
        }

        [TestMethod]
        public void DisablePost_Then_IsDecoratedWithAuthorizeAndAdminRole()
        {
            var type = _sut.GetType();
            var methodInfo = type.GetMethod("Disable", new[] {typeof(int), typeof(FormCollection)});
            var attributes = methodInfo.GetCustomAttributes(typeof(SEAuthorizeAttribute), true);
            Assert.IsTrue(attributes.Any(), "No Authorize Attribute found");
            Assert.IsTrue(((SEAuthorizeAttribute) attributes.First()).Roles.Contains("Admin"),
                "No Admin role found on attribute");
        }
    }
}