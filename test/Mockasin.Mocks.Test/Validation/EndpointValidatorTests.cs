using System.Collections.Generic;
using Mockasin.Mocks.Endpoints;
using Mockasin.Mocks.Validation;
using Mockasin.Mocks.Validation.Abstractions;
using Moq;
using Xunit;

namespace Mockasin.Mocks.Test.Validation
{
	public class EndpointValidatorTests
	{
		private readonly Mock<IMockSectionValidator<IEndpointAction>> _actionValidator = new Mock<IMockSectionValidator<IEndpointAction>>();
		private readonly SectionName _name = new SectionName("$");
		private readonly string _defaultValidPath = "path";
		private readonly string _defaultInvalidPath = "!!-invalid";
		private readonly List<IEndpointAction> _defaultValidActionList = new List<IEndpointAction>
		{
			new EndpointAction {}
		};

		[Fact]
		public void Validate_NullSection_ReturnsSingleError()
		{
			// Arrange
			var validator = new EndpointValidator(_actionValidator.Object);

			// Act
			var result = validator.Validate(null, _name);

			// Assert
			Assert.True(result.HasErrors);
			var error = Assert.Single(result.Errors);
			Assert.Equal(ErrorMessageFormatter.Format("$", "Endpoint is null"), error);
		}

		[Theory]
		[MemberData(nameof(ValidationData.NullAndWhitespace), MemberType = typeof(ValidationData))]
		public void Validate_NullAndWhitespacePath_ReturnsSingleError(string path)
		{
			// Arrange
			var validator = new EndpointValidator(_actionValidator.Object);
			var section = new Endpoint
			{
				Path = path,
				Actions = _defaultValidActionList
			};

			// Act
			var result = validator.Validate(section, _name);

			// Assert
			Assert.True(result.HasErrors);
			var error = Assert.Single(result.Errors);
			Assert.Equal(ErrorMessageFormatter.Format("$.path", "Endpoint must have a path"), error);
		}

		[Theory]
		[InlineData("path")]
		[InlineData("path/with/parts")]
		[InlineData("path/with/a/to/z")]
		[InlineData("path/with/CAPITALS")]
		public void Validate_ValidPath_ReturnsNoError(string path)
		{
			// Arrange
			var validator = new EndpointValidator(_actionValidator.Object);
			var section = new Endpoint
			{
				Path = path,
				Actions = _defaultValidActionList
			};

			// Act
			var result = validator.Validate(section, _name);

			// Assert
			Assert.False(result.HasErrors);
		}

		[Theory]
		[InlineData("!")]
		[InlineData("path\\with\\backslashes")]
		[InlineData("path?with=query")]
		public void Validate_InvalidPath_ReturnsSingleError(string path)
		{
			// Arrange
			var validator = new EndpointValidator(_actionValidator.Object);
			var section = new Endpoint
			{
				Path = path,
				Actions = _defaultValidActionList
			};

			// Act
			var result = validator.Validate(section, _name);

			// Assert
			Assert.True(result.HasErrors);
			var error = Assert.Single(result.Errors);
			Assert.Equal(ErrorMessageFormatter.Format("$.path", $"Invalid path '{path}'. Path can only contain A-Z, a-z, 0-9 and slashes (/)."), error);
		}

		[Fact]
		public void Validate_NullActions_ReturnsSingleError()
		{
			// Arrange
			var validator = new EndpointValidator(_actionValidator.Object);
			var section = new Endpoint
			{
				Path = _defaultValidPath,
				Actions = null
			};

			// Act
			var result = validator.Validate(section, _name);

			// Assert
			Assert.True(result.HasErrors);
			var error = Assert.Single(result.Errors);
			Assert.Equal(ErrorMessageFormatter.Format("$.actions", $"Endpoints must have an actions array"), error);
		}

		[Fact]
		public void Validate_EmptyActions_ReturnsSingleError()
		{
			// Arrange
			var validator = new EndpointValidator(_actionValidator.Object);
			var section = new Endpoint
			{
				Path = _defaultValidPath,
				Actions = new List<IEndpointAction>()
			};

			// Act
			var result = validator.Validate(section, _name);

			// Assert
			Assert.True(result.HasErrors);
			var error = Assert.Single(result.Errors);
			Assert.Equal(ErrorMessageFormatter.Format("$.actions", $"Actions array must have at least one item"), error);
		}

