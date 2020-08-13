using System.Text.RegularExpressions;
using Mockasin.Mocks.Endpoints;
using Mockasin.Mocks.Validation.Abstractions;

namespace Mockasin.Mocks.Validation
{
	public class EndpointValidator : IMockSectionValidator<Endpoint>
	{
		private static readonly Regex PathPattern = new Regex("^[A-Za-z0-9/]+$", RegexOptions.Compiled);
		private readonly IMockSectionValidator<EndpointAction> _actionValidator;

		public EndpointValidator(IMockSectionValidator<EndpointAction> actionValidator)
		{
			_actionValidator = actionValidator;
		}

		public ValidationResult Validate(Endpoint section, SectionName sectionName)
		{
			var result = new ValidationResult();

			if (section is null)
			{
				result.AddError(sectionName, "Endpoint is null");
				return result;
			}

			if (string.IsNullOrWhiteSpace(section.Path))
			{
				result.AddError(sectionName.WithProperty("path"), "Endpoint must have a path");
			}
			else if (!PathPattern.IsMatch(section.Path))
			{
				result.AddError(sectionName.WithProperty("path"), $"Invalid path '{section.Path}'. Path can only contain A-Z, a-z, 0-9 and slashes (/).");
			}

			if (section.Actions is null)
			{
				result.AddError(sectionName.WithProperty("actions"), "Endpoints must have an actions array");
			}
			else if (section.Actions.Count < 1)
			{
				result.AddError(sectionName.WithProperty("actions"), "Actions array must have at least one item");
			}
			else
			{
				for (int i = 0; i < section.Actions.Count; i++)
				{
					var action = section.Actions[i];
					var actionValidationResult = _actionValidator.Validate(action, sectionName.WithProperty("actions", i));
					result.Append(actionValidationResult);
				}
			}

			if (section.Endpoints is object)
			{
				for (int i = 0; i < section.Endpoints.Count; i++)
				{
					var endpoint = section.Endpoints[i];
					var actionValidationResult = Validate(endpoint, sectionName.WithProperty("endpoints", i));
					result.Append(actionValidationResult);
				}
			}

			return result;
		}
	}
}