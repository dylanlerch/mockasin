using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using Mockasin.Mocks.Router;

namespace Mockasin.Web.Controllers
{
	[Controller]
	public class MockController : ControllerBase
	{
		private readonly ILogger<MockController> _logger;
		private readonly IMockRouter _mockRouter;

		public MockController(ILogger<MockController> logger, IMockRouter mockRouter)
		{
			_logger = logger;
			_mockRouter = mockRouter;
		}

		[HttpGet]
		public IActionResult Index()
		{
			var mockResult = _mockRouter.Route(Request.Method, Request.Path.Value);
			AddMockResponseHeaders(mockResult);

			if (mockResult.JsonBody is object)
			{
				return new JsonResult(mockResult.JsonBody)
				{
					StatusCode = mockResult.StatusCode
				};
			}
			else if (mockResult.StringBody is object)
			{
				return new ContentResult
				{
					Content = mockResult.StringBody,
					StatusCode = mockResult.StatusCode
				};
			}
			else
			{
				return new StatusCodeResult(mockResult.StatusCode);
			}
		}

		private void AddMockResponseHeaders(MockResponse response)
		{
			if (response.Headers is object)
			{
				foreach (var header in response.Headers)
				{
					Response.Headers.Add(header.Key, header.Value);
				}
			}
		}
	}
}
