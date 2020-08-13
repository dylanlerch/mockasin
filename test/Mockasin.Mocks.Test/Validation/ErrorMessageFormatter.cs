namespace Mockasin.Mocks.Test.Validation
{
	/// <summary>
	/// Generates formatted validation error messages.
	/// 
	/// I don't normally like to make tests too 'smart', but if I ever decide to
	/// change the format of the message that's returned from a validation error
	/// message, I really don't want to have to change all those assertions by
	/// hand.
	/// </summary>
	public static class ErrorMessageFormatter
	{
		private const string ErrorMessageFormat = "Error in {0}: {1}";
		
		/// <summary>
		/// Returns a formatted error message
		/// </summary>
		/// <param name="section">Section portion of the error message</param>
		/// <param name="message">Message portion of the error message</param>
		/// <returns></returns>
		public static string Format(string section, string message)
		{
			return string.Format(ErrorMessageFormat, section, message);
		}
	}
}