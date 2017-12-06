using Newtonsoft.Json;
using RestSharp;
using System.Text;
using RestSharp.Serializers;
using SecurityEssentials.Acceptance.Tests.Utility.SecurityEssentials.Acceptance.Tests.Utility;

namespace SecurityEssentials.Acceptance.Tests.Utility
{
	public static class HttpWeb
	{

		public static IRestResponse Get(string url)
		{
			var client = new RestClient(url);
			var request = new RestRequest(Method.GET);
			return client.Execute(request);
		}

		public static IRestResponse PostJsonStream(string url, object body)
		{
			var client = new RestClient(url);
			var request = new RestRequest(Method.POST)
			{
				JsonSerializer = NewtonsoftJsonSerializer.Default,
				RequestFormat = DataFormat.Json
			};
			request.AddJsonBody(body);
			return client.Execute(request);
		}
	}
}
