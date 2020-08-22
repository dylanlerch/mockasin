using System;
using System.Linq;

namespace Mockasin.Mocks.Router
{
	public static class RoutingExtensions
	{
		public static string[] SplitRoute(this string route)
		{
			if (route is null)
			{
				throw new ArgumentNullException(nameof(route), "Route can not be null");
			}

			var routeParts = route.Trim().Split('/');

			// If the route starts or ends with a slash, or is just whitespace,
			// we'll have an empty string at the start or end. Remove that.
			int skip = 0, take = routeParts.Length;

			if (routeParts.Length > 0)
			{
				// If the first element is null or whitespace, we want to skip
				// it
				var firstRoutePart = routeParts[0];
				skip = string.IsNullOrWhiteSpace(firstRoutePart) ? 1 : 0;

				// If there is more than 1 element (so the first element isn't
				// also the last element), and last element is null or whitespace 
				// then we want to skip it as well. 
				var lastRoutePart = routeParts[^1];
				var skipEnd = routeParts.Length > 1 && string.IsNullOrWhiteSpace(lastRoutePart) ? 1 : 0;
				take = routeParts.Length - skip - skipEnd;
			}

			return routeParts.Skip(skip).Take(take).ToArray();
		}
	}
}