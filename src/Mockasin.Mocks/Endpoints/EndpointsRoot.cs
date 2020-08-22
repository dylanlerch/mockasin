using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Mockasin.Mocks.Validation;
using Mockasin.Mocks.Validation.Abstractions;

namespace Mockasin.Mocks.Endpoints
{
	public class EndpointsRoot
	{
		[JsonPropertyName("endpoints")]
		public List<Endpoint> Endpoints { get; set; }

		[JsonIgnore]
		public EndpointsRootStatus Status { get; set; } = new EndpointsRootStatus();

		public EndpointsRoot() { }

		public EndpointsRoot(string errorMessage)
		{
			Status.ErrorMessage = errorMessage;
		}

		public static EndpointsRoot LoadFromFile(string fileName, IMockSectionValidator<EndpointsRoot> validator, ILogger logger = null)
		{
			EndpointsRoot root;

			try
			{
				var file = File.ReadAllText(fileName);
				root = JsonSerializer.Deserialize<EndpointsRoot>(file);
			}
			catch (Exception e) when (e is ArgumentException || e is ArgumentNullException || e is PathTooLongException || e is DirectoryNotFoundException || e is IOException || e is UnauthorizedAccessException || e is FileNotFoundException || e is NotSupportedException || e is SecurityException)
			{
				return new EndpointsRoot($"Error loading configuration file: {e.Message}");
			}
			catch (JsonException e)
			{
				return new EndpointsRoot($"Error reading configuration file: {e.Message}");
			}
			catch (Exception e)
			{
				var errorMessage = $"An unexpected error occurred attemping to read the configuration file.";
				logger.LogError(e, errorMessage);
				return new EndpointsRoot(errorMessage);
			}

			var validationResult = validator.Validate(root, new SectionName("$"));

			if (validationResult.HasErrors)
			{
				var errorMessage = new StringBuilder();
				errorMessage.AppendLine("Configuration file was read correctly but failed validation. Errors:");

				foreach (var validationError in validationResult.Errors)
				{
					errorMessage.AppendLine($"  - {validationError}");
				}

				// Remove the final new line character
				errorMessage.Length--;

				return new EndpointsRoot(errorMessage.ToString());
			}

			return root;
		}
	}
}
