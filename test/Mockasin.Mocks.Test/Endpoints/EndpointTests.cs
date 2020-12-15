using System.Collections.Generic;
using Mockasin.Mocks.Endpoints;
using Moq;
using Xunit;

namespace Mockasin.Mocks.Test.Endpoints
{
	public class EndpointTests
	{
		[Theory]
		[InlineData(null, null)]
		[InlineData(null, new string[0])]
		[InlineData(null, new string[] { "" })]
		[InlineData("", null)]
		[InlineData("", new string[0])]
		[InlineData("", new string[] { "" })]
		[InlineData(" ", null)]
		[InlineData(" ", new string[0])]
		[InlineData(" ", new string[] { "" })]
		[InlineData("//", null)]
		[InlineData("//", new string[0])]
		[InlineData("//", new string[] { "" })]
		[InlineData("  /  ", null)]
		[InlineData("  /  ", new string[0])]
		[InlineData("  /  ", new string[] { "" })]
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
		[InlineData("/     spaces    /    before     /    and    /     after    /   each   /   part   /   dont   /    matter    ", new string[] { "spaces", "before", "and", "after", "each", "part", "dont", "matter" })]
		[InlineData("/spaces/before/and/after/each/part/dont/matter", new string[] { "     spaces   ", "    before   ", "   and   ", "   after   ", "  each  ", "  part  ", "   dont   ", "   matter   " })]
		[InlineData("         /spaces/at/the/start/and/end/dont/matter         ", new string[] { "spaces", "at", "the", "start", "and", "end", "dont", "matter" })]
		[InlineData("/spaces/at/the/start/and/end/dont/matter", new string[] { "       spaces", "at", "the", "start", "and", "end", "dont", "matter      " })]
		[InlineData("///", new string[] { "", "" })]
		public void MatchesPath_PathsMatchExactly_ReturnTrueAndEmptyRemainingElements(string endpointPath, string[] matchPath)
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
		[InlineData("///", new string[] { "", "", "" }, new string[] { "" })]
		public void MatchesPath_PathsMatchPrefix_ReturnTrueAndRemainingElements(string endpointPath, string[] matchPath, string[] expectedRemaining)
		{
			// Arrange
			var endpoint = new Endpoint { Path = endpointPath };

			// Act
			var match = endpoint.MatchesPath(matchPath, out var remaining);

			// Assert
			Assert.True(match);
			Assert.Equal(expectedRemaining, remaining);
		}

		[Fact]
		public void GetActionWithMatchingMethod_NullActions_ReturnsNull()
		{
			// Arrange
			var endpoint = new Endpoint();

			// Act
			var action = endpoint.GetActionWithMatchingMethod("anyMethod");

			// Assert
			Assert.Null(action);
		}

		[Fact]
		public void GetActionWithMatchingMethod_NoActions_ReturnsNull()
		{
			// Arrange
			var endpoint = new Endpoint { Actions = new List<EndpointAction>() };

			// Act
			var action = endpoint.GetActionWithMatchingMethod("anyMethod");

			// Assert
			Assert.Null(action);
		}

		[Fact]
		public void GetActionWithMatchingMethod_SingleActionMatches_ReturnsSingleAction()
		{
			// Arrange
			var action = new Mock<EndpointAction>();
			action.Setup(m => m.MatchesMethod(It.IsAny<string>())).Returns(true);
			var endpoint = new Endpoint
			{
				Actions = new List<EndpointAction> { action.Object }
			};

			// Act
			var match = endpoint.GetActionWithMatchingMethod("method");

			// Assert
			Assert.Same(action.Object, match);
		}

		[Fact]
		public void GetActionWithMatchingMethod_MultipleActionMatches_ReturnsFirstAction()
		{
			// Arrange
			var action1 = new Mock<EndpointAction>();
			action1.Setup(m => m.MatchesMethod(It.IsAny<string>())).Returns(true);

			var action2 = new Mock<EndpointAction>();
			action2.Setup(m => m.MatchesMethod(It.IsAny<string>())).Returns(true);

			var endpoint = new Endpoint
			{
				Actions = new List<EndpointAction> { action1.Object, action2.Object }
			};

			// Act
			var match = endpoint.GetActionWithMatchingMethod("method");

			// Assert
			Assert.Same(action1.Object, match);
		}

		[Fact]
		public void GetActionWithMatchingMethod_MultipleActionsNoneMatch_ReturnNull()
		{
			// Arrange
			var action1 = new Mock<EndpointAction>();
			action1.Setup(m => m.MatchesMethod(It.IsAny<string>())).Returns(false);

			var action2 = new Mock<EndpointAction>();
			action2.Setup(m => m.MatchesMethod(It.IsAny<string>())).Returns(false);

			var endpoint = new Endpoint
			{
				Actions = new List<EndpointAction> { action1.Object, action2.Object }
			};

			// Act
			var match = endpoint.GetActionWithMatchingMethod("method");

			// Assert
			Assert.Null(match);
		}
	}
}