using Mockasin.Mocks.Endpoints;
using Xunit;

namespace Mockasin.Mocks.Test.Endpoints
{
	public class EndpointTests
	{
		[Theory]
		[InlineData(null, null)]
		[InlineData(null, new string[0])]
		[InlineData("", null)]
		[InlineData("", new string[0])]
		[InlineData(" ", null)]
		[InlineData(" ", new string[0])]
		public void MatchesPath_EndpointAndMatchPathEmpty_PathsMatchExactly(string endpointPath, string[] matchPath)
		{
			// Arrange
			var endpoint = new Endpoint { Path = endpointPath };

			// Act
			var match = endpoint.MatchesPath(matchPath, out var remaining);

			// Assert
			Assert.True(match);
			Assert.Empty(remaining);
		}

		[Theory]
		[InlineData("test/path", new string[] { "test" })] // Match path shorter than endpoint path
		[InlineData("/test//path", new string[] { "test", "path" })] // Double slashes result in an empty path element
		[InlineData("/test/path", new string[] { "path", "test" })]
		[InlineData("/test/path", new string[] { "path", "test", "foo", "bar" })]
		public void MatchesPath_PathsDoNotMatch_ReturnFalseAndNull(string endpointPath, string[] matchPath)
		{
			// Arrange
			var endpoint = new Endpoint { Path = endpointPath };

			// Act
			var match = endpoint.MatchesPath(matchPath, out var remaining);

			// Assert
			Assert.False(match);
			Assert.Null(remaining);
		}

		[Theory]
		[InlineData("test", new string[] { "test" })]
		[InlineData("test/path", new string[] { "test", "path" })]
		[InlineData("test/path/that/is/long", new string[] { "test", "path", "that", "is", "long" })]
		[InlineData("match/REGARDLESS/of/CASE", new string[] { "MATCH", "regardless", "OF", "case" })]
		[InlineData("/leading/slashes/dont/matter", new string[] { "leading", "slashes", "dont", "matter" })]
		[InlineData("trailing/slashes/dont/matter/", new string[] { "trailing", "slashes", "dont", "matter" })]
		[InlineData("/leading/and/trailing/slashes/dont/matter/", new string[] { "leading", "and", "trailing", "slashes", "dont", "matter" })]
		public void MatchesPath_PathsMathExactly_ReturnTrueAndEmptyRemainingElements(string endpointPath, string[] matchPath)
		{
			// Arrange
			var endpoint = new Endpoint { Path = endpointPath };

			// Act
			var match = endpoint.MatchesPath(matchPath, out var remaining);

			// Assert
			Assert.True(match);
			Assert.Empty(remaining);
		}

		[Theory]
		[InlineData("test", new string[] { "test", "path" }, new string[] { "path" })]
		[InlineData("/test", new string[] { "test", "path" }, new string[] { "path" })]
		[InlineData("test/path", new string[] { "test", "path", "FOO" }, new string[] { "FOO" })]
		[InlineData("test//path", new string[] { "test", "", "path", "foo" }, new string[] { "foo" })]
		[InlineData("/test//path", new string[] { "test", "", "path", "foo" }, new string[] { "foo" })]
		[InlineData("test/test/test/test", new string[] { "test", "test", "test", "test", "path" }, new string[] { "path" })]
		[InlineData("/test/test/test/test/", new string[] { "test", "test", "test", "test", "path" }, new string[] { "path" })]
		public void MatchesPath_PathsMathExactly_ReturnTrueAndRemainingElements(string endpointPath, string[] matchPath, string[] expectedRemaining)
		{
			// Arrange
			var endpoint = new Endpoint { Path = endpointPath };

			// Act
			var match = endpoint.MatchesPath(matchPath, out var remaining);

			// Assert
			Assert.True(match);
			Assert.Equal(expectedRemaining, remaining);
		}
	}
}