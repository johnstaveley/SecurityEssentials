using NUnit.Framework;
using SecurityEssentials.Controllers;
using System;
using System.Linq;
using System.Web.Mvc;

namespace SecurityEssentials.Unit.Tests.Controllers
{

	[TestFixture]
	public class ValidateAntiForgeryTokenTest
	{

		[Test]
		[TestCase(typeof(HttpPostAttribute))]
		[TestCase(typeof(HttpPutAttribute))]
		[TestCase(typeof(HttpDeleteAttribute))]
		public void AllHttpStateChangingControllerActionsShouldBeDecoratedWithValidateAntiForgeryTokenAttribute(Type action)
		{
			var allControllerTypes = typeof(AccountController).Assembly.GetTypes()
				.Where(type => typeof(Controller).IsAssignableFrom(type));
			var allControllerActions = allControllerTypes.SelectMany(type => type.GetMethods());

			var failingActions = allControllerActions
				.Where(method => Attribute.GetCustomAttribute(method, action) != null)
				.Where(method => Attribute.GetCustomAttribute(method, typeof(ValidateAntiForgeryTokenAttribute)) == null)
				.ToList();

			var message = string.Empty;
			if (failingActions.Any())
			{
				message =
					failingActions.Count + " action(s) not decorated with the ValidateAntiForgeryToken token" +
					(failingActions.Count == 1 ? ":\n" : "s:\n") +
					failingActions.Select(method => $"{method.Name} in {method.DeclaringType.Name}")
						.Aggregate((a, b) => $"{a},\n{b}");
			}
			Assert.IsFalse(failingActions.Any(), message);
		}
	}
}
