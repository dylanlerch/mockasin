using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Mockasin.Mocks.Models
{
	public class Response
	{
		[JsonPropertyName("statusCode")]
		public int StatusCode { get; set; } = 200;

		[JsonPropertyName("body")]
		public JsonElement Body { get; set; }

		[JsonPropertyName("headers")]
		public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
	}
}