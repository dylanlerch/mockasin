using System.Collections.Generic;

namespace Mockasin.Mocks.Validation
{
	public class ValidationResult
	{
		public bool HasErrors { get => _errors.Count > 0; }
		public string[] Errors { get => _errors.ToArray(); }
		
		private readonly List<string> _errors = new List<string>();

		public void AddError(SectionName sectionName, string errorMessage)
		{
			var fullErrorMessage = $"Error in {sectionName}: {errorMessage}";	
			_errors.Add(fullErrorMessage);
		}

		public void Append(ValidationResult result)
		{
			if (result?.Errors is object)
			{
				_errors.AddRange(result.Errors);
			}
		}
	}
}