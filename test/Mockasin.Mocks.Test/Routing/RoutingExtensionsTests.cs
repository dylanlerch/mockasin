using Mockasin.Mocks.Router;
using System;
using Xunit;

namespace Mockasin.Mocks.Test.Routing
{
	public class RoutingExtensionsTests
	{
		[Fact]
		public void SplitPath_NullPath_ThrowsArgumentException()
		{
			// Arrange
			string value = null;

			// Act
			var exception = Assert.Throws<ArgumentNullException>(() => value.SplitPath());

			// Assert
			Assert.Equal("Path can not be null (Parameter 'path')", exception.Message);
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
		public void SplitPath_ValidPath_SplitsPath(string path, string[] expectedSplitPath)
		{
			// Act
			var splitPath = path.SplitPath();

			// Assert
			Assert.Equal(expectedSplitPath, splitPath);
		}
	}
}