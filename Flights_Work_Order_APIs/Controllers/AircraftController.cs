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
    public class AircraftController : ControllerBase
    {
        private readonly FlightWorkOrderContext _context;
        private readonly ILogger<AircraftController> _logger;

        public AircraftController(FlightWorkOrderContext context, ILogger<AircraftController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Aircraft
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AircraftDto>>> GetAircraft()
        {
            var aircraft = await _context.Aircraft
                .Select(a => new AircraftDto
                {
                    Id = a.Id,
                    RegistrationNumber = a.RegistrationNumber,
                    Model = a.Model,
                    Manufacturer = a.Manufacturer,
                    ManufactureYear = a.ManufactureYear,
                    PassengerCapacity = a.PassengerCapacity,
                    Status = a.Status,
                    CreatedAt = a.CreatedAt,
                    UpdatedAt = a.UpdatedAt
                })
                .ToListAsync();

            return Ok(aircraft);
        }

        // GET: api/Aircraft/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AircraftDto>> GetAircraft(int id)
        {
            var aircraft = await _context.Aircraft.FindAsync(id);

            if (aircraft == null)
            {
                return NotFound();
            }

            var aircraftDto = new AircraftDto
            {
                Id = aircraft.Id,
                RegistrationNumber = aircraft.RegistrationNumber,
                Model = aircraft.Model,
                Manufacturer = aircraft.Manufacturer,
                ManufactureYear = aircraft.ManufactureYear,
                PassengerCapacity = aircraft.PassengerCapacity,
                Status = aircraft.Status,
                CreatedAt = aircraft.CreatedAt,
                UpdatedAt = aircraft.UpdatedAt
            };

            return Ok(aircraftDto);
        }

        // POST: api/Aircraft
        [HttpPost]
        public async Task<ActionResult<AircraftDto>> CreateAircraft(CreateAircraftDto createAircraftDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if registration number already exists
            if (await _context.Aircraft.AnyAsync(a => a.RegistrationNumber == createAircraftDto.RegistrationNumber))
            {
                return Conflict("An aircraft with this registration number already exists.");
            }

            var aircraft = new Aircraft
            {
                RegistrationNumber = createAircraftDto.RegistrationNumber,
                Model = createAircraftDto.Model,
                Manufacturer = createAircraftDto.Manufacturer,
                ManufactureYear = createAircraftDto.ManufactureYear,
                PassengerCapacity = createAircraftDto.PassengerCapacity,
                Status = createAircraftDto.Status
            };

            _context.Aircraft.Add(aircraft);
            await _context.SaveChangesAsync();

            var aircraftDto = new AircraftDto
            {
                Id = aircraft.Id,
                RegistrationNumber = aircraft.RegistrationNumber,
                Model = aircraft.Model,
                Manufacturer = aircraft.Manufacturer,
                ManufactureYear = aircraft.ManufactureYear,
                PassengerCapacity = aircraft.PassengerCapacity,
                Status = aircraft.Status,
                CreatedAt = aircraft.CreatedAt,
                UpdatedAt = aircraft.UpdatedAt
            };

            return CreatedAtAction(nameof(GetAircraft), new { id = aircraft.Id }, aircraftDto);
        }

        // PUT: api/Aircraft/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAircraft(int id, UpdateAircraftDto updateAircraftDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var aircraft = await _context.Aircraft.FindAsync(id);
            if (aircraft == null)
            {
                return NotFound();
            }

            // Check if registration number already exists (excluding current aircraft)
            if (!string.IsNullOrEmpty(updateAircraftDto.RegistrationNumber) &&
                await _context.Aircraft.AnyAsync(a => a.RegistrationNumber == updateAircraftDto.RegistrationNumber && a.Id != id))
            {
                return Conflict("An aircraft with this registration number already exists.");
            }

            // Update only provided fields
            if (!string.IsNullOrEmpty(updateAircraftDto.RegistrationNumber))
                aircraft.RegistrationNumber = updateAircraftDto.RegistrationNumber;
            if (!string.IsNullOrEmpty(updateAircraftDto.Model))
                aircraft.Model = updateAircraftDto.Model;
            if (!string.IsNullOrEmpty(updateAircraftDto.Manufacturer))
                aircraft.Manufacturer = updateAircraftDto.Manufacturer;
            if (updateAircraftDto.ManufactureYear.HasValue)
                aircraft.ManufactureYear = updateAircraftDto.ManufactureYear.Value;
            if (updateAircraftDto.PassengerCapacity.HasValue)
                aircraft.PassengerCapacity = updateAircraftDto.PassengerCapacity.Value;
            if (updateAircraftDto.Status.HasValue)
                aircraft.Status = updateAircraftDto.Status.Value;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AircraftExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Aircraft/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAircraft(int id)
        {
            var aircraft = await _context.Aircraft.FindAsync(id);
            if (aircraft == null)
            {
                return NotFound();
            }

            // Check if aircraft has associated flights or work orders
            var hasFlights = await _context.Flights.AnyAsync(f => f.AircraftId == id);
            var hasWorkOrders = await _context.WorkOrders.AnyAsync(w => w.AircraftId == id);

            if (hasFlights || hasWorkOrders)
            {
                return BadRequest("Cannot delete aircraft that has associated flights or work orders.");
            }

            _context.Aircraft.Remove(aircraft);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AircraftExists(int id)
        {
            return _context.Aircraft.Any(e => e.Id == id);
        }
    }
}