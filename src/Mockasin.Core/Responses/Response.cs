using System.Collections.Generic;
using System.Text.Json;

namespace Mockasin.Core.Responses
{
	public class Response
	{
		public int StatusCode { get; set; } = 0;
		public JsonDocument Body { get; set; }
	}
}
