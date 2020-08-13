using System;
using Mockasin.Mocks.Validation;
using Xunit;

namespace Mockasin.Mocks.Test.Validation
{
	public class SectionNameTests
	{
		[Fact]
		public void New_NullPropertyPath_ThrowsException()
		{
			// Arrange + Act
			var exception = Assert.Throws<ArgumentNullException>(() =>
			{
				new SectionName(null);
			});

			// Assert
			Assert.Equal("Value cannot be null. (Parameter 'propertyPath')", exception.Message);
		}

		[Fact]
		public void New_SomePropertyPath_IsSetAsPropertyPath()
		{
			// Arrange + Act
			var sectionName = new SectionName("somePath");

			// Assert
			Assert.Equal("somePath", sectionName.PropertyPath);
		}

		[Fact]
		public void WithProperty_NullPropertyName_ThrowsException()
		{
			// Arrange
			var sectionName = new SectionName("somePath");

			// Act
			var exception = Assert.Throws<ArgumentNullException>(() =>
			{
				sectionName.WithProperty(null);
			});

			// Assert
			Assert.Equal("Value cannot be null. (Parameter 'propertyName')", exception.Message);
		}

		[Fact]
		public void WithProperty_PropertyNameSet_MergesBasePathWithPropertyName()
		{
			// Arrange
			var sectionName = new SectionName("somePath");

			// Act
			var newSectionName = sectionName.WithProperty("someProperty");

			// Assert
			Assert.Equal("somePath.someProperty", newSectionName.PropertyPath);
		}

		[Fact]
		public void WithProperty_PropertyNameAndStringIndex_MergesBasePathWithPropertyNameAndIndex()
		{
			// Arrange
			var sectionName = new SectionName("somePath");

			// Act
			var newSectionName = sectionName.WithProperty("someProperty", "12");

			// Assert
			Assert.Equal("somePath.someProperty[12]", newSectionName.PropertyPath);
		}

		[Fact]
		public void WithProperty_PropertyNameAndIntegerIndex_MergesBasePathWithPropertyNameAndIndex()
		{
			// Arrange
			var sectionName = new SectionName("somePath");

			// Act
			var newSectionName = sectionName.WithProperty("someProperty", 20);

			// Assert
			Assert.Equal("somePath.someProperty[20]", newSectionName.PropertyPath);
		}

		[Fact]
		public void WithProperty_PropertyNameAndStringIndex_OriginalSectionNameIsNotChanged()
		{
			// Arrange
			var sectionName = new SectionName("somePath");

			// Act
			sectionName.WithProperty("someProperty", "20");

			// Assert
			Assert.Equal("somePath", sectionName.PropertyPath);
		}

		[Fact]
		public void ToString_PropertyNameAndStringIndex_ReturnsFullPath()
		{
			// Arrange
			var sectionName = new SectionName("somePath");
			var newSectionName = sectionName.WithProperty("someProperty", "20");

			// Act
			var toString = newSectionName.ToString();

			// Assert
			Assert.Equal("somePath.someProperty[20]", toString);
		}
	}
}