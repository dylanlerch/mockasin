using System;
using System.Linq;

namespace Mockasin.Mocks.Router
{
	public static class RoutingExtensions
	{
		public static string[] SplitPath(this string path)
		{
			if (path is null)
			{
				throw new ArgumentNullException(nameof(path), "Path can not be null");
			}

			var pathParts = path.Trim().Split('/');

			// If the path starts or ends with a slash, or is just whitespace,
			// we'll have an empty string at the start or end. Remove that.
			int skip = 0, take = pathParts.Length;

			if (pathParts.Length > 0)
			{
				// If the first element is null or whitespace, we want to skip
				// it
				var firstPathPart = pathParts[0];
				skip = string.IsNullOrWhiteSpace(firstPathPart) ? 1 : 0;

				// If there is more than 1 element (so the first element isn't
				// also the last element), and last element is null or whitespace 
				// then we want to skip it as well. 
				var lastPathPart = pathParts[^1];
				var skipEnd = pathParts.Length > 1 && string.IsNullOrWhiteSpace(lastPathPart) ? 1 : 0;
				take = pathParts.Length - skip - skipEnd;
			}

			return pathParts.Skip(skip).Take(take).ToArray();
		}
	}
}