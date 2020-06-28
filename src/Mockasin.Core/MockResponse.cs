using System.Text.Json;

namespace Mockasin.Core
{
	public class MockResponse
	{
		public int StatusCode { get; set; }
		public JsonDocument Response { get; set; }
	}
}
