using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace Mockasin.Web.Configuration
{
	public static class JsonConfiguration
	{
		public static void Configure(JsonOptions options)
		{
			var converters = options.JsonSerializerOptions.Converters;
			
			converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
		}
	}
}
