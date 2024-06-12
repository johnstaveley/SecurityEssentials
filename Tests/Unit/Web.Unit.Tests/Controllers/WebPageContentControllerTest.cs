using NUnit.Framework;
using SecurityEssentials.Controllers;
using SecurityEssentials.Core.Attributes;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace SecurityEssentials.Unit.Tests.Controllers
{
    [TestFixture]
    public class WebPageContentControllerTest : BaseControllerTest
    {

        private WebPageContentController _sut;

        [SetUp]
        public void Setup()
        {
            BaseSetup();
            _sut = new WebPageContentController
            {
                Url = new UrlHelper(new RequestContext(HttpContext, new RouteData()), new RouteCollection())
            };
            _sut.ControllerContext = new ControllerContext(HttpContext, new RouteData(), _sut);
        }

        [Test]
        public void When_ControllerCreated_Then_IsDecoratedWithNoCache()
        {
            var type = _sut.GetType();
            var attributes = type.GetCustomAttributes(typeof(NoCacheAttribute), true);
            Assert.That(attributes.Any(), "No NoCache Attribute found");
        }

    }
}
