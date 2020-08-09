using Mockasin.Mocks.Endpoints;
using Mockasin.Mocks.Validation.Abstractions;

namespace Mockasin.Mocks.Validation
{
	public class ResponseValidator : IMockSectionValidator<Response>
	{
		public ValidationResult Validate(Response section, string sectionLocation = "")
		{
			return new ValidationResult();
		}
	}
}