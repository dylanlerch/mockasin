namespace Mockasin.Mocks.Validation.Abstractions
{
	public interface IMockSectionValidator<T>
	{
		ValidationResult Validate(T section, SectionName sectionName);
	}
}