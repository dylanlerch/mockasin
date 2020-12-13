using System;
using System.Collections.Generic;
using Mockasin.Mocks.Endpoints;
using Mockasin.Mocks.Validation;
using Mockasin.Mocks.Validation.Abstractions;
using Moq;
using Xunit;

namespace Mockasin.Mocks.Test.Validation
{
	public class EndpointsRootValidatorTests
	{
		private readonly Mock<IMockSectionValidator<IEndpoint>> _endpointValidator = new Mock<IMockSectionValidator<IEndpoint>>();
		private readonly SectionName _name = new SectionName("$");

		[Fact]
		public void Validate_NullSection_ReturnsSingleError()
		{
			// Arrange
			var validator = new EndpointsRootValidator(_endpointValidator.Object);

			// Act
			var result = validator.Validate(null, _name);

			// Assert
			Assert.True(result.HasErrors);
			var error = Assert.Single(result.Errors);
			Assert.Equal(ErrorMessageFormatter.Format("$", "Endpoints file is null"), error);
		}

		[Fact]
		public void Validate_NullEndpointsArray_ReturnsSingleError()
		{
			// Arrange
			var validator = new EndpointsRootValidator(_endpointValidator.Object);
			var section = new EndpointsRoot
			{
				Endpoints = null
			};

			// Act
			var result = validator.Validate(section, _name);

			// Assert
			Assert.True(result.HasErrors);
			var error = Assert.Single(result.Errors);
			Assert.Equal(ErrorMessageFormatter.Format("$.endpoints", "Endpoints array is null"), error);
		}

		[Fact]
		public void Validate_EmptyEndpointsArray_ReturnsSuccessWithoutCallingEndpointValidator()
		{
			// Arrange
			var validator = new EndpointsRootValidator(_endpointValidator.Object);
			var section = new EndpointsRoot
			{
				Endpoints = new List<IEndpoint>()
			};

			// Act
			var result = validator.Validate(section, _name);

			// Assert
			Assert.False(result.HasErrors);
			_endpointValidator.Verify(m => m.Validate(
				It.IsAny<Endpoint>(),
				It.IsAny<SectionName>()
			), Times.Never());
		}

		[Fact]
		public void Validate_SingleEndpointInArray_CallsEndpointValidator()
		{
			// Arrange
			var validator = new EndpointsRootValidator(_endpointValidator.Object);
			var endpoint = new Endpoint();
			var section = new EndpointsRoot
			{
				Endpoints = new List<IEndpoint>
				{
					endpoint
				}
			};

			// Act
			var result = validator.Validate(section, _name);

			// Assert
			Assert.False(result.HasErrors);
			_endpointValidator.Verify(m => m.Validate(
				endpoint,
				It.Is<SectionName>(s => s.PropertyPath == "$.endpoints[0]")
			), Times.Once());
		}

		[Fact]
		public void Validate_MultipleEndpointsInArray_CallsEndpointValidatorForAllEndpoints()
		{
			// Arrange
			var validator = new EndpointsRootValidator(_endpointValidator.Object);
			var endpoint1 = new Endpoint();
			var endpoint2 = new Endpoint();
			var endpoint3 = new Endpoint();
			var section = new EndpointsRoot
			{
				Endpoints = new List<IEndpoint>
				{
					endpoint1,
					endpoint2,
					endpoint3
				}
			};

			// Act
			var result = validator.Validate(section, _name);

			// Assert
			Assert.False(result.HasErrors);

			_endpointValidator.Verify(m => m.Validate(
				endpoint1,
				It.Is<SectionName>(s => s.PropertyPath == "$.endpoints[0]")
			), Times.Once());

			_endpointValidator.Verify(m => m.Validate(
				endpoint2,
				It.Is<SectionName>(s => s.PropertyPath == "$.endpoints[1]")
			), Times.Once());

			_endpointValidator.Verify(m => m.Validate(
				endpoint3,
				It.Is<SectionName>(s => s.PropertyPath == "$.endpoints[2]")
			), Times.Once());
		}
	}
}