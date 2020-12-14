using System;
using System.Collections.Generic;
using Mockasin.Mocks.Endpoints;
using Mockasin.Services;
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

		[Theory]
		[InlineData("GET", "GET")]
		[InlineData("POST", "POST")]
		[InlineData("CUSTOM", "CUSTOM")]
		[InlineData("cUsToMcAsE", "CuSToMCASE")]
		[InlineData("        WHITESPACE", "WHITESPACE       ")]
		public void MatchedMethod_MatchingMethods_ReturnTrue(string setMethod, string givenMethod)
		{
			// Arrange
			var action = new EndpointAction
			{
				Method = setMethod
			};

			// Act
			var matches = action.MatchesMethod(givenMethod);

			// Assert
			Assert.True(matches);
		}

		[Theory]
		[InlineData("GET", "POST")]
		[InlineData("POST", "GET")]
		[InlineData("CUSTOM", "POST")]
		[InlineData("PoSt", "CuSToM")]
		[InlineData("PoSt", "ANY")] // If any is the given method, it should still fail
		public void MatchedMethod_MismatchingMethods_ReturnTrue(string setMethod, string givenMethod)
		{
			// Arrange
			var action = new EndpointAction
			{
				Method = setMethod
			};

			// Act
			var matches = action.MatchesMethod(givenMethod);

			// Assert
			Assert.False(matches);
		}

		[Fact]
		public void GetResponse_NoResponsesAvailable_ReturnsNull()
		{
			// Arrange
			var random = new Mock<IRandomService>();
			var action = new EndpointAction
			{
				Responses = null,
			};

			// Act
			var response = action.GetResponse(random.Object);

			// Assert
			Assert.Null(response);
		}

		[Fact]
		public void GetResponse_InterceptMode_ThrowsException()
		{
			// Arrange
			var random = new Mock<IRandomService>();
			var action = new EndpointAction
			{
				Mode = EndpointActionMode.Intercept,
				Responses = new List<Response>
				{
					new Response()
				}
			};

			// Act
			var exception = Assert.Throws<NotImplementedException>(() => action.GetResponse(random.Object));

			// Assert
			Assert.Equal("Intercept mode is not currently supported", exception.Message);
		}

		[Theory]
		[InlineData(0)]
		[InlineData(1)]
		[InlineData(10)]
		[InlineData(100)]
		[InlineData(int.MaxValue)]
		[InlineData(int.MinValue)]
		public void GetResponse_RandomModeSingleResponse_ReturnsSingleResponse(int weight)
		{
			// Arrange
			var random = new Mock<IRandomService>();
			var response = new Response
			{
				RandomWeight = weight
			};
			var action = new EndpointAction
			{
				Mode = EndpointActionMode.Random,
				Responses = new List<Response>
				{
					response
				}
			};

			// Act
			var actualResponse = action.GetResponse(random.Object);

			// Assert
			Assert.Same(response, actualResponse);
		}

		[Fact]
		public void GetResponse_RandomModeMultipleResponsesLowestRandom_ReturnsFirst()
		{
			// Arrange
			var random = new Mock<IRandomService>();
			random.Setup(m => m.GetRandomIntegerInclusive(It.IsAny<int>(), It.IsAny<int>())).Returns(1);

			var response1 = new Response
			{
				RandomWeight = 1
			};
			var response2 = new Response
			{
				RandomWeight = 1
			};

			var action = new EndpointAction
			{
				Mode = EndpointActionMode.Random,
				Responses = new List<Response>
				{
					response1,
					response2
				}
			};

			// Act
			var actualResponse = action.GetResponse(random.Object);

			// Assert
			Assert.Same(response1, actualResponse);
		}

		[Fact]
		public void GetResponse_RandomModeMultipleResponsesHighestRandom_ReturnsLast()
		{
			// Arrange
			var random = new Mock<IRandomService>();
			random.Setup(m => m.GetRandomIntegerInclusive(It.IsAny<int>(), It.IsAny<int>())).Returns(2);

			var response1 = new Response
			{
				RandomWeight = 1
			};
			var response2 = new Response
			{
				RandomWeight = 1
			};

			var action = new EndpointAction
			{
				Mode = EndpointActionMode.Random,
				Responses = new List<Response>
				{
					response1,
					response2
				}
			};

			// Act
			var actualResponse = action.GetResponse(random.Object);

			// Assert
			Assert.Same(response2, actualResponse);
		}

		[Fact]
		public void GetResponse_RandomModeMultipleResponsesRandomLowerThanMinumum_ReturnsFirst()
		{
			// Arrange
			var random = new Mock<IRandomService>();
			random.Setup(m => m.GetRandomIntegerInclusive(It.IsAny<int>(), It.IsAny<int>())).Returns(0);

			var response1 = new Response
			{
				RandomWeight = 1
			};
			var response2 = new Response
			{
				RandomWeight = 1
			};

			var action = new EndpointAction
			{
				Mode = EndpointActionMode.Random,
				Responses = new List<Response>
				{
					response1,
					response2
				}
			};

			// Act
			var actualResponse = action.GetResponse(random.Object);

			// Assert
			Assert.Same(response1, actualResponse);
		}

		[Fact]
		public void GetResponse_RandomModeMultipleResponsesRandomHigherThanMinumum_ReturnsFirst()
		{
			// Arrange
			var random = new Mock<IRandomService>();
			random.Setup(m => m.GetRandomIntegerInclusive(It.IsAny<int>(), It.IsAny<int>())).Returns(3);

			var response1 = new Response
			{
				RandomWeight = 1
			};
			var response2 = new Response
			{
				RandomWeight = 1
			};

			var action = new EndpointAction
			{
				Mode = EndpointActionMode.Random,
				Responses = new List<Response>
				{
					response1,
					response2
				}
			};

			// Act
			var actualResponse = action.GetResponse(random.Object);

			// Assert
			Assert.Same(response1, actualResponse);
		}


		[Theory]
		[InlineData(0, 0)] // Out of bounds
		[InlineData(1, 0)]
		[InlineData(2, 0)]
		[InlineData(3, 0)]
		[InlineData(4, 1)]
		[InlineData(5, 1)]
		[InlineData(6, 1)]
		[InlineData(7, 1)]
		[InlineData(8, 1)]
		[InlineData(9, 2)]
		[InlineData(10, 0)] // Out of bounds
		public void GetResponse_RandomModeMultipleResponsesVaryingWeights_ReturnsCorrectWeightedResponse(int randomValue, int responseIndex)
		{
			// Arrange
			var random = new Mock<IRandomService>();
			random.Setup(m => m.GetRandomIntegerInclusive(It.IsAny<int>(), It.IsAny<int>())).Returns(randomValue);

			var response1 = new Response
			{
				RandomWeight = 3
			};
			var response2 = new Response
			{
				RandomWeight = 5
			};
			var response3 = new Response
			{
				RandomWeight = 1
			};

			var responses = new List<Response>
			{
					response1,
					response2,
					response3
			};

			var action = new EndpointAction
			{
				Mode = EndpointActionMode.Random,
				Responses = responses
			};

			// Act
			var actualResponse = action.GetResponse(random.Object);

			// Assert
			Assert.Same(responses[responseIndex], actualResponse);
		}

		[Fact]
		public void GetResponse_SingleModeDefaultIndexSingleResponse_ReturnsFirstResponse()
		{
			// Arrange
			var random = new Mock<IRandomService>();
			var response = new Response();

			var action = new EndpointAction
			{
				Mode = EndpointActionMode.Single,
				Responses = new List<Response>
				{
					response
				}
			};

			// Act
			var actualResponse = action.GetResponse(random.Object);

			// Assert
			Assert.Same(response, actualResponse);
		}

		[Fact]
		public void GetResponse_SingleModeDefaultIndexTwoResponses_ReturnsFirstResponse()
		{
			// Arrange
			var random = new Mock<IRandomService>();
			var response1 = new Response();
			var response2 = new Response();

			var action = new EndpointAction
			{
				Mode = EndpointActionMode.Single,
				Responses = new List<Response>
				{
					response1,
					response2
				}
			};

			// Act
			var actualResponse = action.GetResponse(random.Object);

			// Assert
			Assert.Same(response1, actualResponse);
		}

		[Fact]
		public void GetResponse_SingleModeDefaultIndexManyResponses_ReturnsFirstResponse()
		{
			// Arrange
			var random = new Mock<IRandomService>();
			var response1 = new Response();
			var response2 = new Response();
			var response3 = new Response();
			var response4 = new Response();
			var response5 = new Response();

			var action = new EndpointAction
			{
				Mode = EndpointActionMode.Single,
				Responses = new List<Response>
				{
					response1,
					response2,
					response3,
					response4,
					response5
				}
			};

			// Act
			var actualResponse = action.GetResponse(random.Object);

			// Assert
			Assert.Same(response1, actualResponse);
		}

		[Fact]
		public void GetResponse_SingleModeSecondIndexTwoResponses_ReturnsFirstResponse()
		{
			// Arrange
			var random = new Mock<IRandomService>();
			var response1 = new Response();
			var response2 = new Response();

			var action = new EndpointAction
			{
				Mode = EndpointActionMode.Single,
				SingleResponseIndex = 1,
				Responses = new List<Response>
				{
					response1,
					response2
				}
			};

			// Act
			var actualResponse = action.GetResponse(random.Object);

			// Assert
			Assert.Same(response2, actualResponse);
		}

		[Theory]
		[InlineData(0, 0)]
		[InlineData(1, 1)]
		[InlineData(2, 2)]
		[InlineData(3, 3)]
		[InlineData(4, 4)]
		public void GetResponse_SingleModeSpecificIndexManyResponses_ReturnsExpectedResponse(int singleResponseIndex, int expectedResponseIndex)
		{
			// Arrange
			var random = new Mock<IRandomService>();

			var response1 = new Response();
			var response2 = new Response();
			var response3 = new Response();
			var response4 = new Response();
			var response5 = new Response();

			var responses = new List<Response>()
			{
				response1,
				response2,
				response3,
				response4,
				response5
			};

			var action = new EndpointAction
			{
				Mode = EndpointActionMode.Single,
				SingleResponseIndex = singleResponseIndex,
				Responses = responses
			};

			// Act
			var actualResponse = action.GetResponse(random.Object);

			// Assert
			Assert.Same(responses[expectedResponseIndex], actualResponse);
		}

		[Fact]
		public void GetResponse_SingleModeNoResponses_ReturnsNull()
		{
			// Arrange
			var random = new Mock<IRandomService>();
			var action = new EndpointAction
			{
				Mode = EndpointActionMode.Single,
				Responses = new List<Response>()
			};

			// Act
			var actualResponse = action.GetResponse(random.Object);

			// Assert
			Assert.Null(actualResponse);
		}

		[Fact]
		public void GetResponse_SingleModeNullResponses_ReturnsNull()
		{
			// Arrange
			var random = new Mock<IRandomService>();
			var action = new EndpointAction
			{
				Mode = EndpointActionMode.Single,
				Responses = null
			};

			// Act
			var actualResponse = action.GetResponse(random.Object);

			// Assert
			Assert.Null(actualResponse);
		}

		[Theory]
		[InlineData(-1)]
		[InlineData(3)]
		[InlineData(10)]
		[InlineData(int.MinValue)]
		[InlineData(int.MaxValue)]
		public void GetResponse_SingleModeResponseIndexOutOfBounds_DefaultsToFirstResponse(int singleResponseIndex)
		{
			// Arrange
			var random = new Mock<IRandomService>();
			var response1 = new Response();
			var response2 = new Response();
			var response3 = new Response();

			var action = new EndpointAction
			{
				Mode = EndpointActionMode.Single,
				SingleResponseIndex = singleResponseIndex,
				Responses = new List<Response>
				{
					response1,
					response2,
					response3
				}
			};

			// Act
			var actualResponse = action.GetResponse(random.Object);

			// Assert
			Assert.Same(response1, actualResponse);
		}

		[Fact]
		public void GetResponse_InvalidModeMultipleResponses_DefaultsToSingleMode()
		{
			// Arrange
			var random = new Mock<IRandomService>();
			var response1 = new Response();
			var response2 = new Response();

			var action = new EndpointAction
			{
				Mode = "Something that's not even a mode and never will be",
				SingleResponseIndex = 0,
				Responses = new List<Response>
				{
					response1,
					response2
				}
			};

			// Act
			var actualResponse = action.GetResponse(random.Object);

			// Assert
			Assert.Same(response1, actualResponse);
		}
	}
}