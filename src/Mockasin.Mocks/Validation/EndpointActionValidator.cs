using System;
using System.Text.RegularExpressions;
using Mockasin.Mocks.Endpoints;
using Mockasin.Mocks.Validation.Abstractions;

namespace Mockasin.Mocks.Validation
{
	public class EndpointActionValidator : IMockSectionValidator<IEndpointAction>
	{
		private static readonly string[] AllowedModes = new string[]
		{
			EndpointActionMode.Single,
			EndpointActionMode.Random,
			EndpointActionMode.Intercept,
		};

		private static readonly Regex MethodPattern = new Regex("^[A-Za-z]+$", RegexOptions.Compiled);

		private readonly IMockSectionValidator<Response> _responseValidator;

		public EndpointActionValidator(IMockSectionValidator<Response> responseValidator)
		{
			_responseValidator = responseValidator;
		}

		public ValidationResult Validate(IEndpointAction section, SectionName sectionName)
		{
			var result = new ValidationResult();

			if (section is null)
			{
				result.AddError(sectionName, "Action is null");
				return result;
			}

			if (section.Method is object)
			{
				if (!MethodPattern.IsMatch(section.Method))
				{
					result.AddError(sectionName.WithProperty("method"), $"Invalid method '{section.Method}'. Method can only contain A-Z, a-z.");
				}
			}

			if (section.Mode is object)
			{
				// Mode isn't required, but if it's set it needs a valid value
				if (Array.IndexOf(AllowedModes, section.Mode.ToUpperInvariant()) < 0)
				{
					// IndexOf returns -1 if there is no match found. Using this to
					// determine if the mode is in the list of allowed modes.
					result.AddError(sectionName.WithProperty("mode"), $"Invalid mode '{section.Mode}'. Mode must be one of {string.Join(", ", AllowedModes)}.");
				}
			}

			if (section.Responses is null)
			{
				result.AddError(sectionName.WithProperty("responses"), "Action must have a responses array");
			}
			else if (section.Responses.Count < 1)
			{
				result.AddError(sectionName.WithProperty("responses"), "Responses array must have at least one item");
			}
			else
			{
				for (int i = 0; i < section.Responses.Count; i++)
				{
					var action = section.Responses[i];
					var actionValidationResult = _responseValidator.Validate(action, sectionName.WithProperty("responses", i));
					result.Append(actionValidationResult);
				}
			}

			return result;
		}
	}
}