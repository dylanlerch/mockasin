using System.Collections.Generic;
using Mockasin.Mocks.Endpoints;
using Mockasin.Mocks.Validation;
using Mockasin.Mocks.Validation.Abstractions;
using Moq;
using Xunit;

namespace Mockasin.Mocks.Test.Validation
{
	public class EndpointActionValidatorTests
	{
		private readonly Mock<IMockSectionValidator<Response>> _responseValidator = new Mock<IMockSectionValidator<Response>>();
		private readonly SectionName _name = new SectionName("$");
		
		private readonly string _defaultInvalidVerb = "!!INVALID";
		private readonly string _defaultInvalidMode = "NotAMode";
		private readonly List<Response> _defaultValidResponses = new List<Response>
		{
			new Response()
		};

		[Fact]
		public void Validate_NullSection_ReturnsSingleError()
		{
			// Arrange
			var validator = new EndpointActionValidator(_responseValidator.Object);

			// Act
			var result = validator.Validate(null, _name);

			// Assert
			Assert.True(result.HasErrors);
			var error = Assert.Single(result.Errors);
			Assert.Equal(ErrorMessageFormatter.Format("$", "Action is null"), error);
		}

		[Fact]
		public void Validate_DefaultValues_ReturnsNoError()
		{
			// Arrange
			var validator = new EndpointActionValidator(_responseValidator.Object);
			var section = new EndpointAction
			{
				Responses = _defaultValidResponses
			};

			// Act
			var result = validator.Validate(section, _name);

			// Assert
			Assert.False(result.HasErrors);
		}

		[Fact]
		public void Validate_AllAllowedNullValuesNull_ReturnsNoError()
		{
			// Arrange
			var validator = new EndpointActionValidator(_responseValidator.Object);
			var section = new EndpointAction
			{
				Mode = null,
				Verb = null,
				Responses = _defaultValidResponses
			};

			// Act
			var result = validator.Validate(section, _name);

			// Assert
			Assert.False(result.HasErrors);
		}

		[Theory]
		[MemberData(nameof(ValidationData.Whitespace), MemberType = typeof(ValidationData))]
		public void Validate_NullAndWhitespaceVerb_ReturnsSingleError(string verb)
		{
			// Arrange
			var validator = new EndpointActionValidator(_responseValidator.Object);
			var section = new EndpointAction
			{
				Verb = verb,
				Responses = _defaultValidResponses
			};

			// Act
			var result = validator.Validate(section, _name);

			// Assert
			Assert.True(result.HasErrors);
			var error = Assert.Single(result.Errors);
			Assert.Equal(ErrorMessageFormatter.Format("$.verb", $"Invalid verb '{verb}'. Verb can only contain A-Z, a-z."), error);
		}

		[Theory]
		[InlineData("Number12345")]
		[InlineData("***##Symbols!!!")]
		public void Validate_InvalidVerb_ReturnsSingleError(string verb)
		{
			// Arrange
			var validator = new EndpointActionValidator(_responseValidator.Object);
			var section = new EndpointAction
			{
				Verb = verb,
				Responses = _defaultValidResponses
			};

			// Act
			var result = validator.Validate(section, _name);

			// Assert
			Assert.True(result.HasErrors);
			var error = Assert.Single(result.Errors);
			Assert.Equal(ErrorMessageFormatter.Format("$.verb", $"Invalid verb '{verb}'. Verb can only contain A-Z, a-z."), error);
		}

		[Theory]
		[InlineData("ANY")]
		[InlineData("GET")]
		[InlineData("post")]
		[InlineData("PuT")]
		[InlineData("SomethingMadeUp")]
		public void Validate_ValidVerb_ReturnsNoError(string verb)
		{
			// Arrange
			var validator = new EndpointActionValidator(_responseValidator.Object);
			var section = new EndpointAction
			{
				Verb = verb,
				Responses = _defaultValidResponses
			};

			// Act
			var result = validator.Validate(section, _name);

			// Assert
			Assert.False(result.HasErrors);
		}

		[Theory]
		[MemberData(nameof(ValidationData.Whitespace), MemberType = typeof(ValidationData))]
		public void Validate_NullAndWhitespaceMode_ReturnsSingleError(string mode)
		{
			// Arrange
			var validator = new EndpointActionValidator(_responseValidator.Object);
			var section = new EndpointAction
			{
				Mode = mode,
				Responses = _defaultValidResponses
			};

			// Act
			var result = validator.Validate(section, _name);

			// Assert
			Assert.True(result.HasErrors);
			var error = Assert.Single(result.Errors);
			Assert.Equal(ErrorMessageFormatter.Format("$.mode", $"Invalid mode '{mode}'. Mode must be one of SINGLE, RANDOM, INTERCEPT."), error);
		}

