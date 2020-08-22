using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Mockasin.Mocks.Configuration;
using Mockasin.Mocks.Endpoints;
using Mockasin.Mocks.Validation.Abstractions;

namespace Mockasin.Mocks.Router
{
	public class MockRouter : IMockRouter
	{
		private EndpointsRoot _responses;
		private readonly IMockSettings _settings;
		private readonly IMockSectionValidator<EndpointsRoot> _validator;
		private readonly ILogger<MockRouter> _logger;
		private static readonly Random _random = new Random();

		public MockRouter(IMockSettings settings, IMockSectionValidator<EndpointsRoot> validator, ILogger<MockRouter> logger)
		{
			_settings = settings;
			_validator = validator;
			_logger = logger;
		}

		public Response Route(string method, string path)
		{
			// Always reload data on each request while were building and testing things
			_responses = EndpointsRoot.LoadFromFile(_settings.Mock.ConfigurationPath, _validator, _logger);

			if (_responses.Status.IsInvalid)
			{
				// When loading the file, EndpointsRoot catches all errors with
				// loading the file, reading the JSON, or any custom validation
				// that has failed. If it's in an error state, return that error
				// to the caller.
				return ErrorResponse(_responses.Status.ErrorMessage);
			}

			// Otherwise, we know we have a valid endpoint structure. Get the
			// matching endpoint if there is one.
			var endpoints = GetEndpointsForPath(path, _responses);
			if (endpoints.Length == 0)
			{
				return NotFoundResponse();
			}

			var action = GetEndpointActionForMethod(method, endpoints);
			if (action is null)
			{
				return NotFoundResponse();
			}

			return GetResponseForEndpointAction(action);
		}


		public static Endpoint[] GetEndpointsForPath(string path, EndpointsRoot root)
		{
			var pathParts = path.SplitPath();
			return GetEndpointsForPathParts(pathParts, root.Endpoints).ToArray();
		}

		private static List<Endpoint> GetEndpointsForPathParts(string[] pathParts, List<Endpoint> endpoints)
		{
			var matchingEndpoints = new List<Endpoint>();

			if (endpoints is object)
			{
				foreach (var endpoint in endpoints)
				{
					if (endpoint.MatchesPath(pathParts, out var remainingPath))
					{
						if (remainingPath.Length == 0)
						{
							// If there are no elements left in the path, then this
							// endpoint is a match.
							matchingEndpoints.Add(endpoint);
						}
						else
						{
							// If there are elements left in the path, traverse
							// the children for this endpoint
							var matchingChildren = GetEndpointsForPathParts(remainingPath, endpoint.Endpoints);
							matchingEndpoints.AddRange(matchingChildren);
						}
					}
				}
			}

			return matchingEndpoints;
		}

		public EndpointAction GetEndpointActionForMethod(string method, IEnumerable<Endpoint> endpoints)
		{
			var methodUpper = method.Trim().ToUpperInvariant();

			foreach (var endpoint in endpoints)
			{
				// This is a flat collection of endpoints, no need to traverse
				// any of the children.
				if (endpoint.Actions is object)
				{
					foreach (var action in endpoint.Actions)
					{
						var actionMethodUpper = action.Method.Trim().ToUpperInvariant();
						if (actionMethodUpper == EndpointActionMethod.Any || actionMethodUpper == methodUpper)
						{
							// First match wins, return the first match
							return action;
						}
					}
				}
			}

			// There is no matching action in any of the endpoints
			return null;
		}

		public Response GetResponseForEndpointAction(EndpointAction action)
		{
			if (action.Responses.Count == 0)
			{
				return NotFoundResponse();
			}

			var modeUpper = action.Mode.Trim().ToUpperInvariant();
			if (modeUpper == EndpointActionMode.Intercept)
			{
				throw new NotImplementedException("Intercept mode is not currently supported");
			}
			else if (modeUpper == EndpointActionMode.Random)
			{
				// Each random response has a different weight that controls
				// how likely it is to be returned.
				int totalWeight = 0;
				foreach (var response in action.Responses)
				{
					totalWeight += response.RandomWeight;
				}

				int randomWeight;
				lock (_random)
				{
					randomWeight = _random.Next(1, totalWeight + 1);
				}

				var cumulativeWeight = 0;
				foreach (var response in action.Responses)
				{
					cumulativeWeight += response.RandomWeight;
					if (cumulativeWeight >= randomWeight)
					{
						return response;
					}
				}

				// If loop through all of the responses and none are returned,
				// then all the weights are zero. Just return the first one.
				return action.Responses[0];
			}
			else
			{
				// Default to single mode. If someone has managed to configure
				// a mode that is not in the valid list, it will default to
				// single response mode

				if (action.SingleResponseIndex < action.Responses.Count)
				{
					return action.Responses[action.SingleResponseIndex];
				}
				else
				{
					return action.Responses[0];
				}
			}
		}

		private Response NotFoundResponse()
		{
			return new Response
			{
				StatusCode = 404
			};
		}

		private Response ErrorResponse(string errorMessage)
		{
			return new Response
			{
				StatusCode = 500,
				StringBody = errorMessage
			};
		}
	}
}
