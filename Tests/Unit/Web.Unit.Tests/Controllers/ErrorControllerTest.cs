using NUnit.Framework;
using SecurityEssentials.Controllers;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Rhino.Mocks;
using SecurityEssentials.Core;
using SecurityEssentials.Core.Attributes;

namespace SecurityEssentials.Unit.Tests.Controllers
{
	[TestFixture]
	public class ErrorControllerTest : BaseControllerTest
	{

		private ErrorController _sut;
		private IAppSensor _appSensor;

		[SetUp]
		public void Setup()
		{
			BaseSetup();
			_appSensor = MockRepository.GenerateMock<IAppSensor>();
			_sut = new ErrorController(UserIdentity, _appSensor)
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
			Assert.IsFalse(attributes.Any(), "Authorize Attribute not found");

		}

		[Test]
		[TestCase(true, "_Forbidden")]
		[TestCase(false, "Forbidden")]
		public void When_Forbidden_Then_ViewReturned(bool isAjaxRequest, string expectedView)
		{

			// Arrange
			StubReceivedAjaxRequest(isAjaxRequest);

			// Act
			var result = _sut.Forbidden();

			// Assert
			AssertViewOrPartialResultReturned(result, isAjaxRequest, expectedView);
		}

		[Test]
		[TestCase(true, "_Index")]
		[TestCase(false, "Index")]
		public void When_Index_Then_ViewReturned(bool isAjaxRequest, string expectedView)
		{

			// Arrange
			StubReceivedAjaxRequest(isAjaxRequest);

			// Act
			var result = _sut.Index();

			// Assert
			AssertViewOrPartialResultReturned(result, isAjaxRequest, expectedView);

		}

		[Test]
		[TestCase(true, "_NotFound")]
		[TestCase(false, "NotFound")]
		public void When_NotFound_Then_ViewReturned(bool isAjaxRequest, string expectedView)
		{

			// Arrange
			StubReceivedAjaxRequest(isAjaxRequest);
		    HttpRequest.Stub(a => a.CurrentExecutionFilePath).Return("");

            // Act
            var result = _sut.NotFound();

			// Assert
			AssertViewOrPartialResultReturned(result, isAjaxRequest, expectedView);
		}

	}
}
