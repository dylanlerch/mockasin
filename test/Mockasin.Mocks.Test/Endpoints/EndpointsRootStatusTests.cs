using System.Collections.Generic;
using Mockasin.Mocks.Endpoints;
using Mockasin.Mocks.Test.TestData;
using Mockasin.Mocks.Validation;
using Mockasin.Mocks.Validation.Abstractions;
using Moq;
using Xunit;

namespace Mockasin.Mocks.Test.Endpoints
{
	public class EndpointsRootStatusTests
	{
		[Fact]
		public void IsInvalid_ErrorMessageNull_ReturnsFalse()
		{
			// Arrange
			var status = new EndpointsRootStatus
			{
				ErrorMessage = null
			};

			// Assert
			Assert.False(status.IsInvalid);
		}

		[Fact]
		public void IsInvalid_ErrorMessageSet_ReturnsFalse()
		{
			// Arrange
			var status = new EndpointsRootStatus
			{
				ErrorMessage = ""
			};

			// Assert
			Assert.True(status.IsInvalid);
		}
	}
}