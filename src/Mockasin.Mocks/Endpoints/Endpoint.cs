using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Mockasin.Mocks.Router;

namespace Mockasin.Mocks.Endpoints
{
	public class Endpoint
	{
		[JsonPropertyName("path")]
		public string Path { get; set; }

		[JsonPropertyName("actions")]
		public List<EndpointAction> Actions { get; set; }

		[JsonPropertyName("endpoints")]
		public List<Endpoint> Endpoints { get; set; } = new List<Endpoint>();

		private EndpointDetails Details { get; set; } = new EndpointDetails();

		public bool MatchesPath(string[] pathToMatch, out string[] remainingPath)
		{
			if (Details.SplitPath is null)
			{
				// If there is no stored path currently split, split it
				if (Path is null)
				{
					// If the path is null, can't split. Just make the split
					// path an empty array
					Details.SplitPath = new string[] { "" };
				}
				else
				{
					Details.SplitPath = Path.SplitPath();
				}
			}

			if (pathToMatch is null || pathToMatch.Length == 0)
			{
				// If an invalid path was passed in, convert it to the default 
				// base empty path
				pathToMatch = new string[] { "" };
			}

			if (pathToMatch.Length < Details.SplitPath.Length)
			{
				// If the path to match has fewer parts than the path for this
				// endpoint, it can never match.
				remainingPath = null;
				return false;
			}

			// At this point, the path for this endpoint has the same or fewer
			// elements than path that's being matched. Loop through each
			// element of the path for this endpoint and compare with the
			// element at the same index in path that's being matched. If any of
			// these do not match, we can exit.
			for (int i = 0; i < Details.SplitPath.Length; i++)
			{
				var matchPathElement = pathToMatch[i];
				var endpointPathElement = Details.SplitPath[i];

				if (matchPathElement.ToLowerInvariant() != endpointPathElement.ToLowerInvariant())
				{
					remainingPath = null;
					return false;
				}
			}

			// If reaching this point, the path for this endpoint is a prefix of
			// the path that is being matched. Return true rlkturn a new array
			// with those matched elements removed.
			remainingPath = pathToMatch.Skip(Details.SplitPath.Length).ToArray();
			return true;
		}
	}
}
