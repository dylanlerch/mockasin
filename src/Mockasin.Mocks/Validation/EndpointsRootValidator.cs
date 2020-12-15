using Mockasin.Mocks.Endpoints;
using Mockasin.Mocks.Validation.Abstractions;

namespace Mockasin.Mocks.Validation
{
	public class EndpointsRootValidator : IMockSectionValidator<EndpointsRoot>
	{
		private readonly IMockSectionValidator<Endpoint> _endpointValidator;

		public EndpointsRootValidator(IMockSectionValidator<Endpoint> endpointValidator)
		{
			_endpointValidator = endpointValidator;
		}

		public ValidationResult Validate(EndpointsRoot section, SectionName sectionName)
		{
			var result = new ValidationResult();

			if (section is null)
			{
				result.AddError(sectionName, "Endpoints file is null");
			}
			else if (section.Endpoints is null)
			{
				result.AddError(sectionName.WithProperty("endpoints"), "Endpoints array is null");
			}
			else
			{
				for (int i = 0; i < section.Endpoints.Count; i++)
				{
					var endpoint = section.Endpoints[i];
					var endpointValidationResult = _endpointValidator.Validate(endpoint, sectionName.WithProperty("endpoints", i));
					result.Append(endpointValidationResult);
				}
			}

			return result;
		}
	}
}