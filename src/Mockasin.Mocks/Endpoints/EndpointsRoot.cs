using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Mockasin.Mocks.Router;
using Mockasin.Mocks.Validation;
using Mockasin.Mocks.Validation.Abstractions;
using Mockasin.Services;

namespace Mockasin.Mocks.Endpoints
{
	public class EndpointsRoot
	{
		[JsonPropertyName("endpoints")]
		public List<IEndpoint> Endpoints { get; set; }

		[JsonIgnore]
		public EndpointsRootStatus Status { get; set; } = new EndpointsRootStatus();

		public EndpointsRoot() { }

		public EndpointsRoot(string errorMessage)
		{
			Status.ErrorMessage = errorMessage;
		}

		public Response GetResponse(string method, string path, IRandomService random)
		{
			var pathParts = path.SplitPath();
			return GetResponseForPathParts(method, pathParts, Endpoints, random);
		}

		private Response GetResponseForPathParts(string method, string[] pathParts, List<IEndpoint> endpoints, IRandomService random)
		{
			if (endpoints is object)
			{
				foreach (var endpoint in endpoints)
				{
					if (endpoint.MatchesPath(pathParts, out var remainingPath))
					{
						if (remainingPath.Length == 0)
						{
							// If there are no elements left in the path, then this
							// endpoint matches the path. Now, see if there is an
							// action with a method that matches the given method
							var action = endpoint.GetActionWithMatchingMethod(method);
							if (action is object)
							{
								// Actions have multiple responses, and the action
								// determines which of these to return. GetResponse
								// gets the specific response to return.

								// Exit at the first matching action that is found.
								return action.GetResponse(random);
							}
						}
						else
						{
							// If there are elements left in the path, traverse
							// the children for this endpoint
							var response = GetResponseForPathParts(method, remainingPath, endpoint.Endpoints, random);
							if (response is object)
							{
								return response;
							}
						}
					}
				}
			}

			// If there is no response found in the loop above, there is no
			// match in this part of the response tree. Return null.
			return null;
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
