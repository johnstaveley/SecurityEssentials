using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SecurityEssentials.Controllers;
using SecurityEssentials.Core;
using SecurityEssentials.Core.Constants;
using SecurityEssentials.Core.Identity;
using Serilog;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SecurityEssentials.Unit.Tests.Core
{

	[TestClass]
	public class AppSensorFixture
	{

		private AppSensor _sut;
		private string _controllerName;
		private string _actionName;
		private string _httpMethod;
		private ILogger _logger;
		private IUserIdentity _userIdentity;
		private HttpContextBase _httpContext;
		private HttpRequestBase _httpRequest;
		private HttpSessionStateBase _httpSession;
		private HomeController _controller;
		List<string> _expectedFormKeys;

		[TestInitialize]
		public void Setup()
		{
			_controllerName = "Account";
			_actionName = "Register";
			_httpMethod = "POST";
			_expectedFormKeys = new List<string>() { "TestField1", "TestField2" };
			_logger = MockRepository.GenerateMock<ILogger>();
			_userIdentity = MockRepository.GenerateMock<IUserIdentity>();
			_userIdentity.Stub(a => a.GetRequester(Arg<Controller>.Is.Anything, Arg<AppSensorDetectionPointKind>.Is.Anything))
				.Return(null) // will be ignored but still the API requires it
				.WhenCalled(_ => {
					var appSensorDetectionPointKind = (AppSensorDetectionPointKind?)_.Arguments[1];
					var requester = new Requester()
					{
						AppSensorDetectionPoint = appSensorDetectionPointKind
					};
					_.ReturnValue = requester;
				});
			_sut = new AppSensor(_userIdentity, _logger);
			_httpSession = MockRepository.GenerateMock<HttpSessionStateBase>();
			_httpContext = MockRepository.GenerateMock<HttpContextBase>();
			_httpRequest = MockRepository.GenerateMock<HttpRequestBase>();
			_httpContext.Stub(c => c.Request).Return(_httpRequest);
			_httpContext.Stub(c => c.Session).Return(_httpSession);
			_controller = new HomeController();
			_controller.ControllerContext = new ControllerContext(_httpContext, new RouteData(), _controller);

		}

		[TestCleanup]
		public void Teardown()
		{
			_logger.VerifyAllExpectations();
			_httpContext.VerifyAllExpectations();
			_httpRequest.VerifyAllExpectations();
			_httpSession.VerifyAllExpectations();
			_userIdentity.VerifyAllExpectations();
		}

		[TestMethod]
		public void GIVEN_NoModelStateErrors_WHEN_InspectModelStateErrors_THEN_NothingLogged()
		{
			// Arrange
			StubControllerCall();

			// Act
			_sut.InspectModelStateErrors(_controller);

			// Assert
			_logger.AssertWasNotCalled(a => a.Information(Arg<string>.Is.Anything, Arg<object[]>.Is.Anything));


		}

		[TestMethod]
		public void GIVEN_RequiredFieldMissed_WHEN_InspectModelStateErrors_THEN_DataMissingFromRequestDetectionPointLogged()
		{
			// Arrange
			StubControllerCall();
			_controller.ModelState.AddModelError("UserName", "Field is required");

			// Act
			_sut.InspectModelStateErrors(_controller);

			// Assert
			AssertDetectionPointFound(AppSensorDetectionPointKind.RE6);

		}

		[TestMethod]
		public void GIVEN_FieldInvalidAcccordingToRegex_WHEN_InspectModelStateErrors_THEN_ViolationOfImplementedWhiteListsDetectionPointLogged()
		{
			// Arrange
			StubControllerCall();
			_controller.ModelState.AddModelError("EmailAddress", "Email Address does not appear to be valid");

			// Act
			_sut.InspectModelStateErrors(_controller);

			// Assert
			AssertDetectionPointFound(AppSensorDetectionPointKind.IE2);

		}

		[TestMethod]
		public void GIVEN_FieldTooLong_WHEN_InspectModelStateErrors_THEN_UnexpectedQuantityOfCharactersInParameterDetectionPointLogged()
		{
			// Arrange
			StubControllerCall();
			_controller.ModelState.AddModelError("UserName", " Y with a maximum length of 15");

			// Act
			_sut.InspectModelStateErrors(_controller);

			// Assert
			AssertDetectionPointFound(AppSensorDetectionPointKind.RE7);

		}

		[TestMethod]
		public void GIVEN_FieldTooShort_WHEN_InspectModelStateErrors_THEN_UnexpectedQuantityOfCharactersInParameterDetectionPointLogged()
		{
			// Arrange
			StubControllerCall();
			_controller.ModelState.AddModelError("UserName", "X with a minimum length of 15");

			// Act
			_sut.InspectModelStateErrors(_controller);

			// Assert
			AssertDetectionPointFound(AppSensorDetectionPointKind.RE7);

		}

		[TestMethod]
		public void GIVEN_StringLengthConditionViolatedOnUserName_WHEN_InspectModelStateErrors_THEN_UnexpectedQuantityOfCharactersInUserNameDetectionPointLogged()
		{
			// Arrange
			StubControllerCall();
			_controller.ModelState.AddModelError("EmailAddress", "The Email Address must be at least 7 and less than 255 characters long");

			// Act
			_sut.InspectModelStateErrors(_controller);

			// Assert
			AssertDetectionPointFound(AppSensorDetectionPointKind.AE4);

		}

		[TestMethod]
		public void GIVEN_StringLengthConditionViolatedOnPassword_WHEN_InspectModelStateErrors_THEN_UnexpectedQuantityOfCharactersInPasswordDetectionPointLogged()
		{
			// Arrange
			StubControllerCall();
			_controller.ModelState.AddModelError("Password", "The Password must be at least 8 and less than 100 characters long");

			// Act
			_sut.InspectModelStateErrors(_controller);

			// Assert
			AssertDetectionPointFound(AppSensorDetectionPointKind.AE5);

		}

		[TestMethod]
		public void GIVEN_StringLengthConditionViolatedOnAnyNormalField_WHEN_InspectModelStateErrors_THEN_UnexpectedQuantityOfCharactersInParameterDetectionPointLogged()
		{
			// Arrange
			StubControllerCall();
			_controller.ModelState.AddModelError("AnyField", "The Any Field must be at least 7 and less than 255 characters long");

			// Act
			_sut.InspectModelStateErrors(_controller);

			// Assert
			AssertDetectionPointFound(AppSensorDetectionPointKind.RE7);

		}

		[TestMethod]
		public void GIVEN_FormDataValid_WHEN_ValidateFormData_THEN_NothingLogged()
		{
			// Arrange
			StubControllerCall();
			_expectedFormKeys = new List<string>() { "TestField1", "TestField2", "__RequestVerificationToken" };

			// Act
			_sut.ValidateFormData(_controller, _expectedFormKeys);

			// Assert
			AssertNoDetectionPointFound();

		}

		[TestMethod]
		public void GIVEN_AdditionalField_WHEN_ValidateFormData_THEN_DetectionPointLogged()
		{
			// Arrange
			StubControllerCall();
			_expectedFormKeys = new List<string>() { "TestField1", "__RequestVerificationToken" };

			// Act
			_sut.ValidateFormData(_controller, _expectedFormKeys);

			// Assert
			AssertFormValidationDetectionPointFound(AppSensorDetectionPointKind.RE5, "additional");

		}

		[TestMethod]
		public void GIVEN_AdditionalFieldOnLogOn_WHEN_ValidateFormData_THEN_DetectionPointLogged()
		{
			// Arrange
			_httpMethod = "POST";
			_controllerName = "Account";
			_actionName = "LogOn";
			StubControllerCall();
			_expectedFormKeys = new List<string>() { "TestField1", "__RequestVerificationToken" };

			// Act
			_sut.ValidateFormData(_controller, _expectedFormKeys);

			// Assert
			AssertFormValidationDetectionPointFound(AppSensorDetectionPointKind.AE10, "additional");

		}

		[TestMethod]
		public void GIVEN_MissingField_WHEN_ValidateFormData_THEN_DetectionPointLogged()
		{
			// Arrange
			StubControllerCall();
			_expectedFormKeys = new List<string>() { "TestField1", "TestField2", "TestField3", "__RequestVerificationToken" };

			// Act
			_sut.ValidateFormData(_controller, _expectedFormKeys);

			// Assert
			AssertFormValidationDetectionPointFound(AppSensorDetectionPointKind.RE6, "missing");

		}

		[TestMethod]
		public void GIVEN_MissingFieldOnLogOn_WHEN_ValidateFormData_THEN_DetectionPointLogged()
		{
			// Arrange
			_httpMethod = "POST";
			_controllerName = "Account";
			_actionName = "LogOn";
			StubControllerCall();
			_expectedFormKeys = new List<string>() { "TestField1", "TestField2", "TestField3", "__RequestVerificationToken" };

			// Act
			_sut.ValidateFormData(_controller, _expectedFormKeys);

			// Assert
			AssertFormValidationDetectionPointFound(AppSensorDetectionPointKind.AE11, "missing");

		}

		[TestMethod, Ignore]
		public void GIVEN_InputParameterWithSQLComment_WHEN_ValidateFormData_THEN_DetectionPointLogged()
		{
			// Arrange
			StubControllerCall("joe bloggs -- ");

			// Act
			_sut.ValidateFormData(_controller, _expectedFormKeys);

			// Assert
			AssertFormValidationDetectionPointFound(AppSensorDetectionPointKind.CIE1, "SQL injection");

		}

		[TestMethod, Ignore]
		public void GIVEN_InputParameterWithSQLCommentBlock_WHEN_ValidateFormData_THEN_DetectionPointLogged()
		{
			// Arrange
			StubControllerCall("joe bloggs /* ");

			// Act
			_sut.ValidateFormData(_controller, _expectedFormKeys);

			// Assert
			AssertFormValidationDetectionPointFound(AppSensorDetectionPointKind.CIE1, "SQL injection");

		}

		[TestMethod, Ignore]
		public void GIVEN_InputParameterWithMySQLComment_WHEN_ValidateFormData_THEN_DetectionPointLogged()
		{
			// Arrange
			StubControllerCall("joe bloggs # ");

			// Act
			_sut.ValidateFormData(_controller, _expectedFormKeys);

			// Assert
			AssertFormValidationDetectionPointFound(AppSensorDetectionPointKind.CIE1, "SQL injection");

		}

		[TestMethod, Ignore]
		public void GIVEN_InputParameterWithValidApostrophe_WHEN_ValidateFormData_THEN_NoDetectionPointLogged()
		{
			// Arrange
			StubControllerCall("O'Hara");

			// Act
			_sut.ValidateFormData(_controller, _expectedFormKeys);

			// Assert
			AssertNoDetectionPointFound();

		}

		[TestMethod, Ignore]
		public void GIVEN_SQLInjectionSequence_WHEN_ValidateFormData_THEN_DetectionPointLogged()
		{
			// Arrange
			StubControllerCall("anything' OR 'x'='x");

			// Act
			_sut.ValidateFormData(_controller, _expectedFormKeys);

			// Assert
			AssertFormValidationDetectionPointFound(AppSensorDetectionPointKind.CIE1, "SQL injection");

		}

		[TestMethod, Ignore]
		public void GIVEN_SQLInjectionSequence2_WHEN_ValidateFormData_THEN_DetectionPointLogged()
		{
			// Arrange
			StubControllerCall("x' OR 1=1; -- ");

			// Act
			_sut.ValidateFormData(_controller, _expectedFormKeys);

			// Assert
			AssertFormValidationDetectionPointFound(AppSensorDetectionPointKind.CIE1, "SQL injection");

		}

		[TestMethod, Ignore]
		public void GIVEN_SQLInjectionSequence3_WHEN_ValidateFormData_THEN_DetectionPointLogged()
		{
			// Arrange
			StubControllerCall("value' or sysdate is not null or sysdate<>'");

			// Act
			_sut.ValidateFormData(_controller, _expectedFormKeys);

			// Assert
			AssertFormValidationDetectionPointFound(AppSensorDetectionPointKind.CIE1, "SQL injection");

		}

		[TestMethod, Ignore]
		public void GIVEN_SQLInjectionSequence4_WHEN_ValidateFormData_THEN_DetectionPointLogged()
		{
			// Arrange
			StubControllerCall("value%27 or sysdate is not null or sysdate<>'");

			// Act
			_sut.ValidateFormData(_controller, _expectedFormKeys);

			// Assert
			AssertFormValidationDetectionPointFound(AppSensorDetectionPointKind.CIE1, "SQL injection");

		}


		private void AssertFormValidationDetectionPointFound(AppSensorDetectionPointKind appSensorDetectionPointKind, string type)
		{
			_logger.AssertWasCalled(a => a.Information(
				Arg<string>.Matches(e => e.Contains("{@controllerName} {@methodName} {@httpMethod}") && e.Contains(type)),
				Arg<object[]>.Matches(b => b.OfType<Requester>().Any(
					c => c.AppSensorDetectionPoint == appSensorDetectionPointKind) &&
					b.OfType<string>().Any(d => d == _controllerName) &&
					b.OfType<string>().Any(d => d == _actionName) &&
					b.OfType<string>().Any(d => d == _httpMethod)
					)));
		}

		private void AssertDetectionPointFound(AppSensorDetectionPointKind appSensorDetectionPointKind)
		{
			_logger.AssertWasCalled(a => a.Information(
				Arg<string>.Is.Equal("Failed {@controllerName} {@methodName} {@httpMethod} validation bypass {errorMessage} attempted by user {@requester}"),
				Arg<object[]>.Matches(b => b.OfType<Requester>().Any(
					c => c.AppSensorDetectionPoint == appSensorDetectionPointKind) &&
					b.OfType<string>().Any(d => d == _controllerName) &&
					b.OfType<string>().Any(d => d == _actionName) &&
					b.OfType<string>().Any(d => d == _httpMethod)
					)));
		}

		private void AssertNoDetectionPointFound()
		{
			_logger.AssertWasNotCalled(a => a.Information(Arg<string>.Is.Anything, Arg<object[]>.Is.Anything));
		}

		private void StubControllerCall(string testField1Value = "1")
		{
			_httpRequest.Stub(a => a.HttpMethod).Return(_httpMethod);
			_httpRequest.Stub(a => a.CurrentExecutionFilePath).Return(string.Format("~/{0}/{1}", _controllerName, _actionName));
			_controller.Request.Stub(a => a.Form).Return(new System.Collections.Specialized.NameValueCollection() { { "TestField1", testField1Value }, { "TestField2", "2" }, { "__RequestVerificationToken", "abc" } });

		}

	}
}
