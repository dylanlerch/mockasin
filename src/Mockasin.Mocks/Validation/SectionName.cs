using System;

namespace Mockasin.Mocks.Validation
{
	public class SectionName
	{
		public string PropertyPath { get; private set; }

		public SectionName(string propertyPath)
		{
			if (propertyPath is null)
			{
				var x = nameof(propertyPath);
				
				throw new ArgumentNullException(nameof(propertyPath));
			}

			PropertyPath = propertyPath;
		}

		public SectionName WithProperty(string propertyName, int index)
		{
			return WithProperty(propertyName, index.ToString());
		}

		public SectionName WithProperty(string propertyName, string index = null)
		{
			if (propertyName is null)
			{
				throw new ArgumentNullException(nameof(propertyName));
			}

			var indexPortion = index is object ? $"[{index}]" : "";

			return new SectionName($"{PropertyPath}.{propertyName}{indexPortion}");
		}

		public override string ToString()
		{
			return PropertyPath;
		}
	}
}