using System.Collections.Generic;
using System.Text.Json;

namespace Mockasin.Mocks.Router
{
	public class MockResponse
	{
		public int StatusCode { get; set; }
		public Dictionary<string, string> Headers { get; set; }
		public string StringBody { get; set; }
		public JsonDocument JsonBody { get; set; }
	}
}
