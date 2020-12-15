using System.Collections.Generic;
using Mockasin.Mocks.Endpoints;
using Mockasin.Services;
using Moq;
using Xunit;

namespace Mockasin.Mocks.Test.Endpoints
{
	public class EndpointsRootTests
	{
		[Fact]
		public void GetResponse_NoParameters_CreatesEmptyEndpointRoot()
		{
			// Arrange
			var random = new Mock<IRandomService>();
			var root = new EndpointsRoot();

			// Act
			var response = root.GetResponse("ANY", "ANY", random.Object);

			// Assert
			Assert.Null(root.Endpoints);
			Assert.Equal(404, response.StatusCode);
			Assert.Null(response.StringBody);
		}

		[Fact]
		public void GetResponse_WithErrorMessage_ReturnsErrorResponse()
		{
			// Arrange
			var random = new Mock<IRandomService>();
			var root = new EndpointsRoot("Error message");

			// Act
			var response = root.GetResponse("ANY", "ANY", random.Object);

			// Assert
			Assert.Null(root.Endpoints);
			Assert.Equal(500, response.StatusCode);
			Assert.Equal("Error message", response.StringBody);
		}

		[Fact]
		public void GetResponse_NullEndpoints_ReturnsNull()
		{
			// Arrange
			var random = new Mock<IRandomService>();
			var root = new EndpointsRoot
			{
				Endpoints = null
			};

			// Act
			var response = root.GetResponse("GET", "/a/b/c", random.Object);

			// Assert
			Assert.Null(root.Endpoints);
			Assert.Equal(404, response.StatusCode);
			Assert.Null(response.StringBody);
		}

		[Fact]
		public void GetResponse_EmptyEndpoints_ReturnsNull()
		{
			// Arrange
			var random = new Mock<IRandomService>();
			var root = new EndpointsRoot
			{
				Endpoints = new List<IEndpoint>()
			};

			// Act
			var response = root.GetResponse("GET", "/a/b/c", random.Object);

			// Assert
			Assert.Empty(root.Endpoints);
			Assert.Equal(404, response.StatusCode);
			Assert.Null(response.StringBody);
		}

		[Theory]
		[InlineData(null, "a/b/c")]
		[InlineData("GET", null)]
		[InlineData(null, null)]
		public void GetResponse_NullMethodAndPath_ReturnsNull(string method, string path)
		{
			// Arrange
			var random = new Mock<IRandomService>();
			var root = new EndpointsRoot();

			// Act
			var response = root.GetResponse(method, path, random.Object);

			// Assert
			Assert.Null(root.Endpoints);
			Assert.Equal(404, response.StatusCode);
			Assert.Null(response.StringBody);
		}


		delegate void MockMatchesPath(string[] path, out string[] remaining);

		[Fact]
		public void GetResponse_FirstMatchesPath_ReturnsFirst()
		{
			// Arrange
			var random = new Mock<IRandomService>();

			var response = new Response();

			var action = new Mock<IEndpointAction>();
			action.Setup(m => m.GetResponse(It.IsAny<IRandomService>()))
				.Returns(response);

			var endpoint = new Mock<IEndpoint>();
			endpoint.Setup(m => m.MatchesPath(It.IsAny<string[]>(), out It.Ref<string[]>.IsAny))
				.Callback(new MockMatchesPath((string[] path, out string[] remaining) => { remaining = new string[0]; }))
				.Returns(true);
			endpoint.Setup(m => m.GetActionWithMatchingMethod(It.IsAny<string>()))
				.Returns(action.Object);

			var endpoints = new List<IEndpoint> { endpoint.Object };

			var root = new EndpointsRoot { Endpoints = endpoints };

			// Act
			var actualResponse = root.GetResponse("GET", "some/path", random.Object);

			// Assert
			Assert.Same(response, actualResponse);
		}

		[Fact]
		public void GetResponse_SecondMatchesPath_ReturnsSecond()
		{
			// Arrange
			var random = new Mock<IRandomService>();

			var response1 = new Response();

			var action1 = new Mock<IEndpointAction>();
			action1.Setup(m => m.GetResponse(It.IsAny<IRandomService>()))
				.Returns(response1);

			var endpoint1 = new Mock<IEndpoint>();
			endpoint1.Setup(m => m.MatchesPath(It.IsAny<string[]>(), out It.Ref<string[]>.IsAny))
				.Returns(false);


			var response2 = new Response();

			var action2 = new Mock<IEndpointAction>();
			action2.Setup(m => m.GetResponse(It.IsAny<IRandomService>()))
				.Returns(response2);

			var endpoint2 = new Mock<IEndpoint>();
			endpoint2.Setup(m => m.MatchesPath(It.IsAny<string[]>(), out It.Ref<string[]>.IsAny))
				.Callback(new MockMatchesPath((string[] path, out string[] remaining) => { remaining = new string[0]; }))
				.Returns(true);
			endpoint2.Setup(m => m.GetActionWithMatchingMethod(It.IsAny<string>()))
				.Returns(action2.Object);

			var endpoints = new List<IEndpoint>
			{
				endpoint1.Object,
				endpoint2.Object
			};

			var root = new EndpointsRoot { Endpoints = endpoints };

			// Act
			var actualResponse = root.GetResponse("GET", "some/path", random.Object);

			// Assert
			Assert.Same(response2, actualResponse);
		}

		[Fact]
		public void GetResponse_BothMatchPathButFirstDoesntMatchMethod_ReturnsSecond()
		{
			// Arrange
			var random = new Mock<IRandomService>();

			var response1 = new Response();

			var action1 = new Mock<IEndpointAction>();
			action1.Setup(m => m.GetResponse(It.IsAny<IRandomService>()))
				.Returns(response1);

			var endpoint1 = new Mock<IEndpoint>();
			endpoint1.Setup(m => m.MatchesPath(It.IsAny<string[]>(), out It.Ref<string[]>.IsAny))
				.Callback(new MockMatchesPath((string[] path, out string[] remaining) => { remaining = new string[0]; }))
				.Returns(true);
			endpoint1.Setup(m => m.GetActionWithMatchingMethod(It.IsAny<string>()))
				.Returns<IEndpointAction>(null);


			var response2 = new Response();

			var action2 = new Mock<IEndpointAction>();
			action2.Setup(m => m.GetResponse(It.IsAny<IRandomService>()))
				.Returns(response2);

			var endpoint2 = new Mock<IEndpoint>();
			endpoint2.Setup(m => m.MatchesPath(It.IsAny<string[]>(), out It.Ref<string[]>.IsAny))
				.Callback(new MockMatchesPath((string[] path, out string[] remaining) => { remaining = new string[0]; }))
				.Returns(true);
			endpoint2.Setup(m => m.GetActionWithMatchingMethod(It.IsAny<string>()))
				.Returns(action2.Object);

			var endpoints = new List<IEndpoint>
			{
				endpoint1.Object,
				endpoint2.Object
			};

			var root = new EndpointsRoot { Endpoints = endpoints };

			// Act
			var actualResponse = root.GetResponse("GET", "some/path", random.Object);

			// Assert
			Assert.Same(response2, actualResponse);
		}
	}
}