using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecurityEssentials.Core;
using System.Web;
using SecurityEssentials.Core.Identity;
using Rhino.Mocks;
using SecurityEssentials.Model;
using System.Collections.Generic;
using System;
using SecurityEssentials.Unit.Tests.TestDbSet;
using System.Threading.Tasks;
using System.Web.Routing;
using System.Web.Script.Serialization;

namespace SecurityEssentials.Unit.Tests.Controllers
{
    public abstract class BaseControllerTest
    {

        protected ISEContext _context;
        protected HttpContextBase _httpContext;
        protected HttpRequestBase _httpRequest;
        protected HttpSessionStateBase _httpSession;
        protected IUserIdentity _userIdentity;
        protected string _encryptedSecurityAnswer = "encryptedSecurityAnswer";
        protected string _firstName = "Bob";
        protected string _testUserName = "testuserName@test.com";
        protected int _testUserId = 5;
        protected DateTime _lastAccountActivity = DateTime.Parse("2016-05-10");
          
        protected void BaseSetup()
        {
            _context = MockRepository.GenerateStub<ISEContext>();
            _context.LookupItem = new TestDbSet<LookupItem>();
            _context.User = new TestDbSet<User>();
            _context.User.Add(GetUser());
            _context.Stub(a => a.SaveChangesAsync()).Return(Task.FromResult(0));
            _userIdentity = MockRepository.GenerateMock<IUserIdentity>();
            _httpSession = MockRepository.GenerateMock<HttpSessionStateBase>();
            _httpContext = MockRepository.GenerateMock<HttpContextBase>();
            _httpRequest = MockRepository.GenerateMock<HttpRequestBase>();
            _httpContext.Stub(c => c.Request).Return(_httpRequest);
            _httpContext.Stub(c => c.Session).Return(_httpSession);

        }

        protected void VerifyAllExpectations()
        {
            _httpContext.VerifyAllExpectations();
            _httpRequest.VerifyAllExpectations();
            _httpSession.VerifyAllExpectations();
            _userIdentity.VerifyAllExpectations();

        }

        protected User GetUser()
        {
            return new User()
            {
                Id = _testUserId,
                Enabled = true,
                Approved = true,
                EmailVerified = true,
                FirstName = _firstName,
                UserName = _testUserName,
                EmailConfirmationToken = "test1",
                SecurityQuestionLookupItemId = 1,
                SecurityQuestionLookupItem = new LookupItem() { Id = 1, Description = "test question" },
                SecurityAnswer = _encryptedSecurityAnswer,
                UserLogs = new List<UserLog>() {
                new UserLog() { Id = 2, DateCreated = DateTime.Parse("2016-06-10"), Description = "did stuff" },
                new UserLog() { Id = 1, DateCreated = _lastAccountActivity, Description = "did old stuff" }
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
            return (T)viewResult.ViewData.Model;

        }

        public ViewResult AssertViewResultReturned(ActionResult actionResult, string viewName)
        {
            Assert.IsNotNull(actionResult, "No result was returned from controller");
            Assert.IsInstanceOfType(actionResult, typeof(ViewResult), "Not ViewResult returned from controller");
            var viewResult = (ViewResult)actionResult;
            if (!string.IsNullOrEmpty(viewName))
            {
                Assert.AreEqual(viewName, viewResult.ViewName);
            }
            return viewResult;
        }

        public void AssertPartialViewResultReturned(ActionResult actionResult, string partialViewName)
        {
            Assert.IsNotNull(actionResult, "No result was returned from controller");
            Assert.IsInstanceOfType(actionResult, typeof(PartialViewResult), "Not PartialViewResult returned from controller");
            var viewResult = (PartialViewResult) actionResult;
            if (!string.IsNullOrEmpty(partialViewName))
            {
                Assert.AreEqual(partialViewName, viewResult.ViewName);
            }
        }

        public void AssertRedirectToActionReturned(ActionResult result, object action, string controller)
        {
            Assert.IsNotNull(result, "No result was returned from controller");
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult), "Not RedirectToRouteResult returned from controller");
            var redirectResult = (RedirectToRouteResult)result;
            Assert.AreEqual(action, redirectResult.RouteValues.Values.ToList()[0]);
            Assert.AreEqual(controller, redirectResult.RouteValues.Values.ToList()[1]);

        }

    }
}
