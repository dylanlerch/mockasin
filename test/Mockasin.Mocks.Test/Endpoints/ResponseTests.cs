using Mockasin.Mocks.Endpoints;
using Xunit;

namespace Mockasin.Mocks.Test.Endpoints
{
	public class ResponseTests
	{
		[Fact]
		public void Constructor_Defaults_AreSet()
		{
			// Arrange + Act
			var response = new Response();

			// Assert
			Assert.Equal(200, response.StatusCode);
			Assert.Equal(1, response.RandomWeight);
			Assert.Empty(response.Headers);
		}

		[Fact]
		public void Execute_Always_ReturnsNull()
		{
			// Arrange + Act
			var response = new Response();

			// Act
			var executedResponse = response.Execute();

			// Assert
			Assert.Null(executedResponse);
		}
	}
}