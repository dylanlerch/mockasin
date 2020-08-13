using Mockasin.Mocks.Validation;
using Xunit;

namespace Mockasin.Mocks.Test.Validation
{
	public class ValidationResultTests
	{
		[Fact]
		public void New_DefaultConstructor_HasNoErrors()
		{
			// Arrange + Act
			var result = new ValidationResult();

			// Assert
			Assert.False(result.HasErrors);
			Assert.Empty(result.Errors);
		}

		[Fact]
		public void AddError_AddSingleError_ContainsErrors()
		{
			// Arrange
			var result = new ValidationResult();

			// Act
			result.AddError(new SectionName(""), "");

			// Assert
			Assert.True(result.HasErrors);
			Assert.Single(result.Errors);
		}

		[Fact]
		public void AddError_AddSingleError_FormatsCorrectly()
		{
			// Arrange
			var result = new ValidationResult();

			// Act
			result.AddError(new SectionName("sectionName"), "Something broke");

			// Assert
			var error = Assert.Single(result.Errors);
			Assert.Equal(ErrorMessageFormatter.Format("sectionName", "Something broke"), error);
		}

		[Fact]
		public void AddError_AddManyErrors_ContainsErrors()
		{
			// Arrange
			var result = new ValidationResult();

			// Act
			result.AddError(new SectionName(""), "");
			result.AddError(new SectionName(""), "");
			result.AddError(new SectionName(""), "");

			// Assert
			Assert.True(result.HasErrors);
			Assert.Equal(3, result.Errors.Length);
		}

		[Fact]
		public void Append_TwoListsOfManyErrors_MergesInOrder()
		{
			// Arrange
			var firstResults = new ValidationResult();
			firstResults.AddError(new SectionName("1"), "1");
			firstResults.AddError(new SectionName("2"), "2");

			var secondResults = new ValidationResult();
			secondResults.AddError(new SectionName("3"), "3");
			secondResults.AddError(new SectionName("4"), "4");

			// Act
			firstResults.Append(secondResults);

			// Assert
			Assert.Equal(4, firstResults.Errors.Length);
			Assert.Equal("Error in 1: 1", firstResults.Errors[0]);
			Assert.Equal("Error in 2: 2", firstResults.Errors[1]);
			Assert.Equal("Error in 3: 3", firstResults.Errors[2]);
			Assert.Equal("Error in 4: 4", firstResults.Errors[3]);

			Assert.Equal(2, secondResults.Errors.Length); // Should only append to called ValidationResult
		}

		[Fact]
		public void Append_NullResults_DoesNothing()
		{
			// Arrange
			var firstResults = new ValidationResult();

			// Act
			firstResults.Append(null);

			// Assert
			Assert.Empty(firstResults.Errors);
		}
	}
}