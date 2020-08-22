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
				return new Response
				{
					StatusCode = 500,
					StringBody = _responses.Status.ErrorMessage
				};
			}

			// Otherwise, we know we have a valid endpoint structure. Get the
			// matching endpoint if there is one.
			var endpoint = GetEndpointForPath(path, _responses);

			if (endpoint is null)
			{
				return NotFoundResponse();
			}

			return new Response();
		}


		public static Endpoint GetEndpointForPath(string path, EndpointsRoot root)
		{
			if (path is null)
			{
				return null;
			}

			var pathParts = path.SplitPath();
			return GetEndpointForPathParts(pathParts, root.Endpoints);
		}

		private static Endpoint GetEndpointForPathParts(string[] pathParts, List<Endpoint> endpoints)
		{
			foreach (var endpoint in endpoints)
			{
				if (endpoint.MatchesPath(pathParts, out var remainingPath))
				{
					if (remainingPath.Length == 0)
					{
						// If there are no elements left in the path, then this
						// endpoint is a match
						return endpoint;
					}
					else
					{
						// If there are elements left in the path, traverse
						// the children for this endpoint
						var childMatch = GetEndpointForPathParts(remainingPath, endpoint.Endpoints);

						if (childMatch is object)
						{
							return childMatch;
						}
					}
				}
				else
				{
					return null;
				}
			}

			// If the whole tree has been traversed and there are no matches,
			// the path doesn't exist.
			return null;
		}

		private Response NotFoundResponse()
		{
			return new Response
			{
				StatusCode = 404
			};
		}
	}
}
