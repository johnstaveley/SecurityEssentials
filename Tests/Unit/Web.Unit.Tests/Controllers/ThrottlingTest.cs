using NUnit.Framework;
using Rhino.Mocks;
using SecurityEssentials.Controllers;
using SecurityEssentials.Core;
using SecurityEssentials.Core.Attributes;
using SecurityEssentials.Core.Identity;
using SecurityEssentials.ViewModel;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;

namespace SecurityEssentials.Unit.Tests.Controllers
{
	/// <summary>
	/// Examples to show that the authorize attribute and roles can be tested for in .Net
	/// </summary>
	[TestFixture]
	public class ThrottlingTest : BaseControllerTest
	{

		private AccountController _sut;
		private IAppConfiguration _configuration;
		private IAppSensor _appSensor;
		private IEncryption _encryption;
		private IFormsAuth _formsAuth;
		private IHttpCache _httpCache;
		private IRecaptcha _recaptcha;
		private IServices _services;
		private IUserManager _userManager;

		[SetUp]
		public void Setup()
		{
			BaseSetup();
			_appSensor = MockRepository.GenerateMock<IAppSensor>();
			_configuration = MockRepository.GenerateMock<IAppConfiguration>();
			_encryption = MockRepository.GenerateMock<IEncryption>();
			_formsAuth = MockRepository.GenerateMock<IFormsAuth>();
			_httpCache = MockRepository.GenerateMock<IHttpCache>();
			_recaptcha = MockRepository.GenerateMock<IRecaptcha>();
			_services = MockRepository.GenerateMock<IServices>();
			_userManager = MockRepository.GenerateMock<IUserManager>();
			_sut = new AccountController(_appSensor, _configuration, _encryption, _formsAuth, Context,
				_httpCache, _userManager, _recaptcha, _services, UserIdentity);
			_sut.ControllerContext = new ControllerContext(HttpContext, new RouteData(), _sut);

		}

		[TearDown]
		public void Teardown()
		{
			VerifyAllExpectations();
			_appSensor.VerifyAllExpectations();
			_configuration.VerifyAllExpectations();
			_encryption.VerifyAllExpectations();
			_formsAuth.VerifyAllExpectations();
			_httpCache.VerifyAllExpectations();
			_recaptcha.VerifyAllExpectations();
			_services.VerifyAllExpectations();
			_userManager.VerifyAllExpectations();
		}

		[Test]
		public void When_LogOnPost_Then_IsDecoratedAntiThrottlingAttribute()
		{
			// Act
			var type = _sut.GetType();
			var methodInfo = type.GetMethod("LogOnAsync", new[] { typeof(LogOnViewModel), typeof(string) });

			//Assert
			AssertMethodIsDecoratedWithAntiThrottlingAttribute(methodInfo);
		}

		[Test]
		public void When_ChangeEmailAddressPost_Then_IsDecoratedAntiThrottlingAttribute()
		{
			// Act
			var type = _sut.GetType();
			var methodInfo = type.GetMethod("ChangeEmailAddressAsync", new[] { typeof(ChangeEmailAddressViewModel) });

			//Assert
			AssertMethodIsDecoratedWithAntiThrottlingAttribute(methodInfo);
		}

		[Test]
		public void When_ChangePasswordPost_Then_IsDecoratedAntiThrottlingAttribute()
		{
			// Act
			var type = _sut.GetType();
			var methodInfo = type.GetMethod("ChangePasswordAsync", new[] { typeof(ChangePasswordViewModel) });

			//Assert
			AssertMethodIsDecoratedWithAntiThrottlingAttribute(methodInfo);
		}

		[Test]
		public void When_EmailVerifyGet_Then_IsDecoratedAntiThrottlingAttribute()
		{
			// Act
			var type = _sut.GetType();
			var methodInfo = type.GetMethod("EmailVerifyAsync");

			//Assert
			AssertMethodIsDecoratedWithAntiThrottlingAttribute(methodInfo);
		}

		[Test]
		public void When_RecoverPost_Then_IsDecoratedAntiThrottlingAttribute()
		{
			// Act
			var type = _sut.GetType();
			var methodInfo = type.GetMethod("RecoverAsync", new[] { typeof(RecoverViewModel) });

			//Assert
			AssertMethodIsDecoratedWithAntiThrottlingAttribute(methodInfo);
		}

		[Test]
		public void When_RecoverPasswordPost_Then_IsDecoratedAntiThrottlingAttribute()
		{
			// Act
			var type = _sut.GetType();
			var methodInfo = type.GetMethod("RecoverPasswordAsync", new[] { typeof(RecoverPasswordViewModel) });

			//Assert
			AssertMethodIsDecoratedWithAntiThrottlingAttribute(methodInfo);
		}

		[Test]
		public void When_ChangeSecurityInformationPost_Then_IsDecoratedAntiThrottlingAttribute()
		{
			// Act
			var type = _sut.GetType();
			var methodInfo = type.GetMethod("ChangeSecurityInformationAsync", new[] { typeof(ChangeSecurityInformationViewModel) });

			//Assert
			AssertMethodIsDecoratedWithAntiThrottlingAttribute(methodInfo);
		}

		[Test]
		public void When_RegisterPost_Then_IsDecoratedAntiThrottlingAttribute()
		{
			// Act
			var type = _sut.GetType();
			var methodInfo = type.GetMethod("RegisterAsync", new[] { typeof(FormCollection) });

			//Assert
			AssertMethodIsDecoratedWithAntiThrottlingAttribute(methodInfo);
		}

		private void AssertMethodIsDecoratedWithAntiThrottlingAttribute(MethodInfo methodInfo)
		{
			var attributes = methodInfo.GetCustomAttributes(typeof(AllowXRequestsEveryXSecondsAttribute), true);
			Assert.That(attributes.Any(), "No Throttling Attribute found");
			var attribute = ((AllowXRequestsEveryXSecondsAttribute)attributes.First());
			Assert.That(attribute.Seconds > 40 && attribute.Seconds < 120);
			Assert.That(attribute.Requests < 6);

		}

	}
}
