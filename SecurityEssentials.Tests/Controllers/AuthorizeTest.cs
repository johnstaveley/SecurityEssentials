using System;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecurityEssentials.Controllers;
using SecurityEssentials.Core;
using Rhino.Mocks;
using SecurityEssentials.Core.Identity;
using System.Web;
using System.Web.Routing;

namespace SecurityEssentials.Unit.Tests.Controllers
{
	/// <summary>
	/// Examples to show that the authorize attribute and roles can be tested for in .Net
	/// </summary>
	[TestClass]
	public class AuthorizeTest : BaseControllerTest
	{

        private UserController _sut;

        [TestInitialize]
        public void Setup()
        {
            _context = MockRepository.GenerateStub<ISEContext>();
            _userIdentity = MockRepository.GenerateStub<IUserIdentity>();
            _sut = new UserController(_context, _userIdentity);
            _httpContext = MockRepository.GenerateMock<HttpContextBase>();
            _httpRequest = MockRepository.GenerateMock<HttpRequestBase>();
            _sut.Url = new UrlHelper(new RequestContext(_httpContext, new RouteData()), new RouteCollection());
            _sut.ControllerContext = new ControllerContext(_httpContext, new RouteData(), _sut);

        }

        [TestCleanup]
        public void Teardown()
        {
            base.VerifyAllExpectations();
        }

		[TestMethod]
		public void Controller_THEN_IsDecoratedWithAuthorize()
		{
			var type = _sut.GetType();
			var attributes = type.GetCustomAttributes(typeof(AuthorizeAttribute), true);
			Assert.IsTrue(attributes.Any(), "No Authorize Attribute found");
		}

		[TestMethod]
		public void DisableGet_THEN_IsDecoratedWithAuthorizeAndAdminRole()
		{
			var type = _sut.GetType();
			var methodInfo = type.GetMethod("Disable", new Type[] { typeof(int) });
			var attributes = methodInfo.GetCustomAttributes(typeof(AuthorizeAttribute), true);
			Assert.IsTrue(attributes.Any(), "No Authorize Attribute found");
			Assert.IsTrue(((AuthorizeAttribute) attributes.First()).Roles.Contains("Admin"), "No Admin attribute is found");
		}

		[TestMethod]
		public void DisablePost_THEN_IsDecoratedWithAuthorizeAndAdminRole()
		{
			var type = _sut.GetType();
			var methodInfo = type.GetMethod("Disable", new Type[] { typeof(int), typeof(FormCollection) });
			var attributes = methodInfo.GetCustomAttributes(typeof(AuthorizeAttribute), true);
			Assert.IsTrue(attributes.Any(), "No Authorize Attribute found");
			Assert.IsTrue(((AuthorizeAttribute)attributes.First()).Roles.Contains("Admin"), "No Admin attribute is found");
		}


	}
}
