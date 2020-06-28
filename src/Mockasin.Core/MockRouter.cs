using System.Collections.Generic;
using Mockasin.Core.Responses;

namespace Mockasin.Core
{
	public class MockRouter : IMockRouter
	{
		private MockStructure _responses = new MockStructure();

		public MockResponse Route(string verb, string route)
		{
			return "OK";
		}
	}
}
