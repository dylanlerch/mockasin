using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Mockasin.Mocks.Endpoints
{
	public class Endpoint
	{
		[JsonPropertyName("path")]
		public string Path { get; set; }

		[JsonPropertyName("actions")]
		public List<Action> Actions { get; set; }
		
		[JsonPropertyName("endpoints")]
		public List<Endpoint> Endpoints { get; set; } = new List<Endpoint>();
	}
}
