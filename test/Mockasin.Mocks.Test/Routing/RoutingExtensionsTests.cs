using Mockasin.Mocks.Router;
using System;
using Xunit;

namespace Mockasin.Mocks.Test.Routing
{
	public class RoutingExtensionsTests
	{
		[Fact]
		public void SplitRoute_NullRoute_ThrowsArgumentException()
		{
			// Arrange
			string value = null;

			// Act
			var exception = Assert.Throws<ArgumentNullException>(() => value.SplitRoute());

			// Assert
			Assert.Equal("Route can not be null (Parameter 'route')", exception.Message);
		}

		[Theory]
		[InlineData("", new string[0])]
		[InlineData("     ", new string[0])]
		[InlineData("/", new string[0])]
		[InlineData("  /  ", new string[0])]
		[InlineData("test", new string[] { "test" })]
		[InlineData("/test", new string[] { "test" })]
		[InlineData("test/", new string[] { "test" })]
		[InlineData("/test/", new string[] { "test" })]
		[InlineData("lots/of/parts/to/test/", new string[] { "lots", "of", "parts", "to", "test" })]
		public void SplitRoute_ValidRoute_SplitsRoute(string route, string[] expectedSplitRoute)
		{
			// Act
			var splitRoute = route.SplitRoute();

			// Assert
			Assert.Equal(expectedSplitRoute, splitRoute);
		}
	}
}