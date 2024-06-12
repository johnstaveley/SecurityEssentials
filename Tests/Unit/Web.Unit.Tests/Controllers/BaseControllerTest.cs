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

		public dynamic AssertJsonResultReturned(ActionResult actionResult)
		{
			Assert.That(actionResult, Is.Not.Null, "No result was returned from controller");
			Assert.That(actionResult, Is.InstanceOf<JsonResult>(), "Not JsonResult returned from controller");
			var jsonResult = (JsonResult)actionResult;
			Assert.That(jsonResult.Data, Is.Not.Null, "Json Result should contain data");
			var serializer = new JavaScriptSerializer();
			var responseObject = serializer.Serialize(jsonResult.Data) as dynamic;
			var response = serializer.Deserialize<dynamic>(responseObject);
			return response;
		}

		public void AssertViewResultWithError(ActionResult actionResult, string errorValue)
		{
			Assert.That(actionResult, Is.Not.Null, "No result was returned from controller");
			Assert.That(actionResult, Is.InstanceOf<ViewResult>(), "Not ViewResult returned from controller");
			var viewResult = (ViewResult)actionResult;
			Assert.That(viewResult.ViewData.ModelState, Is.Not.Null);
			Assert.That(viewResult.ViewData.ModelState.ToList().Count == 0, Is.False);
			var error = viewResult.ViewData.ModelState.ToList().Select(a => a.Value.Errors).Where(b => b.Count > 0).ToList();
			Assert.That(errorValue, Is.EqualTo(error[0][0].ErrorMessage));

		}

		public T AssertViewResultReturnsType<T>(ActionResult actionResult)
		{
			Assert.That(actionResult, Is.Not.Null, "No result was returned from controller");
			Assert.That(actionResult, Is.InstanceOf<ViewResult>(), "Not ViewResult returned from controller");
			var viewResult = (ViewResult)actionResult;
			Assert.That(viewResult.ViewData.Model, Is.Not.EqualTo("Not expected type returned as data"));
			return (T)viewResult.ViewData.Model;

		}
		public T AssertHttpContentReturnsType<T>(IHttpActionResult result)
		{
			Assert.That(result, Is.InstanceOf<OkNegotiatedContentResult<T>>());
			var conNegResult = result as OkNegotiatedContentResult<T>;
			Assert.That(conNegResult, Is.Not.Null);
			return conNegResult.Content;
		}
		public ViewResult AssertViewResultReturned(ActionResult actionResult, string viewName)
		{
			Assert.That(actionResult, Is.Not.Null, "No result was returned from controller");
			Assert.That(actionResult, Is.InstanceOf<ViewResult>(), "Not ViewResult returned from controller");
			var viewResult = (ViewResult)actionResult;
			if (!string.IsNullOrEmpty(viewName))
			{
				Assert.That(viewName, Is.EqualTo(viewResult.ViewName));
			}
			return viewResult;
		}

		public void AssertNotFoundReturned(ActionResult actionResult)
		{
			Assert.That(actionResult, Is.Not.Null, "No result was returned from controller");
			Assert.That(actionResult, Is.InstanceOf<HttpNotFoundResult>(), "Not HttpNotFoundResult returned from controller");
		}

		public void AssertBadRequestReturned(ActionResult actionResult)
		{
			Assert.That(actionResult, Is.Not.Null, "No result was returned from controller");
			Assert.That(actionResult, Is.InstanceOf<HttpStatusCodeResult>(), "Not BadRequestResult returned from controller");
			var statusCodeResult = (HttpStatusCodeResult)actionResult;
			Assert.That(statusCodeResult.StatusCode, Is.EqualTo(400));
		}

		public void AssertPartialViewResultReturned(ActionResult actionResult, string partialViewName)
		{
			Assert.That(actionResult, Is.Not.Null, "No result was returned from controller");
			Assert.That(actionResult, Is.InstanceOf<PartialViewResult>(), "Not PartialViewResult returned from controller");
			var viewResult = (PartialViewResult)actionResult;
			if (!string.IsNullOrEmpty(partialViewName))
			{
				Assert.That(partialViewName, Is.EqualTo(viewResult.ViewName));
			}
		}

		public void AssertRedirectToActionReturned(ActionResult result, object action, string controller)
		{
			Assert.That(result, Is.Not.Null, "No result was returned from controller");
			Assert.That(result, Is.InstanceOf<System.Web.Mvc.RedirectToRouteResult>(), "Not RedirectToRouteResult returned from controller");
			var redirectResult = (System.Web.Mvc.RedirectToRouteResult)result;
			Assert.That(action, Is.EqualTo(redirectResult.RouteValues.Values.ToList()[0]));
			Assert.That(controller, Is.EqualTo(redirectResult.RouteValues.Values.ToList()[1]));

		}

		public void AssertRedirectToActionReturned(ActionResult result, object routeValue, object action, string controller)
		{
			Assert.That(result, Is.Not.Null, "No result was returned from controller");
			Assert.That(result, Is.InstanceOf<System.Web.Mvc.RedirectToRouteResult>(), "Not RedirectToRouteResult returned from controller");
			var redirectResult = (System.Web.Mvc.RedirectToRouteResult)result;
			Assert.That(routeValue, Is.EqualTo(redirectResult.RouteValues.Values.ToList()[0]), "Route variable was not correct");
			Assert.That(action, Is.EqualTo(redirectResult.RouteValues.Values.ToList()[1]), "Route action was not correct");
			Assert.That(controller, Is.EqualTo(redirectResult.RouteValues.Values.ToList()[2]), "Route controller name was not correct");

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
