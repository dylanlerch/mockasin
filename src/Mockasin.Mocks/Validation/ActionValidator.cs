using Mockasin.Mocks.Endpoints;
using Mockasin.Mocks.Validation.Abstractions;

namespace Mockasin.Mocks.Validation
{
	public class ActionValidator : IMockSectionValidator<Action>
	{
		private IMockSectionValidator<Response> _responseValidator;

		public ActionValidator(IMockSectionValidator<Response> responseValidator)
		{
			_responseValidator = responseValidator;
		}

		public ValidationResult Validate(Action section, string sectionLocation = "")
		{
			return new ValidationResult();
		}
	}
}