namespace SecurityEssentials.Acceptance.Tests.Utility
{
    using Newtonsoft.Json;
    using System.IO;

    namespace SecurityEssentials.Acceptance.Tests.Utility
    {
        public class NewtonsoftJsonSerializer : RestSharp.Serializers.ISerializer
		{
			private readonly  JsonSerializer _serializer;

			public NewtonsoftJsonSerializer(JsonSerializer serializer)
			{
				_serializer = serializer;
			}

			public string ContentType
			{
				get => "application/json";
                set { }
			}

			public string DateFormat { get; set; }

			public string Namespace { get; set; }

			public string RootElement { get; set; }

			public string Serialize(object obj)
			{
				using (var stringWriter = new StringWriter())
				{
					using (var jsonTextWriter = new JsonTextWriter(stringWriter))
					{
						_serializer.Serialize(jsonTextWriter, obj);
						return stringWriter.ToString();
					}
				}
			}

			public T Deserialize<T>(RestSharp.RestResponse response)
			{
				var content = response.Content;

				using (var stringReader = new StringReader(content))
				{
					using (var jsonTextReader = new JsonTextReader(stringReader))
					{
						return _serializer.Deserialize<T>(jsonTextReader);
					}
				}
			}

			public static NewtonsoftJsonSerializer Default => new NewtonsoftJsonSerializer(new JsonSerializer
			{
				NullValueHandling = NullValueHandling.Ignore,
			});
		}
	}
}
