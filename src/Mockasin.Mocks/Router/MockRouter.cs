using Microsoft.Extensions.Logging;
using Mockasin.Mocks.Configuration;
using Mockasin.Mocks.Endpoints;
using Mockasin.Mocks.Validation.Abstractions;

namespace Mockasin.Mocks.Router
{
	public class MockRouter : IMockRouter
	{
		private EndpointsRoot _responses;
		private IMockSettings _settings;
		private IMockSectionValidator<EndpointsRoot> _validator;
		private ILogger<MockRouter> _logger;

		public MockRouter(IMockSettings settings, IMockSectionValidator<EndpointsRoot> validator, ILogger<MockRouter> logger)
		{
			_settings = settings;
			_validator = validator;
			_logger = logger;
		}

		public MockResponse Route(string verb, string route)
		{
			// Always reload data on each request while were building and testing things
			_responses = EndpointsRoot.LoadFromFile(_settings.Mock.ConfigurationPath, _validator, _logger);

			if (_responses.IsInvalid)
			{
				// When loading the file, EndpointsRoot catches all errors with
				// loading the file, reading the JSON, or any custom validation
				// that has failed. If it's in an error state, return that error
				// to the caller.
				return new MockResponse
				{
					StatusCode = 500,
					StringBody = _responses.ErrorMessage
				};
			}

			// Otherwise, we know we have a valid endpoint structure.
			return new MockResponse();
		}
	}
}
