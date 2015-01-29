using System;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecurityEssentials.Controllers;

namespace SecurityEssentials.Tests.Controllers
{
	/// <summary>
	/// Examples to show that the auithorize attribute and roles can be tested for in .Net
	/// </summary>
	[TestClass]
	public class UserTest
	{

		[TestMethod]
		public void Controller_THEN_IsDecoratedWithAuthorize()
		{
			var controller = new UserController();
			var type = controller.GetType();
			var attributes = type.GetCustomAttributes(typeof(AuthorizeAttribute), true);
			Assert.IsTrue(attributes.Any(), "No Authorize Attribute found");
		}

		[TestMethod]
		public void DisableGet_THEN_IsDecoratedWithAuthorizeAndAdminRole()
		{
			var controller = new UserController();
			var type = controller.GetType();
			var methodInfo = type.GetMethod("Disable", new Type[] { typeof(int) });
			var attributes = methodInfo.GetCustomAttributes(typeof(AuthorizeAttribute), true);
			Assert.IsTrue(attributes.Any(), "No Authorize Attribute found");
			Assert.IsTrue(((AuthorizeAttribute) attributes.First()).Roles.Contains("Admin"), "No Admin attribute is found");
		}

		[TestMethod]
		public void DisablePost_THEN_IsDecoratedWithAuthorizeAndAdminRole()
		{
			var controller = new UserController();
			var type = controller.GetType();
			var methodInfo = type.GetMethod("Disable", new Type[] { typeof(int), typeof(FormCollection) });
			var attributes = methodInfo.GetCustomAttributes(typeof(AuthorizeAttribute), true);
			Assert.IsTrue(attributes.Any(), "No Authorize Attribute found");
			Assert.IsTrue(((AuthorizeAttribute)attributes.First()).Roles.Contains("Admin"), "No Admin attribute is found");
		}


	}
}
