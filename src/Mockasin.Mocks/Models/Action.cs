using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Mockasin.Mocks.Models
{
	public class Action
	{
		[JsonPropertyName("verb")]
		public string Verb { get; set; } = ActionVerb.Any;

		[JsonPropertyName("mode")]
		public string Mode { get; set; } = ActionMode.Single;
		
		[JsonPropertyName("singleResponseIndex")]
		public int SingleResponseIndex { get; set; } = 0;
		
		[JsonPropertyName("responses")]
		public List<Response> Responses { get; set; } = new List<Response>();
	}
}
