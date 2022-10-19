using Microsoft.AspNetCore.Mvc;

namespace Idempotency.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromHeader(Name = "X-Idempotency-Key")]string h)
        {
            return Ok(new WeatherForecast());
        }
    }
}
