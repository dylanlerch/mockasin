using System.Collections.Generic;

namespace Mockasin.Core.Responses
{
	public class Endpoint
	{
		public string Path { get; set; }
		public EndpointMode Mode { get; set; } = EndpointMode.Single;
		public string Verb { get; set; } = EndpointVerb.Any;
		public int DefaultResponseIndex { get; set; } = 0;
		public List<Response> Responses { get; set; } = new List<Response>();
		public List<Endpoint> Children { get; set; } = new List<Endpoint>();
	}
}
