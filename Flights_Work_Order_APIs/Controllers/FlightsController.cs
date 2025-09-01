using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Flights_Work_Order_APIs.Data;
using Flights_Work_Order_APIs.Models;
using Flights_Work_Order_APIs.DTOs;

namespace Flights_Work_Order_APIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FlightsController : ControllerBase
    {
        private readonly FlightWorkOrderContext _context;
        private readonly ILogger<FlightsController> _logger;

        public FlightsController(FlightWorkOrderContext context, ILogger<FlightsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Flights
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FlightDto>>> GetFlights()
        {
            var flights = await _context.Flights
                .Include(f => f.Aircraft)
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

        private bool FlightExists(int id)
        {
            return _context.Flights.Any(e => e.Id == id);
        }
    }
}