using Mockasin.Mocks.Endpoints;
using Mockasin.Mocks.Validation.Abstractions;

namespace Mockasin.Mocks.Validation
{
	public class ResponseValidator : IMockSectionValidator<Response>
	{
		public ValidationResult Validate(Response section, SectionName sectionName)
		{
			return new ValidationResult();
		}
	}
}