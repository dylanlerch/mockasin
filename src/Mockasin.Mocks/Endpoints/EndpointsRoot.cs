using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Mockasin.Mocks.Endpoints
{
	public class EndpointsRoot
	{
		[JsonPropertyName("endpoints")]
		public List<Endpoint> Endpoints { get; set; }

		[JsonIgnore]
		public bool IsInvalid { get => ErrorMessage is object; }

		[JsonIgnore]
		public string ErrorMessage { get; private set; }

		public static EndpointsRoot LoadFromFile(string fileName, ILogger logger = null)
		{
			try
			{
				var file = File.ReadAllText(fileName);
				return JsonSerializer.Deserialize<EndpointsRoot>(file);
			}
			catch (Exception e) when (e is ArgumentException || e is ArgumentNullException || e is PathTooLongException || e is DirectoryNotFoundException || e is IOException || e is UnauthorizedAccessException || e is FileNotFoundException || e is NotSupportedException || e is SecurityException)
			{
				return new EndpointsRoot
				{
					ErrorMessage = $"Error loading configuration file: {e.Message}"
				};
			}
			catch (JsonException e)
			{
				return new EndpointsRoot
				{
					ErrorMessage = $"Error reading configuration file: {e.Message}"
				};
			}
			catch (Exception e)
			{
				var errorMessage = $"An unexpected error occurred attemping to read the configuration file.";
				
				if (logger is object)
				{
					logger.LogError(e, errorMessage);
				}
				
				return new EndpointsRoot
				{
					ErrorMessage = errorMessage
				};
			}
		}
	}
}