		[Theory]
		[InlineData("A")]
		[InlineData("AAAAAAAAAAAAAAAAA")]
		[InlineData("SOMETHINGOUTOFRANGE")]
		public void Validate_InvalidMode_ReturnsSingleError(string mode)
		{
			// Arrange
			var validator = new EndpointActionValidator(_responseValidator.Object);
			var section = new EndpointAction
			{
				Mode = mode,
				Responses = _defaultValidResponses
			};

			// Act
			var result = validator.Validate(section, _name);

			// Assert
			Assert.True(result.HasErrors);
			var error = Assert.Single(result.Errors);
			Assert.Equal(ErrorMessageFormatter.Format("$.mode", $"Invalid mode '{mode}'. Mode must be one of SINGLE, RANDOM, INTERCEPT."), error);
		}

		[Theory]
		[InlineData("SINGLE")]
		[InlineData("single")]
		[InlineData("RANDOM")]
		[InlineData("Random")]
		[InlineData("INTERCEPT")]
		[InlineData("InTeRcEpT")]
		public void Validate_ValidMode_ReturnsSingleError(string mode)
		{
			// Arrange
			var validator = new EndpointActionValidator(_responseValidator.Object);
			var section = new EndpointAction
			{
				Mode = mode,
				Responses = _defaultValidResponses
			};

			// Act
			var result = validator.Validate(section, _name);

			// Assert
			Assert.False(result.HasErrors);
		}

		[Fact]
		public void Validate_NullResponses_ReturnsSingleError()
		{
			// Arrange
			var validator = new EndpointActionValidator(_responseValidator.Object);
			var section = new EndpointAction
			{
				Responses = null
			};

			// Act
			var result = validator.Validate(section, _name);

			// Assert
			Assert.True(result.HasErrors);
			var error = Assert.Single(result.Errors);
			Assert.Equal(ErrorMessageFormatter.Format("$.responses", $"Action must have a responses array"), error);
		}

		[Fact]
		public void Validate_EmptyActions_ReturnsSingleError()
		{
			// Arrange
			var validator = new EndpointActionValidator(_responseValidator.Object);
			var section = new EndpointAction
			{
				Responses = new List<Response>()
			};

			// Act
			var result = validator.Validate(section, _name);

			// Assert
			Assert.True(result.HasErrors);
			var error = Assert.Single(result.Errors);
			Assert.Equal(ErrorMessageFormatter.Format("$.responses", $"Responses array must have at least one item"), error);
		}

		[Fact]
		public void Validate_SingleResponseInArray_CallsResponseValidator()
		{
			// Arrange
			var validator = new EndpointActionValidator(_responseValidator.Object);
			var response = new Response();
			var section = new EndpointAction
			{
				Responses = new List<Response>
				{
					response
				}
			};

			// Act
			var result = validator.Validate(section, _name);

			// Assert
			Assert.False(result.HasErrors);
			_responseValidator.Verify(m => m.Validate(
				response,
				It.Is<SectionName>(s => s.PropertyPath == "$.responses[0]")
			), Times.Once());
		}

		[Fact]
		public void Validate_MultipleResponsesInArray_CallsResponseValidatorForAllReponses()
		{
			// Arrange
			var validator = new EndpointActionValidator(_responseValidator.Object);
			var response1 = new Response();
			var response2 = new Response();
			var response3 = new Response();
			var section = new EndpointAction
			{
				Responses = new List<Response>
				{
					response1,
					response2,
					response3
				}
			};

			// Act
			var result = validator.Validate(section, _name);

			// Assert
			Assert.False(result.HasErrors);

			_responseValidator.Verify(m => m.Validate(
				response1,
				It.Is<SectionName>(s => s.PropertyPath == "$.responses[0]")
			), Times.Once());

			_responseValidator.Verify(m => m.Validate(
				response2,
				It.Is<SectionName>(s => s.PropertyPath == "$.responses[1]")
			), Times.Once());

			_responseValidator.Verify(m => m.Validate(
				response3,
				It.Is<SectionName>(s => s.PropertyPath == "$.responses[2]")
			), Times.Once());
		}

		[Fact]
		public void Validate_InvalidVerbAndModeAndActions_MergesErrors()
		{
			// Arrange
			var validator = new EndpointActionValidator(_responseValidator.Object);
			var section = new EndpointAction
			{
				Verb = _defaultInvalidVerb,
				Mode = _defaultInvalidMode,
				Responses = _defaultValidResponses
			};

			var invalidValidationResult = new ValidationResult();
			invalidValidationResult.AddError(new SectionName("$"), "Something broke");
			_responseValidator.Setup(
				m => m.Validate(It.IsAny<Response>(), It.IsAny<SectionName>())
			).Returns(invalidValidationResult);

			// Act
			var result = validator.Validate(section, _name);

			// Assert
			Assert.True(result.HasErrors);
			Assert.Equal(3, result.Errors.Length);
		}
	}
}