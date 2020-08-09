using Microsoft.AspNetCore.Mvc;
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
			
			var result = new ObjectResult(mockResult);
			result.StatusCode = mockResult.StatusCode;

			return result;
		}
	}
}
