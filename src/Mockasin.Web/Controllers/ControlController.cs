using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Mockasin.Web.Controllers
{
	[ApiController]
	[Route("_control")]
	public class ControlController : ControllerBase
	{
		private readonly ILogger<ControlController> _logger;

		public ControlController(ILogger<ControlController> logger)
		{
			_logger = logger;
		}

		[HttpGet]
		public IActionResult Index()
		{
			_logger.LogInformation($"CONTROL");
			return Ok();
		}
	}
}
