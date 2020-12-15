using System;
using System.IO;
using System.Security;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Mockasin.Mocks.Configuration;
using Mockasin.Mocks.Endpoints;
using Mockasin.Mocks.Validation;
using Mockasin.Mocks.Validation.Abstractions;

namespace Mockasin.Mocks.Store
{
	public interface IMockStore
	{
		IEndpointsRoot GetEndpointsRoot();
	}

	public class MockStore : IMockStore
	{
		private EndpointsRoot _responses;
		private readonly object _responsesLock = new object();

		private readonly IMockSettings _settings;
		private readonly IMockSectionValidator<EndpointsRoot> _validator;
		private readonly ILogger<MockStore> _logger;

		public MockStore(IMockSettings settings, IMockSectionValidator<EndpointsRoot> validator, ILogger<MockStore> logger)
		{
			_settings = settings;
			_validator = validator;
			_logger = logger;
		}

		public IEndpointsRoot GetEndpointsRoot()
		{
			lock (_responsesLock)
			{
				if (_responses is null)
				{
					_responses = LoadFromFile();
				}
			}

			return _responses;
		}

		private EndpointsRoot LoadFromFile()
		{
			EndpointsRoot root;

			try
			{
				var file = File.ReadAllText(_settings.Mock.ConfigurationPath);
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
				_logger.LogError(e, errorMessage);
				return new EndpointsRoot(errorMessage);
			}

			var validationResult = _validator.Validate(root, new SectionName("$"));

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
