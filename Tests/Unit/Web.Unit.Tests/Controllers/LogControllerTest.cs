using NUnit.Framework;
using Rhino.Mocks;
using SecurityEssentials.Controllers;
using SecurityEssentials.Core;
using SecurityEssentials.Core.Attributes;
using SecurityEssentials.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace SecurityEssentials.Unit.Tests.Controllers
{
	[TestFixture]
	public class LogControllerTest : BaseControllerTest
	{

		private LogController _sut;
		private IAppSensor _appSensor;

		[SetUp]
		public void Setup()
		{
			BaseSetup();
			_appSensor = MockRepository.GenerateMock<IAppSensor>();
			_sut = new LogController(Context, UserIdentity, _appSensor)
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
		public void When_ControllerCreated_Then_IsDecoratedWithAuthorizeAndRoles()
		{
			var type = _sut.GetType();
			var attributes = type.GetCustomAttributes(typeof(SeAuthorizeAttribute), true);
			Assert.That(attributes.Any(), "No Hep C Authorize Attribute found");
			Assert.That(((SeAuthorizeAttribute)attributes.First()).Roles.Contains("Admin"), "No Admin role found on attribute");
		}

		[Test]
		public void When_Index_Then_ViewReturned()
		{

			// Arrange
			Context.Log.Add(new Log { Id = 1, Level = "Information", Message = "Log 1", TimeStamp = DateTime.Parse("2017-03-01") });


			// Act
			var result = _sut.Index();

			// Assert
			var model = AssertViewResultReturnsType<List<Log>>(result);
			Assert.That(model.Count, Is.EqualTo(1));
			AssertViewResultReturned(result, "Index");

		}



	}
}
