using System.Web.Mvc;

namespace SecurityEssentials.Core
{
	public interface IHttpCache
	{
		object GetCache(string key);
		void RemoveFromCache(string key);
		void SetCache(string key, string value);
	}
}