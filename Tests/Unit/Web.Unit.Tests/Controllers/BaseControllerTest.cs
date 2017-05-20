using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SecurityEssentials.Core;
using SecurityEssentials.Core.Identity;
using SecurityEssentials.Model;
using SecurityEssentials.Unit.Tests.TestDbSet;

namespace SecurityEssentials.Unit.Tests.Controllers
{
    public abstract class BaseControllerTest
    {
        protected IAppSensor AppSensor;
        protected ISEContext Context;
        protected string EncryptedSecurityAnswer = "encryptedSecurityAnswer";
        protected string FirstName = "Bob";
        protected HttpContextBase HttpContext;
        protected HttpRequestBase HttpRequest;
        protected HttpSessionStateBase HttpSession;
        protected DateTime LastAccountActivity = DateTime.Parse("2016-05-10");
        protected int TestUserId = 5;
        protected string TestUserName = "testuserName@test.com";
        protected IUserIdentity UserIdentity;

        protected void BaseSetup()
        {
            AppSensor = MockRepository.GenerateMock<IAppSensor>();
            Context = MockRepository.GenerateStub<ISEContext>();
            Context.LookupItem = new TestDbSet<LookupItem>();
            Context.User = new TestDbSet<User> {GetUser()};
            Context.Stub(a => a.SaveChangesAsync()).Return(Task.FromResult(0));
            UserIdentity = MockRepository.GenerateMock<IUserIdentity>();
            HttpSession = MockRepository.GenerateMock<HttpSessionStateBase>();
            HttpContext = MockRepository.GenerateMock<HttpContextBase>();
            HttpRequest = MockRepository.GenerateMock<HttpRequestBase>();
            HttpContext.Stub(c => c.Request).Return(HttpRequest);
            HttpContext.Stub(c => c.Session).Return(HttpSession);
        }

        protected void VerifyAllExpectations()
        {
            AppSensor.VerifyAllExpectations();
            HttpContext.VerifyAllExpectations();
            HttpRequest.VerifyAllExpectations();
            HttpSession.VerifyAllExpectations();
            UserIdentity.VerifyAllExpectations();
        }

        protected User GetUser()
        {
            return new User
            {
                Id = TestUserId,
                Enabled = true,
                Approved = true,
                EmailVerified = true,
                FirstName = FirstName,
                UserName = TestUserName,
                EmailConfirmationToken = "test1",
                SecurityQuestionLookupItemId = 1,
                SecurityQuestionLookupItem = new LookupItem {Id = 1, Description = "test question"},
                SecurityAnswer = EncryptedSecurityAnswer,
                UserLogs = new List<UserLog>
                {
                    new UserLog {Id = 2, DateCreated = DateTime.Parse("2016-06-10"), Description = "did stuff"},
                    new UserLog {Id = 1, DateCreated = LastAccountActivity, Description = "did old stuff"}
                }
            };
        }

        public dynamic AssertJsonResultReturned(ActionResult actionResult)
        {
            Assert.IsNotNull(actionResult, "No result was returned from controller");
            Assert.IsInstanceOfType(actionResult, typeof(JsonResult), "Not JsonResult returned from controller");
            var jsonResult = (JsonResult) actionResult;
            Assert.IsNotNull(jsonResult.Data, "Json Result should contain data");
            var serializer = new JavaScriptSerializer();
            var responseObject = serializer.Serialize(jsonResult.Data) as dynamic;
            var response = serializer.Deserialize<dynamic>(responseObject);
            return response;
        }

        public void AssertViewResultWithError(ActionResult actionResult, string errorValue)
        {
            Assert.IsNotNull(actionResult, "No result was returned from controller");
            Assert.IsInstanceOfType(actionResult, typeof(ViewResult), "Not ViewResult returned from controller");
            var viewResult = (ViewResult) actionResult;
            Assert.IsNotNull(viewResult.ViewData.ModelState);
            Assert.IsFalse(viewResult.ViewData.ModelState.ToList().Count == 0);
            var error = viewResult.ViewData.ModelState.ToList()[0].Value.Errors.ToList();
            Assert.AreEqual(errorValue, error[0].ErrorMessage);
        }

        public T AssertViewResultReturnsType<T>(ActionResult actionResult)
        {
            Assert.IsNotNull(actionResult, "No result was returned from controller");
            Assert.IsInstanceOfType(actionResult, typeof(ViewResult), "Not ViewResult returned from controller");
            var viewResult = (ViewResult) actionResult;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(T), "Not expected type returned as data");
            return (T) viewResult.ViewData.Model;
        }

        public ViewResult AssertViewResultReturned(ActionResult actionResult, string viewName)
        {
            Assert.IsNotNull(actionResult, "No result was returned from controller");
            Assert.IsInstanceOfType(actionResult, typeof(ViewResult), "Not ViewResult returned from controller");
            var viewResult = (ViewResult) actionResult;
            if (!string.IsNullOrEmpty(viewName))
                Assert.AreEqual(viewName, viewResult.ViewName);
            return viewResult;
        }

        public void AssertPartialViewResultReturned(ActionResult actionResult, string partialViewName)
        {
            Assert.IsNotNull(actionResult, "No result was returned from controller");
            Assert.IsInstanceOfType(actionResult, typeof(PartialViewResult),
                "Not PartialViewResult returned from controller");
            var viewResult = (PartialViewResult) actionResult;
            if (!string.IsNullOrEmpty(partialViewName))
                Assert.AreEqual(partialViewName, viewResult.ViewName);
        }

        public void AssertRedirectToActionReturned(ActionResult result, object action, string controller)
        {
            Assert.IsNotNull(result, "No result was returned from controller");
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult),
                "Not RedirectToRouteResult returned from controller");
            var redirectResult = (RedirectToRouteResult) result;
            Assert.AreEqual(action, redirectResult.RouteValues.Values.ToList()[0]);
            Assert.AreEqual(controller, redirectResult.RouteValues.Values.ToList()[1]);
        }
    }
}