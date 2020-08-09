using System.Collections.Generic;
using Mockasin.Mocks.Configuration;
using Mockasin.Mocks.Models;

namespace Mockasin.Mocks
{
	public class MockRouter : IMockRouter
	{
		private EndpointsRoot _responses = new EndpointsRoot();

		public MockRouter(IMockSettings settings)
		{
			_responses = EndpointsRoot.LoadFromFile(settings.Mock.ConfigurationPath);
		}

		public MockResponse Route(string verb, string route)
		{
			return new MockResponse();
		}
	}
}
