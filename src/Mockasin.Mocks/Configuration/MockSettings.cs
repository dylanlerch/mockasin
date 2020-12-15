namespace Mockasin.Mocks.Configuration
{
	public class MockSettings
	{
		public MockSettings() { }
		public MockSettings(string configurationPath)
		{
			this.ConfigurationPath = configurationPath;
		}

		public string ConfigurationPath { get; private set; }
	}
}