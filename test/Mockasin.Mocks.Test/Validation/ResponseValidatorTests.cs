using System;
using System.Collections.Generic;
using System.Text.Json;
using Mockasin.Mocks.Endpoints;
using Mockasin.Mocks.Validation;
using Xunit;

namespace Mockasin.Mocks.Test.Validation
{
	public class ResponseValidatorTests
	{
		private readonly SectionName _name = new SectionName("$");

		[Fact]
		public void Validate_NullSection_ReturnsSingleError()
		{
			// Arrange
			var validator = new ResponseValidator();

			// Act
			var result = validator.Validate(null, _name);

			// Assert
			Assert.True(result.HasErrors);
			var error = Assert.Single(result.Errors);
			Assert.Equal(ErrorMessageFormatter.Format("$", "Response is null"), error);
		}

		[Fact]
		public void Validate_NullHeaders_ReturnsNoError()
		{
			// Arrange
			var validator = new ResponseValidator();
			var section = new Response
			{
				Headers = null
			};

			// Act
			var result = validator.Validate(section, _name);

			// Assert
			Assert.False(result.HasErrors);
		}

		[Fact]
		public void Validate_EmptyHeaderDictionary_ReturnsNoError()
		{
			// Arrange
			var validator = new ResponseValidator();
			var section = new Response
			{
				Headers = new Dictionary<string, string>()
			};

			// Act
			var result = validator.Validate(section, _name);

			// Assert
			Assert.False(result.HasErrors);
		}

		[Theory]
		[InlineData("à", "valid")]
		[InlineData("∆", "valid")]
		public void Validate_SingleNonAsciiHeaderKey_ReturnsSingleError(string key, string value)
		{
			// Arrange
			var validator = new ResponseValidator();
			var section = new Response
			{
				Headers = new Dictionary<string, string>
				{
					{ key, value }
				}
			};

			// Act
			var result = validator.Validate(section, _name);

			// Assert
			Assert.True(result.HasErrors);
			var error = Assert.Single(result.Errors);
			Assert.Equal(ErrorMessageFormatter.Format($"$.headers[{key}]", $"Invalid header key '{key}'. Headers can only contain ASCII characters."), error);
		}

		[Theory]
		[InlineData("valid", "à")]
		[InlineData("valid", "∆")]
		public void Validate_SingleNonAsciiHeaderValue_ReturnsSingleError(string key, string value)
		{
			// Arrange
			var validator = new ResponseValidator();
			var section = new Response
			{
				Headers = new Dictionary<string, string>
				{
					{ key, value }
				}
			};

			// Act
			var result = validator.Validate(section, _name);

			// Assert
			Assert.True(result.HasErrors);
			var error = Assert.Single(result.Errors);
			Assert.Equal(ErrorMessageFormatter.Format($"$.headers[{key}]", $"Invalid header value '{value}'. Headers can only contain ASCII characters."), error);
		}


		[Theory]
		[InlineData("∆", "à")]
		[InlineData("à", "∆")]
		[InlineData("∆∆∆∆∆∆∆", "∆∆∆∆∆∆∆")]
		public void Validate_SingleNonAsciiHeaderKeyAndValue_ReturnsSingleError(string key, string value)
		{
			// Arrange
			var validator = new ResponseValidator();
			var section = new Response
			{
				Headers = new Dictionary<string, string>
				{
					{ key, value }
				}
			};

			// Act
			var result = validator.Validate(section, _name);

			// Assert
			Assert.True(result.HasErrors);
			Assert.Equal(2, result.Errors.Length);

			Assert.Equal(ErrorMessageFormatter.Format($"$.headers[{key}]", $"Invalid header key '{key}'. Headers can only contain ASCII characters."), result.Errors[0]);
			Assert.Equal(ErrorMessageFormatter.Format($"$.headers[{key}]", $"Invalid header value '{value}'. Headers can only contain ASCII characters."), result.Errors[1]);
		}

		[Fact]
		public void Validate_MultipleInvalidHeaderKeysAndValues_ReturnsMultipleError()
		{
			// Arrange
			var validator = new ResponseValidator();
			var section = new Response
			{
				Headers = new Dictionary<string, string>
				{
					{ "Valid", "Valid" },
					{ "∆", "∆" },
					{ "AnotherValid", "Valid" },
					{ "∆∆∆∆", "∆" },
					{ "æææææ", "∆" }
				}
			};

			// Act
			var result = validator.Validate(section, _name);

			// Assert
			Assert.True(result.HasErrors);
			Assert.Equal(6, result.Errors.Length);
		}

		[Fact]
		public void Validate_ValidHeaders_ReturnsNoError()
		{
			// Arrange
			var validator = new ResponseValidator();
			var section = new Response
			{
				Headers = new Dictionary<string, string>
				{
					{ "Valid", "Valid" },
					{ "Test", "Valid" },
					{ "Authentication", "398429348923849239489238" },
					{ "Something-With-Dashes", "Value-With-Dashes" }
				}
			};

			// Act
			var result = validator.Validate(section, _name);

			// Assert
			Assert.False(result.HasErrors);
		}

		[Fact]
		public void Validate_OnlyJsonBodySet_ReturnsNoError()
		{
			// Arrange
			var validator = new ResponseValidator();
			var document = JsonDocument.Parse("{}");
			var section = new Response
			{
				JsonBody = document.RootElement
			};

			// Act
			var result = validator.Validate(section, _name);

			// Assert
			Assert.False(result.HasErrors);
		}

		[Fact]
		public void Validate_OnlyXmlBodySet_ReturnsNoError()
		{
			// Arrange
			var validator = new ResponseValidator();
			var section = new Response
			{
				XmlBody = "<year>1999</year>"
			};

			// Act
			var result = validator.Validate(section, _name);

			// Assert
			Assert.False(result.HasErrors);
		}

		[Fact]
		public void Validate_OnlyStringBodySet_ReturnsNoError()
		{
			// Arrange
			var validator = new ResponseValidator();
			var section = new Response
			{
				XmlBody = "Plain text! Hooray!"
			};

			// Act
			var result = validator.Validate(section, _name);

			// Assert
			Assert.False(result.HasErrors);
		}

		[Theory]
		[InlineData(true, true, true, "jsonBody, xmlBody, and stringBody")]
		[InlineData(true, true, false, "jsonBody and xmlBody")]
		[InlineData(true, false, true, "jsonBody and stringBody")]
		[InlineData(false, true, true, "xmlBody and stringBody")]
		public void Validate_MultipleBodiesSet_ReturnsSingleError(bool jsonBody, bool xmlBody, bool stringBody, string bodyList)
		{
			// Arrange
			var validator = new ResponseValidator();

			var json = jsonBody ? new JsonElement?(JsonDocument.Parse("{}").RootElement) : null;
			string xml = xmlBody ? "<year>1999</year>" : null;
			string str = stringBody ? "Plain text! Hooray!" : null;

			var section = new Response
			{
				JsonBody = json,
				XmlBody = xml,
				StringBody = str
			};

			// Act
			var result = validator.Validate(section, _name);

			// Assert
			Assert.True(result.HasErrors);
			var error = Assert.Single(result.Errors);
			Assert.Equal(ErrorMessageFormatter.Format($"$", $"Only one type of body can be set per response. {bodyList} are set."), error);
		}
	}
}