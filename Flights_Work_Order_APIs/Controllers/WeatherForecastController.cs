using Flights_Work_Order_APIs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flights_Work_Order_APIs.Controllers
{
    /// <summary>
    /// Weather forecast controller demonstrating protected endpoints
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Require authentication for all endpoints
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
        /// Gets weather forecast data (requires authentication)
        /// </summary>
        /// <returns>Weather forecast data wrapped in generic API response</returns>
        [HttpGet(Name = "GetWeatherForecast")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<WeatherForecast>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        public ActionResult<ApiResponse<IEnumerable<WeatherForecast>>> Get()
        {
            var forecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();

            _logger.LogInformation("Weather forecast requested by user: {Username}", User.Identity?.Name);

            return Ok(ApiResponse<IEnumerable<WeatherForecast>>.SuccessResponse(
                forecasts, 
                "Weather forecast retrieved successfully"));
        }
    }
}
