using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Mockasin.Mocks.Endpoints
{
	public class EndpointAction
	{
		[JsonPropertyName("verb")]
		public string Verb { get; set; } = EndpointActionVerb.Any;

		[JsonPropertyName("mode")]
		public string Mode { get; set; } = EndpointActionMode.Single;
		
		[JsonPropertyName("singleResponseIndex")]
		public int SingleResponseIndex { get; set; } = 0;
		
		[JsonPropertyName("responses")]
		public List<Response> Responses { get; set; } = new List<Response>();
	}
}
