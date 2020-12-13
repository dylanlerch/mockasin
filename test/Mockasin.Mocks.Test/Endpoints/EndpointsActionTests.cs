using System.Collections.Generic;
using Mockasin.Mocks.Endpoints;
using Mockasin.Mocks.Test.TestData;
using Mockasin.Mocks.Validation;
using Mockasin.Mocks.Validation.Abstractions;
using Moq;
using Xunit;

namespace Mockasin.Mocks.Test.Endpoints
{
	public class EndpointsActionTests
	{
		[Fact]
		public void Constructor_DefaultMethod_IsAny()
		{
			// Arrange
			var action = new EndpointAction();

			// Assert
			Assert.Equal("ANY", action.Method);
		}

		[Theory]
		[InlineData("")]
		[InlineData("GET")]
		[InlineData("POST")]
		[InlineData("PUT")]
		[InlineData("NONSTANDARD")]
		public void MatchedMethod_SetAsAny_AllMethodsReturnTrue(string givenMethod)
		{
			// Arrange
			var action = new EndpointAction
			{
				Method = "ANY"
			};

			// Act
			var matches = action.MatchesMethod(givenMethod);

			// Assert
			Assert.True(matches);
		}

		[Theory]
		[InlineData("ANY")]
		[InlineData("any")]
		[InlineData("AnY")]
		[InlineData("  ANY   ")]
		[InlineData("   any     ")]
		public void MatchedMethod_SetAsAnyWithVaryingCase_AllMethodsReturnTrue(string setMethod)
		{
			// Arrange
			var action = new EndpointAction
			{
				Method = setMethod
			};

			// Act
			var matches = action.MatchesMethod("");

			// Assert
			Assert.True(matches);
		}
	}
}