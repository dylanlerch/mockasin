using System.Text.Json;

namespace Mockasin.Mocks
{
	public class MockResponse
	{
		public int StatusCode { get; set; }
		public JsonDocument Response { get; set; }
	}
}
