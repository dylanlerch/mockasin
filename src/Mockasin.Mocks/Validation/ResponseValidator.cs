using System.Collections.Generic;
using System.Text.RegularExpressions;
using Mockasin.Mocks.Endpoints;
using Mockasin.Mocks.Validation.Abstractions;

namespace Mockasin.Mocks.Validation
{
	public class ResponseValidator : IMockSectionValidator<Response>
	{
		private static readonly Regex AsciiPattern = new Regex("^[\x00-\x7F]+$", RegexOptions.Compiled);

		public ValidationResult Validate(Response section, SectionName sectionName)
		{
			var result = new ValidationResult();

			if (section is null)
			{
				result.AddError(sectionName, "Response is null");
				return result;
			}

			if (section.Headers is object)
			{
				foreach (var (key, value) in section.Headers)
				{
					if (!AsciiPattern.IsMatch(key))
					{
						result.AddError(sectionName.WithProperty("headers", key), $"Invalid header key '{key}'. Headers can only contain ASCII characters.");
					}

					if (!AsciiPattern.IsMatch(value))
					{
						result.AddError(sectionName.WithProperty("headers", key), $"Invalid header value '{value}'. Headers can only contain ASCII characters.");
					}
				}
			}

			// Ensure that only one of jsonBody or stringBody is set
			var setBodyTypes = new List<string>();

			if (section.JsonBody is object)
			{
				setBodyTypes.Add("jsonBody");
			}

			if (section.XmlBody is object)
			{
				setBodyTypes.Add("xmlBody");
			}

			if (section.StringBody is object)
			{
				setBodyTypes.Add("stringBody");
			}

			if (setBodyTypes.Count > 1)
			{
				string formattedBodyList;

				if (setBodyTypes.Count > 2)
				{
					// Create a formatted list of the set bodies
					int lastIndex = setBodyTypes.Count - 1;
					var lastItem = setBodyTypes[lastIndex];
					setBodyTypes.RemoveAt(lastIndex);

					formattedBodyList = $"{string.Join(", ", setBodyTypes)}, and {lastItem}";
				}
				else
				{
					// If it's just two items, don't comma separate
					formattedBodyList = $"{setBodyTypes[0]} and {setBodyTypes[1]}";
				}

				result.AddError(sectionName, $"Only one type of body can be set per response. {formattedBodyList} are set.");
			}

			return result;
		}
	}
}