		[Fact]
		public void Validate_SingleActionInArray_CallsActionValidator()
		{
			// Arrange
			var validator = new EndpointValidator(_actionValidator.Object);
			var action = new EndpointAction();
			var section = new Endpoint
			{
				Path = _defaultValidPath,
				Actions = new List<IEndpointAction>
				{
					action
				}
			};

			// Act
			var result = validator.Validate(section, _name);

			// Assert
			Assert.False(result.HasErrors);
			_actionValidator.Verify(m => m.Validate(
				action,
				It.Is<SectionName>(s => s.PropertyPath == "$.actions[0]")
			), Times.Once());
		}

		[Fact]
		public void Validate_MultipleActionsInArray_CallsActionValidatorForAllActions()
		{
			// Arrange
			var validator = new EndpointValidator(_actionValidator.Object);
			var action1 = new EndpointAction();
			var action2 = new EndpointAction();
			var action3 = new EndpointAction();
			var section = new Endpoint
			{
				Path = _defaultValidPath,
				Actions = new List<IEndpointAction>
				{
					action1,
					action2,
					action3
				}
			};

			// Act
			var result = validator.Validate(section, _name);

			// Assert
			Assert.False(result.HasErrors);

			_actionValidator.Verify(m => m.Validate(
				action1,
				It.Is<SectionName>(s => s.PropertyPath == "$.actions[0]")
			), Times.Once());

			_actionValidator.Verify(m => m.Validate(
				action2,
				It.Is<SectionName>(s => s.PropertyPath == "$.actions[1]")
			), Times.Once());

			_actionValidator.Verify(m => m.Validate(
				action3,
				It.Is<SectionName>(s => s.PropertyPath == "$.actions[2]")
			), Times.Once());
		}

		[Fact]
		public void Validate_HasEmptyChildEndpointList_ReturnsNoError()
		{
			// Arrange
			var validator = new EndpointValidator(_actionValidator.Object);
			var section = new Endpoint
			{
				Path = _defaultValidPath,
				Actions = _defaultValidActionList,
				Endpoints = new List<IEndpoint>()
			};

			// Act
			var result = validator.Validate(section, _name);

			// Assert
			Assert.False(result.HasErrors);
		}

		[Fact]
		public void Validate_HasChildEndpoints_CallsValidateForChildEndpoints()
		{
			// When an endpoint has child endpoints, Validate method calls
			// itself. No nice way to validate this, so setting up the child
			// endpoints to fail the path validation, and validate that is
			// happening

			// Arrange
			var validator = new EndpointValidator(_actionValidator.Object);
			var section = new Endpoint
			{
				Path = _defaultValidPath,
				Actions = _defaultValidActionList,
				Endpoints = new List<IEndpoint>
				{
					new Endpoint
					{
						Path = _defaultInvalidPath,
						Actions = _defaultValidActionList
					},
					new Endpoint
					{
						Path = _defaultInvalidPath,
						Actions = _defaultValidActionList
					}
				}
			};

			// Act
			var result = validator.Validate(section, _name);

			// Assert
			Assert.True(result.HasErrors);
			Assert.Equal(2, result.Errors.Length);
		}

		[Fact]
		public void Validate_InvalidPathAndActionsInArray_MergesErrors()
		{
			// Arrange
			var validator = new EndpointValidator(_actionValidator.Object);

			var section = new Endpoint
			{
				Path = _defaultInvalidPath,
				Actions = _defaultValidActionList
			};

			var invalidValidationResult = new ValidationResult();
			invalidValidationResult.AddError(new SectionName("$"), "Something broke");
			_actionValidator.Setup(
				m => m.Validate(It.IsAny<EndpointAction>(), It.IsAny<SectionName>())
			).Returns(invalidValidationResult);

			// Act
			var result = validator.Validate(section, _name);

			// Assert
			Assert.True(result.HasErrors);
			Assert.Equal(2, result.Errors.Length);
		}
	}
}