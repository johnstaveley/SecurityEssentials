using RestSharp;
using SecurityEssentials.Acceptance.Tests.Utility.SecurityEssentials.Acceptance.Tests.Utility;

namespace SecurityEssentials.Acceptance.Tests.Utility
{
    public static class HttpWeb
	{

		public static RestResponse Get(string url)
		{
			var client = new RestClient();
			var request = new RestRequest(url, Method.Get);
			return client.Execute(request);
		}

		public static RestResponse PostJsonStream(string url, object body)
		{
			var client = new RestClient();
			var request = new RestRequest(url, Method.Post)
			{
				//JsonSerializer = NewtonsoftJsonSerializer.Default,
				RequestFormat = DataFormat.Json
			};
			request.AddJsonBody(body);
			return client.Execute(request);
		}
	}
}
