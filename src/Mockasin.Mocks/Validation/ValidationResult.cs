using System.Collections.Generic;

namespace Mockasin.Mocks.Validation
{
	public class ValidationResult
	{
		public bool HasErrors { get => Errors.Count > 0; }
		public List<string> Errors { get; private set; } = new List<string>();
	}
}