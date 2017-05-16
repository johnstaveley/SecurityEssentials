using System.Net;

namespace SecurityEssentials.Acceptance.Tests.Utility
{
    public static class HttpWeb
    {
        public static WebResponse Get(string url)
        {
            var httpGet = WebRequest.Create(url);
            return httpGet.GetResponse();
        }
    }
}