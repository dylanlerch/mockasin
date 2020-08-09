using Microsoft.Extensions.Configuration;
using Mockasin.Mocks.Configuration;

namespace Mockasin.Web.Configuration
{
	public class Settings : ISettings
	{
		public Settings(IConfiguration configuration)
		{
			// Bind all configuration, including those with private setters.
			// 
			// All the properties have private setters so it's impossible to
			// accidentally write to any of the setting values.
			configuration.Bind(this, options => options.BindNonPublicProperties = true);
		}

		public MockSettings Mock { get; private set; }
	}
}