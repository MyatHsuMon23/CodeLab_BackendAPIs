using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Flights_Work_Order_APIs.Data;
using Flights_Work_Order_APIs.Models;
using Flights_Work_Order_APIs.DTOs;
using Flights_Work_Order_APIs.Services;
using CsvHelper;
using System.Globalization;
using System.Text.Json;

namespace Flights_Work_Order_APIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FlightsController : ControllerBase
    {
        private readonly FlightWorkOrderContext _context;
        private readonly ILogger<FlightsController> _logger;
        private readonly IWorkOrderCommandService _commandService;

        public FlightsController(FlightWorkOrderContext context, ILogger<FlightsController> logger, IWorkOrderCommandService commandService)
        {
            _context = context;
            _logger = logger;
            _commandService = commandService;
        }

        // GET: api/Flights
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FlightDto>>> GetFlights(
            [FromQuery] string? sortBy = null,
            [FromQuery] string? sortOrder = "asc",
            [FromQuery] string? flightNumber = null)
        {
            var query = _context.Flights.Include(f => f.Aircraft).AsQueryable();

            // Apply filtering
            if (!string.IsNullOrWhiteSpace(flightNumber))
            {
                query = query.Where(f => f.FlightNumber.Contains(flightNumber));
            }

            // Apply sorting
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                var isDescending = sortOrder?.ToLower() == "desc";
                
                query = sortBy.ToLower() switch
                {
                    "flightnumber" => isDescending ? query.OrderByDescending(f => f.FlightNumber) : query.OrderBy(f => f.FlightNumber),
                    "origin" => isDescending ? query.OrderByDescending(f => f.Origin) : query.OrderBy(f => f.Origin),
                    "destination" => isDescending ? query.OrderByDescending(f => f.Destination) : query.OrderBy(f => f.Destination),
                    "departuretime" => isDescending ? query.OrderByDescending(f => f.DepartureTime) : query.OrderBy(f => f.DepartureTime),
                    "arrivaltime" => isDescending ? query.OrderByDescending(f => f.ArrivalTime) : query.OrderBy(f => f.ArrivalTime),
                    "status" => isDescending ? query.OrderByDescending(f => f.Status) : query.OrderBy(f => f.Status),
                    _ => query.OrderBy(f => f.Id)
                };
            }
            else
            {
                query = query.OrderBy(f => f.Id);
            }

            var flights = await query
                .Select(f => new FlightDto
                {
                    Id = f.Id,
                    FlightNumber = f.FlightNumber,
                    Origin = f.Origin,
                    Destination = f.Destination,
                    DepartureTime = f.DepartureTime,
                    ArrivalTime = f.ArrivalTime,
                    Status = f.Status,
                    AircraftId = f.AircraftId,
                    AircraftRegistration = f.Aircraft.RegistrationNumber,
                    CreatedAt = f.CreatedAt,
                    UpdatedAt = f.UpdatedAt
                })
                .ToListAsync();

            return Ok(flights);
        }

        // GET: api/Flights/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FlightDto>> GetFlight(int id)
        {
            var flight = await _context.Flights
                .Include(f => f.Aircraft)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (flight == null)
            {
                return NotFound();
            }

            var flightDto = new FlightDto
            {
                Id = flight.Id,
                FlightNumber = flight.FlightNumber,
                Origin = flight.Origin,
                Destination = flight.Destination,
                DepartureTime = flight.DepartureTime,
                ArrivalTime = flight.ArrivalTime,
                Status = flight.Status,
                AircraftId = flight.AircraftId,
                AircraftRegistration = flight.Aircraft.RegistrationNumber,
                CreatedAt = flight.CreatedAt,
                UpdatedAt = flight.UpdatedAt
            };

            return Ok(flightDto);
        }

        // POST: api/Flights
        [HttpPost]
        public async Task<ActionResult<FlightDto>> CreateFlight(CreateFlightDto createFlightDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validate that arrival time is after departure time
            if (createFlightDto.ArrivalTime <= createFlightDto.DepartureTime)
            {
                return BadRequest("Arrival time must be after departure time.");
            }

            // Check if aircraft exists
            if (!await _context.Aircraft.AnyAsync(a => a.Id == createFlightDto.AircraftId))
            {
                return BadRequest("Aircraft not found.");
            }

            var flight = new Flight
            {
                FlightNumber = createFlightDto.FlightNumber,
                Origin = createFlightDto.Origin,
                Destination = createFlightDto.Destination,
                DepartureTime = createFlightDto.DepartureTime,
                ArrivalTime = createFlightDto.ArrivalTime,
                Status = createFlightDto.Status,
                AircraftId = createFlightDto.AircraftId
            };

            _context.Flights.Add(flight);
            await _context.SaveChangesAsync();

            // Reload with aircraft info
            await _context.Entry(flight)
                .Reference(f => f.Aircraft)
                .LoadAsync();

            var flightDto = new FlightDto
            {
                Id = flight.Id,
                FlightNumber = flight.FlightNumber,
                Origin = flight.Origin,
                Destination = flight.Destination,
                DepartureTime = flight.DepartureTime,
                ArrivalTime = flight.ArrivalTime,
                Status = flight.Status,
                AircraftId = flight.AircraftId,
                AircraftRegistration = flight.Aircraft.RegistrationNumber,
                CreatedAt = flight.CreatedAt,
                UpdatedAt = flight.UpdatedAt
            };

            return CreatedAtAction(nameof(GetFlight), new { id = flight.Id }, flightDto);
        }

        // PUT: api/Flights/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFlight(int id, UpdateFlightDto updateFlightDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var flight = await _context.Flights.FindAsync(id);
            if (flight == null)
            {
                return NotFound();
            }

            // Update only provided fields
            if (!string.IsNullOrEmpty(updateFlightDto.FlightNumber))
                flight.FlightNumber = updateFlightDto.FlightNumber;
            if (!string.IsNullOrEmpty(updateFlightDto.Origin))
                flight.Origin = updateFlightDto.Origin;
            if (!string.IsNullOrEmpty(updateFlightDto.Destination))
                flight.Destination = updateFlightDto.Destination;
            if (updateFlightDto.DepartureTime.HasValue)
                flight.DepartureTime = updateFlightDto.DepartureTime.Value;
            if (updateFlightDto.ArrivalTime.HasValue)
                flight.ArrivalTime = updateFlightDto.ArrivalTime.Value;
            if (updateFlightDto.Status.HasValue)
                flight.Status = updateFlightDto.Status.Value;

            // If aircraft is being changed, validate it exists
            if (updateFlightDto.AircraftId.HasValue)
            {
                if (!await _context.Aircraft.AnyAsync(a => a.Id == updateFlightDto.AircraftId.Value))
                {
                    return BadRequest("Aircraft not found.");
                }
                flight.AircraftId = updateFlightDto.AircraftId.Value;
            }

            // Validate that arrival time is after departure time
            if (flight.ArrivalTime <= flight.DepartureTime)
            {
                return BadRequest("Arrival time must be after departure time.");
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FlightExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Flights/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFlight(int id)
        {
            var flight = await _context.Flights.FindAsync(id);
            if (flight == null)
            {
                return NotFound();
            }

            // Check if flight has associated work orders
            var hasWorkOrders = await _context.WorkOrders.AnyAsync(w => w.FlightId == id);

            if (hasWorkOrders)
            {
                return BadRequest("Cannot delete flight that has associated work orders.");
            }

            _context.Flights.Remove(flight);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Flights/by-aircraft/5
        [HttpGet("by-aircraft/{aircraftId}")]
        public async Task<ActionResult<IEnumerable<FlightDto>>> GetFlightsByAircraft(int aircraftId)
        {
            var flights = await _context.Flights
                .Include(f => f.Aircraft)
                .Where(f => f.AircraftId == aircraftId)
                .Select(f => new FlightDto
                {
                    Id = f.Id,
                    FlightNumber = f.FlightNumber,
                    Origin = f.Origin,
                    Destination = f.Destination,
                    DepartureTime = f.DepartureTime,
                    ArrivalTime = f.ArrivalTime,
                    Status = f.Status,
                    AircraftId = f.AircraftId,
                    AircraftRegistration = f.Aircraft.RegistrationNumber,
                    CreatedAt = f.CreatedAt,
                    UpdatedAt = f.UpdatedAt
                })
                .ToListAsync();

            return Ok(flights);
        }

        // POST: api/Flights/import-csv
        [HttpPost("import-csv")]
        public async Task<ActionResult<ImportResultDto>> ImportFlightsFromCsv(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded");
            }

            if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Only CSV files are supported");
            }

            var result = new ImportResultDto();

            try
            {
                using var reader = new StreamReader(file.OpenReadStream());
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

                var importFlights = csv.GetRecords<ImportFlightDto>().ToList();

                // Get a default aircraft (assuming first available aircraft)
                var defaultAircraft = await _context.Aircraft.FirstAsync();

                foreach (var importFlight in importFlights)
                {
                    try
                    {
                        // Validate the imported flight
                        if (!ModelState.IsValid)
                        {
                            result.Errors.Add($"Invalid data for flight {importFlight.FlightNumber}");
                            result.FailureCount++;
                            continue;
                        }

                        // Check if flight already exists
                        if (await _context.Flights.AnyAsync(f => f.FlightNumber == importFlight.FlightNumber))
                        {
                            result.Errors.Add($"Flight {importFlight.FlightNumber} already exists");
                            result.FailureCount++;
                            continue;
                        }

                        // Create flight entity
                        // Note: Since import only has arrival time, we'll set departure time to 1 hour before arrival
                        var flight = new Flight
                        {
                            FlightNumber = importFlight.FlightNumber,
                            Origin = importFlight.OriginAirport,
                            Destination = importFlight.DestinationAirport,
                            ArrivalTime = importFlight.ScheduledArrivalTimeUtc,
                            DepartureTime = importFlight.ScheduledArrivalTimeUtc.AddHours(-1), // Default to 1 hour before arrival
                            Status = FlightStatus.Scheduled,
                            AircraftId = defaultAircraft.Id
                        };

                        _context.Flights.Add(flight);
                        await _context.SaveChangesAsync();

                        // Load aircraft info for response
                        await _context.Entry(flight).Reference(f => f.Aircraft).LoadAsync();

                        var flightDto = new FlightDto
                        {
                            Id = flight.Id,
                            FlightNumber = flight.FlightNumber,
                            Origin = flight.Origin,
                            Destination = flight.Destination,
                            DepartureTime = flight.DepartureTime,
                            ArrivalTime = flight.ArrivalTime,
                            Status = flight.Status,
                            AircraftId = flight.AircraftId,
                            AircraftRegistration = flight.Aircraft.RegistrationNumber,
                            CreatedAt = flight.CreatedAt,
                            UpdatedAt = flight.UpdatedAt
                        };

                        result.ImportedFlights.Add(flightDto);
                        result.SuccessCount++;
                    }
                    catch (Exception ex)
                    {
                        result.Errors.Add($"Error importing flight {importFlight.FlightNumber}: {ex.Message}");
                        result.FailureCount++;
                    }
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error processing CSV file: {ex.Message}");
            }
        }

        // POST: api/Flights/import-json
        [HttpPost("import-json")]
        public async Task<ActionResult<ImportResultDto>> ImportFlightsFromJson(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded");
            }

            if (!file.FileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Only JSON files are supported");
            }

            var result = new ImportResultDto();

            try
            {
                using var reader = new StreamReader(file.OpenReadStream());
                var jsonContent = await reader.ReadToEndAsync();
                
                var importFlights = JsonSerializer.Deserialize<List<ImportFlightDto>>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (importFlights == null)
                {
                    return BadRequest("Invalid JSON format");
                }

                // Get a default aircraft (assuming first available aircraft)
                var defaultAircraft = await _context.Aircraft.FirstAsync();

                foreach (var importFlight in importFlights)
                {
                    try
                    {
                        // Check if flight already exists
                        if (await _context.Flights.AnyAsync(f => f.FlightNumber == importFlight.FlightNumber))
                        {
                            result.Errors.Add($"Flight {importFlight.FlightNumber} already exists");
                            result.FailureCount++;
                            continue;
                        }

                        // Create flight entity
                        // Note: Since import only has arrival time, we'll set departure time to 1 hour before arrival
                        var flight = new Flight
                        {
                            FlightNumber = importFlight.FlightNumber,
                            Origin = importFlight.OriginAirport,
                            Destination = importFlight.DestinationAirport,
                            ArrivalTime = importFlight.ScheduledArrivalTimeUtc,
                            DepartureTime = importFlight.ScheduledArrivalTimeUtc.AddHours(-1), // Default to 1 hour before arrival
                            Status = FlightStatus.Scheduled,
                            AircraftId = defaultAircraft.Id
                        };

                        _context.Flights.Add(flight);
                        await _context.SaveChangesAsync();

                        // Load aircraft info for response
                        await _context.Entry(flight).Reference(f => f.Aircraft).LoadAsync();

                        var flightDto = new FlightDto
                        {
                            Id = flight.Id,
                            FlightNumber = flight.FlightNumber,
                            Origin = flight.Origin,
                            Destination = flight.Destination,
                            DepartureTime = flight.DepartureTime,
                            ArrivalTime = flight.ArrivalTime,
                            Status = flight.Status,
                            AircraftId = flight.AircraftId,
                            AircraftRegistration = flight.Aircraft.RegistrationNumber,
                            CreatedAt = flight.CreatedAt,
                            UpdatedAt = flight.UpdatedAt
                        };

                        result.ImportedFlights.Add(flightDto);
                        result.SuccessCount++;
                    }
                    catch (Exception ex)
                    {
                        result.Errors.Add($"Error importing flight {importFlight.FlightNumber}: {ex.Message}");
                        result.FailureCount++;
                    }
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error processing JSON file: {ex.Message}");
            }
        }

        // POST: api/Flights/{id}/process-work-order-command
        [HttpPost("{id}/process-work-order-command")]
        public async Task<ActionResult<WorkOrderCommandResultDto>> ProcessWorkOrderCommand(int id, [FromBody] WorkOrderCommandDto commandDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var flight = await _context.Flights.FindAsync(id);
            if (flight == null)
            {
                return NotFound($"Flight with ID {id} not found");
            }

            var parsedCommand = _commandService.ParseCommand(commandDto.CommandString);

            var result = new WorkOrderCommandResultDto
            {
                ParsedCommand = parsedCommand,
                FlightId = flight.Id,
                FlightNumber = flight.FlightNumber,
                Success = parsedCommand.IsValid,
                ErrorMessage = parsedCommand.IsValid ? null : string.Join("; ", parsedCommand.ValidationErrors)
            };

            return Ok(result);
        }

        private bool FlightExists(int id)
        {
            return _context.Flights.Any(e => e.Id == id);
        }
    }
}