using Microsoft.Extensions.Logging;
using Mockasin.Mocks.Endpoints;
using Mockasin.Mocks.Store;
using Mockasin.Services;

namespace Mockasin.Mocks.Router
{
	public class MockRouter : IMockRouter
	{
		private readonly IMockStore _store;
		private readonly IRandomService _random;
		private readonly ILogger<MockRouter> _logger;

		public MockRouter(IMockStore store, IRandomService random, ILogger<MockRouter> logger)
		{
			_store = store;
			_random = random;
			_logger = logger;
		}

		public Response Route(string method, string path)
		{
			var endpoints = _store.GetEndpointsRoot();
			return endpoints.GetResponse(method, path, _random);
		}
	}
}
