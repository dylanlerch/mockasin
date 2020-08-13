using Mockasin.Mocks.Endpoints;
using Mockasin.Mocks.Validation.Abstractions;

namespace Mockasin.Mocks.Validation
{
	public class ActionValidator : IMockSectionValidator<Action>
	{
		private readonly IMockSectionValidator<Response> _responseValidator;

		public ActionValidator(IMockSectionValidator<Response> responseValidator)
		{
			_responseValidator = responseValidator;
		}

		public ValidationResult Validate(Action section, SectionName sectionName)
		{
			return new ValidationResult();
		}
	}
}