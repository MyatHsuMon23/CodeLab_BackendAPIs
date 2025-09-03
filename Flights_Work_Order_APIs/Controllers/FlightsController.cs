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
        /// Get all flights with optional filtering, sorting and pagination
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<PaginatedApiResponse<IEnumerable<Flight>>>> GetFlights(
            [FromQuery] string? flightNumber = null,
            [FromQuery] string? sortBy = "FlightNumber",
            [FromQuery] bool sortDescending = false,
            [FromQuery] int page = 1,
            [FromQuery] int perPage = 10)
        {
            try
            {
                // Validate pagination parameters
                if (page < 1) page = 1;
                if (perPage < 1) perPage = 10;
                if (perPage > 100) perPage = 100; // Limit maximum page size

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

                // Get total count before pagination
                var total = await query.CountAsync();

                // Calculate pagination values
                var lastPage = (int)Math.Ceiling((double)total / perPage);
                var skip = (page - 1) * perPage;

                // Apply pagination
                var flights = await query.Skip(skip).Take(perPage).ToListAsync();

                // Create pagination metadata
                var pagination = new Pagination
                {
                    CurrentPage = page,
                    LastPage = lastPage,
                    PerPage = perPage,
                    Total = total
                };

                return Ok(PaginatedApiResponse<IEnumerable<Flight>>.CreateSuccess(flights, pagination, "Flights retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving flights");
                return StatusCode(500, PaginatedApiResponse<IEnumerable<Flight>>.CreateError("Failed to retrieve flights"));
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
        /// Import flights from CSV or JSON file
        /// </summary>
        [HttpPost("import/csv")]
        public async Task<ActionResult<ApiResponse<object>>> ImportFlightsFromCsv(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(ApiResponse<object>.CreateError("File is required"));
                }

                // Validate file type
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (fileExtension != ".csv" && fileExtension != ".json")
                {
                    return BadRequest(ApiResponse<object>.CreateError("Only CSV and JSON files are supported"));
                }

                string fileContent;
                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    fileContent = await reader.ReadToEndAsync();
                }

                if (string.IsNullOrWhiteSpace(fileContent))
                {
                    return BadRequest(ApiResponse<object>.CreateError("File content is empty"));
                }

                var importedCount = 0;
                var errors = new List<string>();

                if (fileExtension == ".csv")
                {
                    // Process CSV file
                    var result = await ProcessCsvContent(fileContent);
                    importedCount = result.ImportedCount;
                    errors = result.Errors;
                }
                else if (fileExtension == ".json")
                {
                    // Process JSON file
                    var result = await ProcessJsonContent(fileContent);
                    importedCount = result.ImportedCount;
                    errors = result.Errors;
                }

                await _context.SaveChangesAsync();

                var resultObj = new
                {
                    ImportedCount = importedCount,
                    Errors = errors
                };

                _logger.LogInformation("Imported {ImportedCount} flights from {FileType} with {ErrorCount} errors", 
                    importedCount, fileExtension.ToUpper(), errors.Count);
                return Ok(ApiResponse<object>.CreateSuccess(resultObj, $"File import completed: {importedCount} flights imported"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing flights from file");
                return StatusCode(500, ApiResponse<object>.CreateError("Failed to import flights from file"));
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

                // If commands are invalid, return error without saving
                if (!isValid)
                {
                    return BadRequest(ApiResponse<object>.CreateError("Invalid command string"));
                }

                // Create submission record only if commands are valid
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
        /// Create a work order for a specific flight
        /// </summary>
        [HttpPost("{flightId}/work-orders")]
        public async Task<ActionResult<ApiResponse<object>>> CreateFlightWorkOrder(int flightId, [FromBody] CreateFlightWorkOrderRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<object>.CreateError("Invalid work order data"));
                }

                // Verify flight exists
                var flight = await _context.Flights.FindAsync(flightId);
                if (flight == null)
                {
                    return NotFound(ApiResponse<object>.CreateError("Flight not found"));
                }

                // Create work order linked to flight
                var workOrderNumber = GenerateWorkOrderNumber();
                var workOrder = new FlightWorkOrder
                {
                    WorkOrderNumber = workOrderNumber,
                    AircraftRegistration = request.AircraftRegistration,
                    TaskDescription = $"Flight {flight.FlightNumber}: {request.TaskDescription}",
                    Priority = request.Priority,
                    AssignedTechnician = request.AssignedTechnician ?? string.Empty,
                    ScheduledDate = request.ScheduledDate,
                    Notes = $"Created for flight {flight.FlightNumber}. {request.Notes}",
                    CreatedBy = User.Identity?.Name ?? "Unknown",
                    Status = WorkOrderStatus.Open
                };

                _context.WorkOrders.Add(workOrder);
                await _context.SaveChangesAsync();

                var result = new
                {
                    WorkOrderId = workOrder.Id,
                    WorkOrderNumber = workOrder.WorkOrderNumber,
                    FlightId = flightId,
                    FlightNumber = flight.FlightNumber,
                    TaskDescription = workOrder.TaskDescription,
                    Priority = workOrder.Priority,
                    Status = workOrder.Status,
                    CreatedBy = workOrder.CreatedBy,
                    CreatedDate = workOrder.CreatedDate
                };

                _logger.LogInformation("Work order {WorkOrderNumber} created for flight {FlightNumber}", workOrderNumber, flight.FlightNumber);
                return Ok(ApiResponse<object>.CreateSuccess(result, "Work order created for flight successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating work order for flight {FlightId}", flightId);
                return StatusCode(500, ApiResponse<object>.CreateError("Failed to create work order for flight"));
            }
        }

        private string GenerateWorkOrderNumber()
        {
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var random = new Random().Next(100, 999);
            return $"WO-{timestamp}-{random}";
        }



        /// <summary>
        /// Process CSV file content and return import results
        /// </summary>
        private async Task<(int ImportedCount, List<string> Errors)> ProcessCsvContent(string csvContent)
        {
            var importedCount = 0;
            var errors = new List<string>();

            var lines = csvContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length < 2)
            {
                errors.Add("CSV must contain header and at least one data row");
                return (importedCount, errors);
            }

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

            return (importedCount, errors);
        }

        /// <summary>
        /// Process JSON file content and return import results
        /// </summary>
        private async Task<(int ImportedCount, List<string> Errors)> ProcessJsonContent(string jsonContent)
        {
            var importedCount = 0;
            var errors = new List<string>();

            try
            {
                var flightImports = JsonSerializer.Deserialize<FlightImportRequest[]>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (flightImports == null || flightImports.Length == 0)
                {
                    errors.Add("JSON file contains no flight data or is invalid");
                    return (importedCount, errors);
                }

                for (int i = 0; i < flightImports.Length; i++)
                {
                    var flightImport = flightImports[i];
                    try
                    {
                        if (string.IsNullOrWhiteSpace(flightImport.FlightNumber))
                        {
                            errors.Add($"Flight {i + 1}: Flight number is required");
                            continue;
                        }

                        if (string.IsNullOrWhiteSpace(flightImport.OriginAirport))
                        {
                            errors.Add($"Flight {i + 1}: Origin airport is required");
                            continue;
                        }

                        if (string.IsNullOrWhiteSpace(flightImport.DestinationAirport))
                        {
                            errors.Add($"Flight {i + 1}: Destination airport is required");
                            continue;
                        }

                        // Check if flight already exists
                        var existingFlight = await _context.Flights
                            .FirstOrDefaultAsync(f => f.FlightNumber == flightImport.FlightNumber);

                        if (existingFlight != null)
                        {
                            errors.Add($"Flight {flightImport.FlightNumber} already exists");
                            continue;
                        }

                        var flight = new Flight
                        {
                            FlightNumber = flightImport.FlightNumber,
                            ScheduledArrivalTimeUtc = flightImport.ScheduledArrivalTimeUtc,
                            OriginAirport = flightImport.OriginAirport,
                            DestinationAirport = flightImport.DestinationAirport
                        };

                        _context.Flights.Add(flight);
                        importedCount++;
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Flight {i + 1}: Error processing flight - {ex.Message}");
                    }
                }
            }
            catch (JsonException ex)
            {
                errors.Add($"Invalid JSON format: {ex.Message}");
            }
            catch (Exception ex)
            {
                errors.Add($"Error processing JSON file: {ex.Message}");
            }

            return (importedCount, errors);
        }
    }
}