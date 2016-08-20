using System;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecurityEssentials.Controllers;
using SecurityEssentials.Core;
using Rhino.Mocks;
using SecurityEssentials.Core.Identity;
using System.Web.Routing;
using SecurityEssentials.Model;
using System.Reflection;
using SecurityEssentials.ViewModel;

namespace SecurityEssentials.Unit.Tests.Controllers
{
	/// <summary>
	/// Examples to show that the authorize attribute and roles can be tested for in .Net
	/// </summary>
	[TestClass]
	public class ThrottlingTest : BaseControllerTest
	{

        private AccountController _sut;
		private IAppConfiguration _configuration;
		private IEncryption _encryption;
		private IFormsAuth _formsAuth;
		private IRecaptcha _recaptcha;
		private IServices _services;
		private IUserManager _userManager;

		[TestInitialize]
        public void Setup()
        {
            base.BaseSetup();
			_configuration = MockRepository.GenerateMock<IAppConfiguration>();
			_encryption = MockRepository.GenerateMock<IEncryption>();
			_formsAuth = MockRepository.GenerateMock<IFormsAuth>();
			_recaptcha = MockRepository.GenerateMock<IRecaptcha>();
			_services = MockRepository.GenerateMock<IServices>();
			_userManager = MockRepository.GenerateMock<IUserManager>();
			_sut = new AccountController(_configuration, _encryption, _formsAuth, _context, 
				_userManager, _recaptcha, _services, _userIdentity);
            _sut.ControllerContext = new ControllerContext(_httpContext, new RouteData(), _sut);

        }

        [TestCleanup]
        public void Teardown()
        {
            base.VerifyAllExpectations();
			_configuration.VerifyAllExpectations();
			_encryption.VerifyAllExpectations();
			_formsAuth.VerifyAllExpectations();
			_recaptcha.VerifyAllExpectations();
			_services.VerifyAllExpectations();
			_userManager.VerifyAllExpectations();
		}
			
		[TestMethod]
		public void LogOnPost_THEN_IsDecoratedAntiThrottlingAttribute()
		{
			// Act
			var type = _sut.GetType();
			var methodInfo = type.GetMethod("LogOn", new Type[] { typeof(LogOn), typeof(string) } );

			//Assert
			AssertMethodIsDecoratedWithAntiThrottlingAttribute(methodInfo);
		}

		[TestMethod]
		public void ChangeEmailAddressPost_THEN_IsDecoratedAntiThrottlingAttribute()
		{
			// Act
			var type = _sut.GetType();
			var methodInfo = type.GetMethod("ChangeEmailAddress", new Type[] { typeof(ChangeEmailAddressViewModel) });			

			//Assert
			AssertMethodIsDecoratedWithAntiThrottlingAttribute(methodInfo);
		}

		[TestMethod]
		public void ChangePasswordPost_THEN_IsDecoratedAntiThrottlingAttribute()
		{
			// Act
			var type = _sut.GetType();
			var methodInfo = type.GetMethod("ChangePassword", new Type[] { typeof(ChangePasswordViewModel) });

			//Assert
			AssertMethodIsDecoratedWithAntiThrottlingAttribute(methodInfo);
		}

		[TestMethod]
		public void EmailVerifyGet_THEN_IsDecoratedAntiThrottlingAttribute()
		{
			// Act
			var type = _sut.GetType();
			var methodInfo = type.GetMethod("EmailVerify");

			//Assert
			AssertMethodIsDecoratedWithAntiThrottlingAttribute(methodInfo);
		}

		[TestMethod]
		public void RecoverPost_THEN_IsDecoratedAntiThrottlingAttribute()
		{
			// Act
			var type = _sut.GetType();
			var methodInfo = type.GetMethod("Recover", new Type[] { typeof(RecoverViewModel) });

			//Assert
			AssertMethodIsDecoratedWithAntiThrottlingAttribute(methodInfo);
		}

		[TestMethod]
		public void RecoverPasswordPost_THEN_IsDecoratedAntiThrottlingAttribute()
		{
			// Act
			var type = _sut.GetType();
			var methodInfo = type.GetMethod("RecoverPassword", new Type[] { typeof(RecoverPasswordViewModel) });

			//Assert
			AssertMethodIsDecoratedWithAntiThrottlingAttribute(methodInfo);
		}

		[TestMethod]
		public void ChangeSecurityInformationPost_THEN_IsDecoratedAntiThrottlingAttribute()
		{
			// Act
			var type = _sut.GetType();
			var methodInfo = type.GetMethod("ChangeSecurityInformation", new Type[] { typeof(ChangeSecurityInformationViewModel) });

			//Assert
			AssertMethodIsDecoratedWithAntiThrottlingAttribute(methodInfo);
		}

		[TestMethod]
		public void RegisterPost_THEN_IsDecoratedAntiThrottlingAttribute()
		{
			// Act
			var type = _sut.GetType();
			var methodInfo = type.GetMethod("Register", new Type[] { typeof(FormCollection) });

			//Assert
			AssertMethodIsDecoratedWithAntiThrottlingAttribute(methodInfo);
		}		

		private void AssertMethodIsDecoratedWithAntiThrottlingAttribute(MethodInfo methodInfo)
		{
			var attributes = methodInfo.GetCustomAttributes(typeof(AllowXRequestsEveryXSecondsAttribute), true);
			Assert.IsTrue(attributes.Any(), "No Throttling Attribute found");
			var attribute = ((AllowXRequestsEveryXSecondsAttribute)attributes.First());
			Assert.IsTrue(attribute.Seconds > 40 && attribute.Seconds < 120);
			Assert.IsTrue(attribute.Requests < 6);

		}

	}
}
