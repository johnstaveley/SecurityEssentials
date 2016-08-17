using System;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecurityEssentials.Controllers;

namespace SecurityEssentials.Unit.Tests.Controllers
{

	[TestClass]
	public class ValidateAntiForgeryTokenTest
	{

		[TestMethod]
		public void AllHttpPostControllerActionsShouldBeDecoratedWithValidateAntiForgeryTokenAttribute()
		{
			var allControllerTypes = typeof(AccountController).Assembly.GetTypes()
			   .Where(type => typeof(Controller).IsAssignableFrom(type));
			var allControllerActions = allControllerTypes.SelectMany(type => type.GetMethods());

			var failingActions = allControllerActions
				.Where(method =>
					Attribute.GetCustomAttribute(method, typeof(HttpPostAttribute)) != null)
				.Where(method =>
					Attribute.GetCustomAttribute(method, typeof(ValidateAntiForgeryTokenAttribute)) == null)
				.ToList();

			var message = string.Empty;
			if (failingActions.Any())
			{
				message =
					failingActions.Count() + " failing action" +
					(failingActions.Count() == 1 ? ":\n" : "s:\n") +
					failingActions.Select(method => method.Name + " in " + method.DeclaringType.Name)
						.Aggregate((a, b) => a + ",\n" + b);
			}

			Assert.IsFalse(failingActions.Any(), message);

		}

	}
}
