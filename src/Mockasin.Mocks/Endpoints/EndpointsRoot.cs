using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Mockasin.Mocks.Endpoints 
{
	public class EndpointsRoot
	{
		[JsonPropertyName("endpoints")]
		public List<Endpoint> Endpoints { get; set; }

		public static EndpointsRoot LoadFromFile(string fileName)
		{
			var file = File.ReadAllText(fileName);
			return JsonSerializer.Deserialize<EndpointsRoot>(file);
		}

		public static async Task<EndpointsRoot> LoadFromFileAsync(string fileName)
		{
			using (var stream = File.OpenRead(fileName))
			{
				return await JsonSerializer.DeserializeAsync<EndpointsRoot>(stream);
			}
		}
	}
}
