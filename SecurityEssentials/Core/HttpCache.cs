using System;
using System.Web;
using System.Web.Caching;

namespace SecurityEssentials.Core
{
	public class HttpCache : IHttpCache
	{

		public object GetCache(string key)
		{
			return HttpRuntime.Cache[key];
		}

		public void SetCache(string key, string value)
		{
			var cacheValue = HttpRuntime.Cache[key];
			if (cacheValue == null)
			{
				HttpRuntime.Cache.Add(key, value, null,
					DateTime.Now.AddDays(1),
					Cache.NoSlidingExpiration,
					CacheItemPriority.Low,
					null);
			}
			else
			{
				HttpRuntime.Cache[key] = value;
			}
		}

		public void RemoveFromCache(string key)
		{
			var cacheValue = HttpRuntime.Cache[key];
			if (cacheValue != null)
			{
				HttpRuntime.Cache.Remove(key);
			}
		}

	}
}