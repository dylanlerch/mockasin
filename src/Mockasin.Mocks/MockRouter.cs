using System.Collections.Generic;
using Mockasin.Mocks.Models;

namespace Mockasin.Mocks
{
	public class MockRouter : IMockRouter
	{
		private EndpointsRoot _responses = new EndpointsRoot();

		public MockRouter(string path)
		{
			_responses = EndpointsRoot.LoadFromFile(path);
		}

		public MockResponse Route(string verb, string route)
		{
			return new MockResponse();
		}
	}
}
