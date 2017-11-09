using NUnit.Framework;
using Rhino.Mocks;
using SecurityEssentials.Core;
using SecurityEssentials.Core.Constants;
using SecurityEssentials.Core.Identity;
using SecurityEssentials.Model;
using SecurityEssentials.Unit.Tests.TestDbSet;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace SecurityEssentials.Unit.Tests.Controllers
{
	public abstract class BaseControllerTest : IDisposable
	{

		protected ISeContext Context;
		protected HttpContextBase HttpContext;
		protected HttpRequestBase HttpRequest;
		protected HttpResponseBase HttpResponse;
		protected HttpSessionStateBase HttpSession;
		protected HttpServerUtilityBase HttpServer;
		protected IUserIdentity UserIdentity;
		protected string EncryptedSecurityAnswer = "encryptedSecurityAnswer";
		protected string EncryptedSecurityAnswerSalt = "hfgkfgjhfkerirtjasfjkGHJKH";
		protected string TestFirstName = "Bob";
		protected string TestUserName = "testuserName@test.net";
		protected int TestUserId = 5;
		protected int TestSuperUserId = 52;
		protected const int TestDeliveryNetworkId = 14;
		protected DateTime LastAccountActivity = DateTime.Parse("2016-05-10");

		protected void BaseSetup()
		{
			Context = MockRepository.GenerateStub<ISeContext>();
			Context.Log = new TestDbSet<Log>();
			Context.LookupItem = new TestDbSet<LookupItem>();
			Context.LookupType = new TestDbSet<LookupType>();
			Context.PreviousPassword = new TestDbSet<PreviousPassword>();
			Context.Role = new TestDbSet<Role>();
			Context.User = new TestDbSet<User> { GetUser() };
			Context.Stub(a => a.SaveChangesAsync()).Return(Task.FromResult(0));
			UserIdentity = MockRepository.GenerateMock<IUserIdentity>();
			HttpSession = MockRepository.GenerateMock<HttpSessionStateBase>();
			HttpContext = MockRepository.GenerateMock<HttpContextBase>();
			HttpRequest = MockRepository.GenerateMock<HttpRequestBase>();
			HttpResponse = MockRepository.GenerateMock<HttpResponseBase>();
			HttpServer = MockRepository.GenerateMock<HttpServerUtilityBase>();
			HttpContext.Stub(c => c.Request).Return(HttpRequest);
			HttpContext.Stub(c => c.Session).Return(HttpSession);
			HttpContext.Stub(c => c.Server).Return(HttpServer);
			HttpContext.Stub(c => c.Response).Return(HttpResponse);

		}

		protected void VerifyAllExpectations()
		{
			HttpContext.VerifyAllExpectations();
			HttpRequest.VerifyAllExpectations();
			HttpResponse.VerifyAllExpectations();
			HttpSession.VerifyAllExpectations();
			UserIdentity.VerifyAllExpectations();
			Context.VerifyAllExpectations();
		}

		protected User GetUser()
		{
			return new User
			{
				Id = TestUserId,
				Enabled = true,
				Approved = true,
				EmailVerified = true,
				FirstName = TestFirstName,
				UserName = TestUserName,
				EmailConfirmationToken = "test1",
				SecurityQuestionLookupItemId = 1,
				SecurityQuestionLookupItem = new LookupItem { Id = 1, Description = "test question" },
				SecurityAnswer = EncryptedSecurityAnswer,
				UserLogs = new List<UserLog>
				{
					new UserLog { Id = 2, CreatedDateUtc = DateTime.Parse("2016-06-10"), Description = "did stuff" },
					new UserLog { Id = 1, CreatedDateUtc = LastAccountActivity, Description = "did old stuff" }
				}
			};
		}

		protected void StubAdminUser(bool isEnabled)
		{
			var id = new Random().Next(50, 10000);
			var user = new User
			{
				Id = id,
				Enabled = isEnabled,
				Approved = true,
				EmailVerified = true,
				FirstName = "Admin",
				LastName = "User",
				UserName = $"admin.user{id}@test.net",
				SecurityQuestionLookupItemId = 1,
				SecurityQuestionLookupItem = new LookupItem { Id = 1, Description = "test question" },
				SecurityAnswer = EncryptedSecurityAnswer
			};
			var userRole = new UserRole { Id = id, RoleId = Consts.Roles.Admin, User = user };
			Context.UserRole.Add(userRole);
		}

		public dynamic AssertJsonResultReturned(ActionResult actionResult)
		{
			Assert.IsNotNull(actionResult, "No result was returned from controller");
			Assert.IsInstanceOf<JsonResult>(actionResult, "Not JsonResult returned from controller");
			var jsonResult = (JsonResult)actionResult;
			Assert.IsNotNull(jsonResult.Data, "Json Result should contain data");
			var serializer = new JavaScriptSerializer();
			var responseObject = serializer.Serialize(jsonResult.Data) as dynamic;
			var response = serializer.Deserialize<dynamic>(responseObject);
			return response;
		}

		public void AssertViewResultWithError(ActionResult actionResult, string errorValue)
		{
			Assert.IsNotNull(actionResult, "No result was returned from controller");
			Assert.IsInstanceOf<ViewResult>(actionResult, "Not ViewResult returned from controller");
			var viewResult = (ViewResult)actionResult;
			Assert.IsNotNull(viewResult.ViewData.ModelState);
			Assert.IsFalse(viewResult.ViewData.ModelState.ToList().Count == 0);
			var error = viewResult.ViewData.ModelState.ToList().Select(a => a.Value.Errors).Where(b => b.Count > 0).ToList();
			Assert.AreEqual(errorValue, error[0][0].ErrorMessage);

		}

		public T AssertViewResultReturnsType<T>(ActionResult actionResult)
		{
			Assert.IsNotNull(actionResult, "No result was returned from controller");
			Assert.IsInstanceOf<ViewResult>(actionResult, "Not ViewResult returned from controller");
			var viewResult = (ViewResult)actionResult;
			Assert.IsInstanceOf<T>(viewResult.ViewData.Model, "Not expected type returned as data");
			return (T)viewResult.ViewData.Model;

		}
		public T AssertHttpContentReturnsType<T>(IHttpActionResult result)
		{
			Assert.IsInstanceOf<OkNegotiatedContentResult<T>>(result);
			var conNegResult = result as OkNegotiatedContentResult<T>;
			Assert.IsNotNull(conNegResult);
			return conNegResult.Content;
		}
		public ViewResult AssertViewResultReturned(ActionResult actionResult, string viewName)
		{
			Assert.IsNotNull(actionResult, "No result was returned from controller");
			Assert.IsInstanceOf<ViewResult>(actionResult, "Not ViewResult returned from controller");
			var viewResult = (ViewResult)actionResult;
			if (!string.IsNullOrEmpty(viewName))
			{
				Assert.AreEqual(viewName, viewResult.ViewName);
			}
			return viewResult;
		}

		public void AssertNotFoundReturned(ActionResult actionResult)
		{
			Assert.IsNotNull(actionResult, "No result was returned from controller");
			Assert.IsInstanceOf<HttpNotFoundResult>(actionResult, "Not HttpNotFoundResult returned from controller");
		}

		public void AssertBadRequestReturned(ActionResult actionResult)
		{
			Assert.IsNotNull(actionResult, "No result was returned from controller");
			Assert.IsInstanceOf<HttpStatusCodeResult>(actionResult, "Not BadRequestResult returned from controller");
			var statusCodeResult = (HttpStatusCodeResult)actionResult;
			Assert.That(statusCodeResult.StatusCode, Is.EqualTo(400));
		}

		public void AssertPartialViewResultReturned(ActionResult actionResult, string partialViewName)
		{
			Assert.IsNotNull(actionResult, "No result was returned from controller");
			Assert.IsInstanceOf<PartialViewResult>(actionResult, "Not PartialViewResult returned from controller");
			var viewResult = (PartialViewResult)actionResult;
			if (!string.IsNullOrEmpty(partialViewName))
			{
				Assert.AreEqual(partialViewName, viewResult.ViewName);
			}
		}

		public void AssertRedirectToActionReturned(ActionResult result, object action, string controller)
		{
			Assert.IsNotNull(result, "No result was returned from controller");
			Assert.IsInstanceOf<System.Web.Mvc.RedirectToRouteResult>(result, "Not RedirectToRouteResult returned from controller");
			var redirectResult = (System.Web.Mvc.RedirectToRouteResult)result;
			Assert.AreEqual(action, redirectResult.RouteValues.Values.ToList()[0]);
			Assert.AreEqual(controller, redirectResult.RouteValues.Values.ToList()[1]);

		}

		public void AssertRedirectToActionReturned(ActionResult result, object routeValue, object action, string controller)
		{
			Assert.IsNotNull(result, "No result was returned from controller");
			Assert.IsInstanceOf<System.Web.Mvc.RedirectToRouteResult>(result, "Not RedirectToRouteResult returned from controller");
			var redirectResult = (System.Web.Mvc.RedirectToRouteResult)result;
			Assert.AreEqual(routeValue, redirectResult.RouteValues.Values.ToList()[0], "Route variable was not correct");
			Assert.AreEqual(action, redirectResult.RouteValues.Values.ToList()[1], "Route action was not correct");
			Assert.AreEqual(controller, redirectResult.RouteValues.Values.ToList()[2], "Route controller name was not correct");

		}
		protected void ExpectGetUserId()
		{
			UserIdentity.Expect(a => a.GetUserId(Arg<Controller>.Is.Anything)).Return(TestUserId);
		}

		protected void ExpectGetRequester()
		{
			UserIdentity.Expect(a => a.GetRequester(Arg<Controller>.Is.Anything, Arg<AppSensorDetectionPointKind?>.Is.Anything)).Return(new Requester());
		}
		protected void ExpectGetUserName(string userName)
		{
			UserIdentity.Expect(a => a.GetUserName(Arg<Controller>.Is.Anything)).Return(userName);
		}
		protected void ExpectActingUserIsAdmin(bool isAdmin)
		{
			UserIdentity.Expect(a => a.IsUserInRole(Arg<Controller>.Is.Anything, Arg<string>.Is.Equal("Admin"))).Return(isAdmin);
		}
		protected void StubQueryString(string key, string value)
		{
			var requestItems = new NameValueCollection { { key, value } };
			HttpRequest.Stub(a => a.QueryString).Return(requestItems);
		}


		protected void StubReceivedAjaxRequest(bool isAjaxRequest)
		{
			var headers = new WebHeaderCollection();
			if (isAjaxRequest) headers.Add("X-Requested-With", "XMLHttpRequest");
			HttpRequest.Expect(a => a.Headers).Return(headers);
		}

		protected void AssertViewOrPartialResultReturned(ActionResult result, bool isPartial, string expectedView)
		{
			if (isPartial)
			{
				AssertPartialViewResultReturned(result, expectedView);
			}
			else
			{
				AssertViewResultReturned(result, expectedView);
			}
		}

		protected void StubHttpContextCurrent()
		{
			System.Web.HttpContext.Current = new HttpContext(
				new HttpRequest("", "http://tempuri.org", ""),
				new HttpResponse(new StringWriter())
			);
		}
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				// Supressing IDisposable issue
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~BaseControllerTest()
		{
			Dispose(false);
		}
	}
}
