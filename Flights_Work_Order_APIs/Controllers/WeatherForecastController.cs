using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Flights_Work_Order_APIs.Models;

namespace Flights_Work_Order_APIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Requires JWT authentication
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Get weather forecast data (demo endpoint)
        /// </summary>
        [HttpGet(Name = "GetWeatherForecast")]
        public ActionResult<ApiResponse<IEnumerable<WeatherForecast>>> Get()
        {
            try
            {
                var forecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                })
                .ToArray();

                return Ok(ApiResponse<IEnumerable<WeatherForecast>>.CreateSuccess(forecasts, "Weather forecast retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving weather forecast");
                return StatusCode(500, ApiResponse<IEnumerable<WeatherForecast>>.CreateError("Failed to retrieve weather forecast"));
            }
        }
    }
}
