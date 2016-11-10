using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Caching;
using System.Net;
using System.Web.Routing;
using System.Globalization;

namespace SecurityEssentials.Core.Attributes
{

	/// <summary>
	/// Decorates any MVC route that needs to have client requests limited by time.
	/// </summary>
	/// <remarks>
	/// Uses the current System.Web.Caching.Cache to store each client request to the decorated route.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class AllowXRequestsEveryXSecondsAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// A unique name for this Throttle.
        /// </summary>
        /// <remarks>
        /// We'll be inserting a Cache record based on this name and client IP, e.g. "Name-192.168.0.1"
        /// </remarks>
        public string Name { get; set; }

        /// <summary>
        /// The number of seconds clients must wait before executing this decorated route again.
        /// </summary>
        public int Seconds { get; set; }

        /// <summary>
        /// The number of requests to allow per client in the given number of seconds
        /// </summary>
        public int Requests { get; set; }

        /// <summary>
        /// A text message (not themed) that will be sent to the client upon throttling.  You can include the token {n} to
        /// show this.Seconds in the message, e.g. "You have performed this action more than {x} times in the last {n} seconds.".
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The content name (themed and from SiteContent) to show upon throttling.  If this is present, the Message parameter will not be used.
        /// </summary>
        public string ContentName { get; set; }

        // Used to get around weird cache behavior with value types
        public class Int32Value
        {
            public Int32Value()
            {
                Value = 1;
            }
            public int Value { get; set; }
        }

        public override void OnActionExecuting(ActionExecutingContext c)
        {
            if (c == null) throw new ArgumentException("ActionExecutingContext not spcecified");
            var key = string.Concat("AllowXRequestsEveryXSeconds-", Name, "-", c.HttpContext.Request.UserHostAddress);
            var allowExecute = false;

            var currentCacheValue = HttpRuntime.Cache[key];
            if (currentCacheValue == null)
            {
                HttpRuntime.Cache.Add(key,
                    new Int32Value(),
                    null, // no dependencies
                    DateTime.Now.AddSeconds(Seconds), // absolute expiration
                    Cache.NoSlidingExpiration,
                    CacheItemPriority.Low,
                    null); // no callback

                allowExecute = true;
            }
            else
            {
                var value = (Int32Value)currentCacheValue;
                value.Value++;
                if (value.Value <= Requests)
                {
                    allowExecute = true;
                }
            }

            if (!allowExecute)
            {
                if (String.IsNullOrEmpty(Message))
                    Message = "You have performed this action more than {x} times in the last {n} seconds.";

                if (!string.IsNullOrEmpty(ContentName))
                {
                    //use SiteContent
                    c.Result = new RedirectToRouteResult(new RouteValueDictionary { { "Controller", "Content" }, { "Action", ContentName } });
                }
                else
                {
                    //just send a message (not themed)
                    c.Result = new ContentResult { Content = Message.Replace("{x}", Requests.ToString(CultureInfo.CurrentCulture)).Replace("{n}", Seconds.ToString(CultureInfo.CurrentCulture)) };
                }

                // see 409 - http://www.w3.org/Protocols/rfc2616/rfc2616-sec10.html
                c.HttpContext.Response.TrySkipIisCustomErrors = true; //to prevent iis from showing default 409 page
                c.HttpContext.Response.StatusCode = (int)HttpStatusCode.Conflict;
            }
        }
    }
}