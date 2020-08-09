using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Mockasin.Mocks.Endpoints
{
	public class Response
	{
		[JsonPropertyName("statusCode")]
		public int StatusCode { get; set; } = 200;

		[JsonPropertyName("headers")]
		public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

		[JsonPropertyName("jsonBody")]
		public JsonElement jsonBody { get; set; }
	}
}
