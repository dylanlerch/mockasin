using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Mockasin.Services;

namespace Mockasin.Mocks.Endpoints
{
	public class EndpointAction
	{
		[JsonPropertyName("method")]
		public string Method { get; set; } = EndpointActionMethod.Any;

		[JsonPropertyName("mode")]
		public string Mode { get; set; } = EndpointActionMode.Single;

		[JsonPropertyName("singleResponseIndex")]
		public int SingleResponseIndex { get; set; } = 0;

		[JsonPropertyName("responses")]
		public List<Response> Responses { get; set; } = new List<Response>();

		public virtual bool MatchesMethod(string method)
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

		public virtual Response GetResponse(IRandomService random)
		{
			if (Responses is null || Responses.Count == 0)
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
				return GetRandomResponse(random);
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

		private Response GetRandomResponse(IRandomService random)
		{
			// Each random response has a different weight that controls
			// how likely it is to be returned.
			int totalWeight = 0;
			foreach (var response in Responses)
			{
				totalWeight += response.RandomWeight;
			}

			// Loop through each of the weights, adding them on to the cumulative
			// weight value. If the cumulative weight for the specific response
			// exceeds the generated value (which is between 1 and the sum of
			// weights) we have the random response. Means what responses with
			// higher relatives weights are proportionately more likely to be
			// chosen
			int randomResponseValue = random.GetRandomIntegerInclusive(1, totalWeight);
			var cumulativeWeight = 0;

			foreach (var response in Responses)
			{
				cumulativeWeight += response.RandomWeight;
				if (cumulativeWeight >= randomResponseValue)
				{
					return response;
				}
			}

			// If loop through all of the responses and none are returned, then
			// all the weights are zero or the random number generate with a 
			// value higher than the sum. Just return the first one.
			return Responses[0];
		}

		private Response GetSingleResponse()
		{
			// If the index is valid for the length of the array
			if (SingleResponseIndex < Responses.Count && SingleResponseIndex >= 0)
			{
				return Responses[SingleResponseIndex];
			}
			else
			{
				// If some wacky index is set, just return the first one.
				return Responses[0];
			}
		}
	}
}
