using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Mockasin.Mocks.Endpoints
{
	public interface IEndpointAction
	{
		string Method { get; set; }
		string Mode { get; set; }
		int SingleResponseIndex { get; set; }
		List<Response> Responses { get; set; }

		bool MatchesMethod(string method);
		Response GetResponse();
	}

	public class EndpointAction : IEndpointAction
	{
		[JsonPropertyName("method")]
		public string Method { get; set; } = EndpointActionMethod.Any;

		[JsonPropertyName("mode")]
		public string Mode { get; set; } = EndpointActionMode.Single;

		[JsonPropertyName("singleResponseIndex")]
		public int SingleResponseIndex { get; set; } = 0;

		[JsonPropertyName("responses")]
		public List<Response> Responses { get; set; } = new List<Response>();

		private static readonly Random _random = new Random();

		public bool MatchesMethod(string method)
		{
			var actionMethodUpper = Method.Trim().ToUpperInvariant();

			// ANY is a special case. If the method for the action is ANY, then
			// any given method should always match.
			if (actionMethodUpper == EndpointActionMethod.Any)
			{
				return true;
			}

			var givenMethodUpper = method.Trim().ToUpperInvariant();
			return actionMethodUpper == givenMethodUpper;
		}

		public Response GetResponse()
		{
			if (Responses.Count == 0)
			{
				return null;
			}

			var modeUpper = Mode.Trim().ToUpperInvariant();

			if (modeUpper == EndpointActionMode.Intercept)
			{
				return GetInterceptResponse();
			}
			else if (modeUpper == EndpointActionMode.Random)
			{
				return GetRandomResponse();
			}
			else
			{
				// Default to single mode. If someone has managed to configure
				// a mode that is not in the valid list, it will default to
				// single response mode
				return GetSingleResponse();
			}
		}

		private Response GetInterceptResponse()
		{
			throw new NotImplementedException("Intercept mode is not currently supported");
		}

		private Response GetRandomResponse()
		{
			// Each random response has a different weight that controls
			// how likely it is to be returned.
			int totalWeight = 0;
			foreach (var response in Responses)
			{
				totalWeight += response.RandomWeight;
			}

			int randomWeight;
			lock (_random)
			{
				randomWeight = _random.Next(1, totalWeight + 1);
			}

			var cumulativeWeight = 0;
			foreach (var response in Responses)
			{
				cumulativeWeight += response.RandomWeight;
				if (cumulativeWeight >= randomWeight)
				{
					return response;
				}
			}

			// If loop through all of the responses and none are returned,
			// then all the weights are zero. Just return the first one.
			return Responses[0];
		}

		private Response GetSingleResponse()
		{
			if (SingleResponseIndex < Responses.Count)
			{
				return Responses[SingleResponseIndex];
			}
			else
			{
				return Responses[0];
			}
		}
	}
}
