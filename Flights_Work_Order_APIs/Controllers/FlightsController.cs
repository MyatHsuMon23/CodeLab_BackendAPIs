using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Flights_Work_Order_APIs.Data;
using Flights_Work_Order_APIs.Models;
using Flights_Work_Order_APIs.Services;
using System.Text.Json;

namespace Flights_Work_Order_APIs.Controllers
{
    /// <summary>
    /// Flight management controller
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Requires JWT authentication
    public class FlightsController : ControllerBase
    {
        private readonly FlightDbContext _context;
        private readonly IFlightCommandService _commandService;
        private readonly ILogger<FlightsController> _logger;

        public FlightsController(
            FlightDbContext context,
            IFlightCommandService commandService,
            ILogger<FlightsController> logger)
        {
            _context = context;
            _commandService = commandService;
            _logger = logger;
        }

        /// <summary>
        /// Get all flights with optional filtering and sorting
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<Flight>>>> GetFlights(
            [FromQuery] string? flightNumber = null,
            [FromQuery] string? sortBy = "FlightNumber",
            [FromQuery] bool sortDescending = false)
        {
            try
            {
                var query = _context.Flights.AsQueryable();

                // Apply filtering
                if (!string.IsNullOrEmpty(flightNumber))
                {
                    query = query.Where(f => f.FlightNumber.Contains(flightNumber));
                }

                // Apply sorting
                switch (sortBy?.ToLower())
                {
                    case "flightnumber":
                        query = sortDescending ? query.OrderByDescending(f => f.FlightNumber) : query.OrderBy(f => f.FlightNumber);
                        break;
                    case "scheduledarrivaltime":
                    case "scheduledarrivaltimeutc":
                        query = sortDescending ? query.OrderByDescending(f => f.ScheduledArrivalTimeUtc) : query.OrderBy(f => f.ScheduledArrivalTimeUtc);
                        break;
                    case "originairport":
                        query = sortDescending ? query.OrderByDescending(f => f.OriginAirport) : query.OrderBy(f => f.OriginAirport);
                        break;
                    case "destinationairport":
                        query = sortDescending ? query.OrderByDescending(f => f.DestinationAirport) : query.OrderBy(f => f.DestinationAirport);
                        break;
                    default:
                        query = query.OrderBy(f => f.FlightNumber);
                        break;
                }

                var flights = await query.ToListAsync();

                return Ok(ApiResponse<IEnumerable<Flight>>.CreateSuccess(flights, "Flights retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving flights");
                return StatusCode(500, ApiResponse<IEnumerable<Flight>>.CreateError("Failed to retrieve flights"));
            }
        }

        /// <summary>
        /// Get flight by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<Flight>>> GetFlight(int id)
        {
            try
            {
                var flight = await _context.Flights
                    .Include(f => f.WorkOrderSubmissions)
                    .FirstOrDefaultAsync(f => f.Id == id);

                if (flight == null)
                {
                    return NotFound(ApiResponse<Flight>.CreateError("Flight not found"));
                }

                return Ok(ApiResponse<Flight>.CreateSuccess(flight, "Flight retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving flight {FlightId}", id);
                return StatusCode(500, ApiResponse<Flight>.CreateError("Failed to retrieve flight"));
            }
        }

        /// <summary>
        /// Import multiple flights from JSON array
        /// </summary>
        [HttpPost("import")]
        public async Task<ActionResult<ApiResponse<object>>> ImportFlights([FromBody] List<FlightImportRequest> flights)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<object>.CreateError("Invalid flight data"));
                }

                var importedCount = 0;
                var errors = new List<string>();

