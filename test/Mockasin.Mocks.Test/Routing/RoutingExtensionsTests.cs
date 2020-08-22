using Mockasin.Mocks.Router;
using System;
using Xunit;

namespace Mockasin.Mocks.Test.Routing
{
	public class RoutingExtensionsTests
	{
		[Theory]
		[InlineData(null, new string[] { "" })]
		[InlineData("", new string[] { "" })]
		[InlineData("     ", new string[] { "" })]
		[InlineData("/", new string[] { "" })]
		[InlineData("  /  ", new string[] { "" })]
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