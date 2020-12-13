using Microsoft.Extensions.Logging;
using Mockasin.Mocks.Configuration;
using Mockasin.Mocks.Endpoints;
using Mockasin.Mocks.Validation.Abstractions;

namespace Mockasin.Mocks.Router
{
	public class MockRouter : IMockRouter
	{
		private EndpointsRoot _responses;
		private readonly IMockSettings _settings;
		private readonly IMockSectionValidator<EndpointsRoot> _validator;
		private readonly ILogger<MockRouter> _logger;

		public MockRouter(IMockSettings settings, IMockSectionValidator<EndpointsRoot> validator, ILogger<MockRouter> logger)
		{
			_settings = settings;
			_validator = validator;
			_logger = logger;
		}

		public Response Route(string method, string path)
		{
			// Always reload data on each request while were building and testing things
			_responses = EndpointsRoot.LoadFromFile(_settings.Mock.ConfigurationPath, _validator, _logger);

			if (_responses.Status.IsInvalid)
			{
				// When loading the file, EndpointsRoot catches all errors with
				// loading the file, reading the JSON, or any custom validation
				// that has failed. If it's in an error state, return that error
				// to the caller.
				return ErrorResponse(_responses.Status.ErrorMessage);
			}

			// Otherwise, we know we have a valid endpoint structure. Get the
			// matching endpoint if there is one.
			var response = _responses.GetResponse(method, path);
			if (response is null)
			{
				return NotFoundResponse();
			}

			return response;
		}

		private Response NotFoundResponse()
		{
			return new Response
			{
				StatusCode = 404
			};
		}

		private Response ErrorResponse(string errorMessage)
		{
			return new Response
			{
				StatusCode = 500,
				StringBody = errorMessage
			};
		}
	}
}
