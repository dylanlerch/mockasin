using Mockasin.Mocks.Endpoints;
using Mockasin.Mocks.Validation.Abstractions;

namespace Mockasin.Mocks.Validation
{
	public class EndpointValidator : IMockSectionValidator<Endpoint>
	{
		private IMockSectionValidator<Action> _actionValidator;

		public EndpointValidator(IMockSectionValidator<Action> actionValidator)
		{
			_actionValidator = actionValidator;
		}

		public ValidationResult Validate(Endpoint section, string sectionLocation = "")
		{
			return new ValidationResult();
		}
	}
}