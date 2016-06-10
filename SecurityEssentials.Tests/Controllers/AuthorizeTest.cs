using System;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecurityEssentials.Controllers;
using SecurityEssentials.Core;
using Rhino.Mocks;

namespace SecurityEssentials.Unit.Tests.Controllers
{
	/// <summary>
	/// Examples to show that the authorize attribute and roles can be tested for in .Net
	/// </summary>
	[TestClass]
	public class AuthorizeTest
	{

        private UserController _sut;
        private ISEContext _context;

        [TestInitialize]
        public void Setup()
        {
            _context = MockRepository.GenerateStub<ISEContext>();
            _sut = new UserController(_context);
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
