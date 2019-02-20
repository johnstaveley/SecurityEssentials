using NUnit.Framework;
using SecurityEssentials.Controllers;
using SecurityEssentials.Core.Attributes;
using System;
using System.Linq;
using System.Web.Http;
using System.Web.Mvc;
using HttpApiDeleteAttribute = System.Web.Http.HttpDeleteAttribute;
using HttpApiPostAttribute = System.Web.Http.HttpPostAttribute;
using HttpApiPutAttribute = System.Web.Http.HttpPutAttribute;
using HttpWebDeleteAttribute = System.Web.Mvc.HttpDeleteAttribute;
using HttpWebPostAttribute = System.Web.Mvc.HttpPostAttribute;
using HttpWebPutAttribute = System.Web.Mvc.HttpPutAttribute;

namespace SecurityEssentials.Unit.Tests.Controllers
{

    [TestFixture]
	public class ValidateAntiForgeryTokenTest
	{

		[Test]
		[TestCase(typeof(HttpWebPostAttribute))]
		[TestCase(typeof(HttpWebPutAttribute))]
		[TestCase(typeof(HttpWebDeleteAttribute))]
		[TestCase(typeof(HttpApiPostAttribute))]
		[TestCase(typeof(HttpApiPutAttribute))]
		[TestCase(typeof(HttpApiDeleteAttribute))]
        public void AllHttpStateChangingControllerActionsShouldBeDecoratedWithValidateAntiForgeryTokenAttribute(Type action)
		{
		    var allControllerTypes = typeof(AccountController).Assembly.GetTypes()
		        .Where(type => typeof(Controller).IsAssignableFrom(type) || typeof(ApiController).IsAssignableFrom(type));
		    var allControllerActions = allControllerTypes.SelectMany(type => type.GetMethods());

            var failingActions = allControllerActions
				.Where(method => !((method.Name == "CspReporting" || method.Name == "CtReporting" || method.Name == "HpkpReporting" ) && method.DeclaringType.Name == "SecurityController"))
				.Where(method => Attribute.GetCustomAttribute(method, action) != null)
			    .Where(method => Attribute.GetCustomAttribute(method, typeof(ValidateAntiForgeryTokenAttribute)) == null && Attribute.GetCustomAttribute(method, typeof(ValidateHttpAntiForgeryTokenAttribute)) == null)
				.ToList();

			var message = string.Empty;
			if (failingActions.Any())
			{
				message =
					failingActions.Count + " action(s) not decorated with the ValidateAntiForgeryToken or ValidateHttpAntiForgeryToken token" +
					(failingActions.Count == 1 ? ":\n" : "s:\n") +
					failingActions.Select(method => $"{method.Name} in {method.DeclaringType.Name}")
						.Aggregate((a, b) => $"{a},\n{b}");
			}
			Assert.IsFalse(failingActions.Any(), message);
		}
	}
}