                foreach (var flightRequest in flights)
                {
                    try
                    {
                        // Check if flight already exists
                        var existingFlight = await _context.Flights
                            .FirstOrDefaultAsync(f => f.FlightNumber == flightRequest.FlightNumber);

                        if (existingFlight != null)
                        {
                            errors.Add($"Flight {flightRequest.FlightNumber} already exists");
                            continue;
                        }

                        var flight = new Flight
                        {
                            FlightNumber = flightRequest.FlightNumber,
                            ScheduledArrivalTimeUtc = flightRequest.ScheduledArrivalTimeUtc,
                            OriginAirport = flightRequest.OriginAirport,
                            DestinationAirport = flightRequest.DestinationAirport
                        };

                        _context.Flights.Add(flight);
                        importedCount++;
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Error importing flight {flightRequest.FlightNumber}: {ex.Message}");
                    }
                }

                await _context.SaveChangesAsync();

                var result = new
                {
                    ImportedCount = importedCount,
                    Errors = errors
                };

                _logger.LogInformation("Imported {ImportedCount} flights with {ErrorCount} errors", importedCount, errors.Count);
                return Ok(ApiResponse<object>.CreateSuccess(result, $"Import completed: {importedCount} flights imported"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing flights");
                return StatusCode(500, ApiResponse<object>.CreateError("Failed to import flights"));
            }
        }

        /// <summary>
        /// Import flights from CSV data
        /// </summary>
        [HttpPost("import/csv")]
        public async Task<ActionResult<ApiResponse<object>>> ImportFlightsFromCsv([FromBody] string csvData)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(csvData))
                {
                    return BadRequest(ApiResponse<object>.CreateError("CSV data is required"));
                }

                var lines = csvData.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                if (lines.Length < 2)
                {
                    return BadRequest(ApiResponse<object>.CreateError("CSV must contain header and at least one data row"));
                }

                var importedCount = 0;
                var errors = new List<string>();

                // Skip header row
                for (int i = 1; i < lines.Length; i++)
                {
                    var line = lines[i].Trim();
                    if (string.IsNullOrEmpty(line)) continue;

                    try
                    {
                        var parts = line.Split(',');
                        if (parts.Length != 4)
                        {
                            errors.Add($"Line {i + 1}: Invalid CSV format, expected 4 columns");
                            continue;
                        }

                        var flightNumber = parts[0].Trim();
                        var scheduledArrivalStr = parts[1].Trim();
                        var originAirport = parts[2].Trim();
                        var destinationAirport = parts[3].Trim();

                        if (!DateTime.TryParse(scheduledArrivalStr, out var scheduledArrival))
                        {
                            errors.Add($"Line {i + 1}: Invalid date format for {flightNumber}");
                            continue;
                        }

                        // Check if flight already exists
                        var existingFlight = await _context.Flights
                            .FirstOrDefaultAsync(f => f.FlightNumber == flightNumber);

                        if (existingFlight != null)
                        {
                            errors.Add($"Flight {flightNumber} already exists");
                            continue;
                        }

                        var flight = new Flight
                        {
                            FlightNumber = flightNumber,
                            ScheduledArrivalTimeUtc = scheduledArrival,
                            OriginAirport = originAirport,
                            DestinationAirport = destinationAirport
                        };

                        _context.Flights.Add(flight);
                        importedCount++;
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Line {i + 1}: Error processing line - {ex.Message}");
                    }
                }

                await _context.SaveChangesAsync();

                var result = new
                {
                    ImportedCount = importedCount,
                    Errors = errors
                };

                _logger.LogInformation("Imported {ImportedCount} flights from CSV with {ErrorCount} errors", importedCount, errors.Count);
                return Ok(ApiResponse<object>.CreateSuccess(result, $"CSV import completed: {importedCount} flights imported"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing flights from CSV");
                return StatusCode(500, ApiResponse<object>.CreateError("Failed to import flights from CSV"));
            }
        }

