using Mockasin.Mocks.Endpoints;
using Mockasin.Mocks.Test.TestData;
using Mockasin.Mocks.Validation;
using Mockasin.Mocks.Validation.Abstractions;
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