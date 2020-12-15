using System.Collections.Generic;
using System.Text.Json.Serialization;
using Mockasin.Mocks.Router;
using Mockasin.Services;

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

		public Response GetResponse(string method, string path, IRandomService random)
		{
			if (Status.IsInvalid)
			{
				// If the EndpointsRoot is in an error state, return the error
				// message immediately.
				return ErrorResponse(Status.ErrorMessage);
			}

			// Otherwise, we know we have a valid endpoint structure. Get the
			// matching endpoint if there is one.
			var pathParts = path.SplitPath();
			var response = GetResponseForPathParts(method, pathParts, Endpoints, random);

			if (response is null)
			{
				return NotFoundResponse();
			}

			return response;
		}

		private Response GetResponseForPathParts(string method, string[] pathParts, List<Endpoint> endpoints, IRandomService random)
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
