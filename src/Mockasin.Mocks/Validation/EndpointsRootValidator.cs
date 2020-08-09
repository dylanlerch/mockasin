using Mockasin.Mocks.Endpoints;
using Mockasin.Mocks.Validation.Abstractions;

namespace Mockasin.Mocks.Validation
{
	public class EndpointsRootValidator : IMockSectionValidator<EndpointsRoot>
	{
		private IMockSectionValidator<Endpoint> _endpointValidator;

		public EndpointsRootValidator(IMockSectionValidator<Endpoint> endpointValidator)
		{
			_endpointValidator = endpointValidator;
		}

		public ValidationResult Validate(EndpointsRoot section, string sectionLocation = "")
		{
			return new ValidationResult();
		}
	}
}