        /// <summary>
        /// Submit work order command for a flight
        /// </summary>
        [HttpPost("{flightId}/commands")]
        public async Task<ActionResult<ApiResponse<object>>> SubmitFlightCommand(int flightId, [FromBody] SubmitFlightCommandRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<object>.CreateError("Invalid command data"));
                }

                // Verify flight exists
                var flight = await _context.Flights.FindAsync(flightId);
                if (flight == null)
                {
                    return NotFound(ApiResponse<object>.CreateError("Flight not found"));
                }

                // Parse and validate commands
                var parsedCommands = _commandService.ParseCommands(request.CommandString);
                var isValid = _commandService.ValidateCommands(parsedCommands);
                var humanReadable = _commandService.ConvertToHumanReadable(parsedCommands);

                // Create submission record
                var submission = new FlightWorkOrderSubmission
                {
                    FlightId = flightId,
                    CommandString = request.CommandString,
                    ParsedCommandsJson = JsonSerializer.Serialize(parsedCommands),
                    SubmittedBy = User.Identity?.Name ?? "Unknown",
                    Notes = request.Notes
                };

                _context.FlightWorkOrderSubmissions.Add(submission);
                await _context.SaveChangesAsync();

                var result = new
                {
                    SubmissionId = submission.Id,
                    FlightNumber = flight.FlightNumber,
                    CommandString = request.CommandString,
                    IsValid = isValid,
                    ParsedCommands = parsedCommands,
                    HumanReadableCommands = humanReadable,
                    SubmittedAt = submission.SubmittedAt
                };

                _logger.LogInformation("Command submitted for flight {FlightId}: {CommandString}", flightId, request.CommandString);
                return Ok(ApiResponse<object>.CreateSuccess(result, "Work order command submitted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting command for flight {FlightId}", flightId);
                return StatusCode(500, ApiResponse<object>.CreateError("Failed to submit work order command"));
            }
        }

        /// <summary>
        /// Get work order command history for a flight
        /// </summary>
        [HttpGet("{flightId}/commands")]
        public async Task<ActionResult<ApiResponse<IEnumerable<object>>>> GetFlightCommandHistory(int flightId)
        {
            try
            {
                // Verify flight exists
                var flight = await _context.Flights.FindAsync(flightId);
                if (flight == null)
                {
                    return NotFound(ApiResponse<IEnumerable<object>>.CreateError("Flight not found"));
                }

                var submissions = await _context.FlightWorkOrderSubmissions
                    .Where(s => s.FlightId == flightId)
                    .OrderByDescending(s => s.SubmittedAt)
                    .ToListAsync();

                var result = submissions.Select(s => 
                {
                    var parsedCommands = JsonSerializer.Deserialize<List<FlightCommand>>(s.ParsedCommandsJson) ?? new List<FlightCommand>();
                    return new
                    {
                        s.Id,
                        s.CommandString,
                        ParsedCommands = parsedCommands,
                        HumanReadableCommands = _commandService.ConvertToHumanReadable(parsedCommands),
                        s.SubmittedAt,
                        s.SubmittedBy,
                        s.Notes
                    };
                }).ToList();

                return Ok(ApiResponse<IEnumerable<object>>.CreateSuccess(result, "Command history retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving command history for flight {FlightId}", flightId);
                return StatusCode(500, ApiResponse<IEnumerable<object>>.CreateError("Failed to retrieve command history"));
            }
        }

        /// <summary>
        /// Parse and validate command string without submitting
        /// </summary>
        [HttpPost("commands/validate")]
        public ActionResult<ApiResponse<object>> ValidateCommand([FromBody] string commandString)
        {
            try
            {
                var parsedCommands = _commandService.ParseCommands(commandString);
                var isValid = _commandService.ValidateCommands(parsedCommands);
                var humanReadable = _commandService.ConvertToHumanReadable(parsedCommands);

                var result = new
                {
                    CommandString = commandString,
                    IsValid = isValid,
                    ParsedCommands = parsedCommands,
                    HumanReadableCommands = humanReadable
                };

                return Ok(ApiResponse<object>.CreateSuccess(result, "Command validated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating command: {CommandString}", commandString);
                return StatusCode(500, ApiResponse<object>.CreateError("Failed to validate command"));
            }
        }
    }
}