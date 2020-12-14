using System.Collections.Generic;
using Mockasin.Mocks.Endpoints;
using Mockasin.Mocks.Test.TestData;
using Mockasin.Mocks.Validation;
using Mockasin.Mocks.Validation.Abstractions;
using Mockasin.Services;
using Moq;
using Xunit;

namespace Mockasin.Mocks.Test.Endpoints
{
	public class EndpointsRootTests
	{
		private readonly Mock<IMockSectionValidator<EndpointsRoot>> _validator = new Mock<IMockSectionValidator<EndpointsRoot>>();

		public EndpointsRootTests()
		{
			// Default validator returns a successful validation
			_validator.Setup(m => m.Validate(It.IsAny<EndpointsRoot>(), It.IsAny<SectionName>())).Returns(new ValidationResult());
		}

		[Fact]
		public void Constructor_NoParameters_CreatesValidEmptyEndpointRoot()
		{
			// Arrange + Act
			var root = new EndpointsRoot();

			// Assert
			Assert.Null(root.Endpoints);
			Assert.False(root.Status.IsInvalid);
		}

		[Fact]
		public void Constructor_WithErrorMessage_CreatesInvalidEmptyEndpointRoot()
		{
			// Arrange + Act
			var root = new EndpointsRoot("Error message");

			// Assert
			Assert.Null(root.Endpoints);
			Assert.True(root.Status.IsInvalid);
			Assert.Equal("Error message", root.Status.ErrorMessage);
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
			Assert.Null(response);
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
			Assert.Null(response);
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
			Assert.Null(response);
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


		[Fact]
		public void LoadFromFile_FileDoesNotExist_CreatesInvalidEmptyEndpointRoot()
		{
			// Arrange
			var path = Data.GetPath("/some/invalid/path/that/does/not/exist.json");

			// Act
			var root = EndpointsRoot.LoadFromFile(path, _validator.Object);

			// Assert
			Assert.Null(root.Endpoints);
			Assert.True(root.Status.IsInvalid);
			Assert.StartsWith("Error loading configuration file:", root.Status.ErrorMessage);
		}

		[Theory]
		[InlineData(Data.Files.Blank)]
		[InlineData(Data.Files.Invalid)]
		public void LoadFromFile_FileIsInvalid_CreatesInvalidEmptyEndpointRoot(string name)
		{
			// Arrange
			var path = Data.GetPath(name);

			// Act
			var root = EndpointsRoot.LoadFromFile(path, _validator.Object);

			// Assert
			Assert.Null(root.Endpoints);
			Assert.True(root.Status.IsInvalid);
			Assert.StartsWith("Error reading configuration file:", root.Status.ErrorMessage);
		}

		[Fact]
		public void LoadFromFile_FileIsOnlyRootJson_CreatesValidEmptyEndpointRoot()
		{
			// Arrange
			var path = Data.GetPath(Data.Files.OnlyRootJson);

			// Act
			var root = EndpointsRoot.LoadFromFile(path, _validator.Object);

			// Assert
			Assert.Null(root.Endpoints);
			Assert.False(root.Status.IsInvalid);
		}

		[Fact]
		public void LoadFromFile_ValidatorReturnsSingleError_CreatesEndpointRootWithError()
		{
			// Arrange
			var path = Data.GetPath(Data.Files.OnlyRootJson);

			var validationResult = new ValidationResult();
			validationResult.AddError(new SectionName("$"), "Broken");
			_validator.Setup(m => m.Validate(It.IsAny<EndpointsRoot>(), It.IsAny<SectionName>())).Returns(validationResult);

			// Act
			var root = EndpointsRoot.LoadFromFile(path, _validator.Object);

			// Assert
			Assert.Null(root.Endpoints);
			Assert.True(root.Status.IsInvalid);

			var expectedErrorMessage = $@"Configuration file was read correctly but failed validation. Errors:
  - {validationResult.Errors[0]}";
			Assert.Equal(expectedErrorMessage, root.Status.ErrorMessage);
		}

		[Fact]
		public void LoadFromFile_ValidatorReturnsManyErrors_CreatesEndpointRootWithAllErrors()
		{
			// Arrange
			var path = Data.GetPath(Data.Files.OnlyRootJson);

			var validationResult = new ValidationResult();
			validationResult.AddError(new SectionName("$"), "Broken");
			validationResult.AddError(new SectionName("$.path"), "Broken again");
			validationResult.AddError(new SectionName("$.endpoints[0].path[0]"), "Broken once more");
			_validator.Setup(m => m.Validate(It.IsAny<EndpointsRoot>(), It.IsAny<SectionName>())).Returns(validationResult);

			// Act
			var root = EndpointsRoot.LoadFromFile(path, _validator.Object);

			// Assert
			Assert.Null(root.Endpoints);
			Assert.True(root.Status.IsInvalid);

			var expectedErrorMessage = $@"Configuration file was read correctly but failed validation. Errors:
  - {validationResult.Errors[0]}
  - {validationResult.Errors[1]}
  - {validationResult.Errors[2]}";
			Assert.Equal(expectedErrorMessage, root.Status.ErrorMessage);
		}

		[Fact]
		public void LoadFromFile_ValidatorReturnsErrorForSomeInput_EndpointsListIsNull()
		{
			// Arrange
			var path = Data.GetPath(Data.Files.SingleEndpoint);

			var validationResult = new ValidationResult();
			validationResult.AddError(new SectionName("$"), "Broken");
			_validator.Setup(m => m.Validate(It.IsAny<EndpointsRoot>(), It.IsAny<SectionName>())).Returns(validationResult);

			// Act
			var root = EndpointsRoot.LoadFromFile(path, _validator.Object);

			// Assert
			Assert.Null(root.Endpoints); // Even though it deserialised some values, these are not returned in the response when there is an error
			Assert.True(root.Status.IsInvalid);
		}
	}
}