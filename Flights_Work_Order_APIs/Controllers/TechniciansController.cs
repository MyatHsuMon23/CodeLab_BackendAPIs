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
    public class TechniciansController : ControllerBase
    {
        private readonly FlightWorkOrderContext _context;
        private readonly ILogger<TechniciansController> _logger;

        public TechniciansController(FlightWorkOrderContext context, ILogger<TechniciansController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Technicians
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TechnicianDto>>> GetTechnicians()
        {
            var technicians = await _context.Technicians
                .Select(t => new TechnicianDto
                {
                    Id = t.Id,
                    FirstName = t.FirstName,
                    LastName = t.LastName,
                    FullName = t.FullName,
                    Email = t.Email,
                    PhoneNumber = t.PhoneNumber,
                    EmployeeId = t.EmployeeId,
                    Specialization = t.Specialization,
                    Status = t.Status,
                    HireDate = t.HireDate,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt
                })
                .ToListAsync();

            return Ok(technicians);
        }

        // GET: api/Technicians/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TechnicianDto>> GetTechnician(int id)
        {
            var technician = await _context.Technicians.FindAsync(id);

            if (technician == null)
            {
                return NotFound();
            }

            var technicianDto = new TechnicianDto
            {
                Id = technician.Id,
                FirstName = technician.FirstName,
                LastName = technician.LastName,
                FullName = technician.FullName,
                Email = technician.Email,
                PhoneNumber = technician.PhoneNumber,
                EmployeeId = technician.EmployeeId,
                Specialization = technician.Specialization,
                Status = technician.Status,
                HireDate = technician.HireDate,
                CreatedAt = technician.CreatedAt,
                UpdatedAt = technician.UpdatedAt
            };

            return Ok(technicianDto);
        }

        // POST: api/Technicians
        [HttpPost]
        public async Task<ActionResult<TechnicianDto>> CreateTechnician(CreateTechnicianDto createTechnicianDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if email already exists
            if (await _context.Technicians.AnyAsync(t => t.Email == createTechnicianDto.Email))
            {
                return Conflict("A technician with this email already exists.");
            }

            // Check if employee ID already exists
            if (await _context.Technicians.AnyAsync(t => t.EmployeeId == createTechnicianDto.EmployeeId))
            {
                return Conflict("A technician with this employee ID already exists.");
            }

            var technician = new Technician
            {
                FirstName = createTechnicianDto.FirstName,
                LastName = createTechnicianDto.LastName,
                Email = createTechnicianDto.Email,
                PhoneNumber = createTechnicianDto.PhoneNumber,
                EmployeeId = createTechnicianDto.EmployeeId,
                Specialization = createTechnicianDto.Specialization,
                Status = createTechnicianDto.Status,
                HireDate = createTechnicianDto.HireDate
            };

            _context.Technicians.Add(technician);
            await _context.SaveChangesAsync();

            var technicianDto = new TechnicianDto
            {
                Id = technician.Id,
                FirstName = technician.FirstName,
                LastName = technician.LastName,
                FullName = technician.FullName,
                Email = technician.Email,
                PhoneNumber = technician.PhoneNumber,
                EmployeeId = technician.EmployeeId,
                Specialization = technician.Specialization,
                Status = technician.Status,
                HireDate = technician.HireDate,
                CreatedAt = technician.CreatedAt,
                UpdatedAt = technician.UpdatedAt
            };

            return CreatedAtAction(nameof(GetTechnician), new { id = technician.Id }, technicianDto);
        }

        // PUT: api/Technicians/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTechnician(int id, UpdateTechnicianDto updateTechnicianDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var technician = await _context.Technicians.FindAsync(id);
            if (technician == null)
            {
                return NotFound();
            }

            // Check if email already exists (excluding current technician)
            if (!string.IsNullOrEmpty(updateTechnicianDto.Email) &&
                await _context.Technicians.AnyAsync(t => t.Email == updateTechnicianDto.Email && t.Id != id))
            {
                return Conflict("A technician with this email already exists.");
            }

            // Check if employee ID already exists (excluding current technician)
            if (!string.IsNullOrEmpty(updateTechnicianDto.EmployeeId) &&
                await _context.Technicians.AnyAsync(t => t.EmployeeId == updateTechnicianDto.EmployeeId && t.Id != id))
            {
                return Conflict("A technician with this employee ID already exists.");
            }

            // Update only provided fields
            if (!string.IsNullOrEmpty(updateTechnicianDto.FirstName))
                technician.FirstName = updateTechnicianDto.FirstName;
            if (!string.IsNullOrEmpty(updateTechnicianDto.LastName))
                technician.LastName = updateTechnicianDto.LastName;
            if (!string.IsNullOrEmpty(updateTechnicianDto.Email))
                technician.Email = updateTechnicianDto.Email;
            if (updateTechnicianDto.PhoneNumber != null)
                technician.PhoneNumber = updateTechnicianDto.PhoneNumber;
            if (!string.IsNullOrEmpty(updateTechnicianDto.EmployeeId))
                technician.EmployeeId = updateTechnicianDto.EmployeeId;
            if (!string.IsNullOrEmpty(updateTechnicianDto.Specialization))
                technician.Specialization = updateTechnicianDto.Specialization;
            if (updateTechnicianDto.Status.HasValue)
                technician.Status = updateTechnicianDto.Status.Value;
            if (updateTechnicianDto.HireDate.HasValue)
                technician.HireDate = updateTechnicianDto.HireDate.Value;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TechnicianExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Technicians/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTechnician(int id)
        {
            var technician = await _context.Technicians.FindAsync(id);
            if (technician == null)
            {
                return NotFound();
            }

            // Check if technician has associated work orders
            var hasWorkOrders = await _context.WorkOrders.AnyAsync(w => w.AssignedTechnicianId == id);

            if (hasWorkOrders)
            {
                return BadRequest("Cannot delete technician that has associated work orders. Consider changing their status to Inactive instead.");
            }

            _context.Technicians.Remove(technician);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Technicians/by-specialization/{specialization}
        [HttpGet("by-specialization/{specialization}")]
        public async Task<ActionResult<IEnumerable<TechnicianDto>>> GetTechniciansBySpecialization(string specialization)
        {
            var technicians = await _context.Technicians
                .Where(t => t.Specialization.Contains(specialization))
                .Select(t => new TechnicianDto
                {
                    Id = t.Id,
                    FirstName = t.FirstName,
                    LastName = t.LastName,
                    FullName = t.FullName,
                    Email = t.Email,
                    PhoneNumber = t.PhoneNumber,
                    EmployeeId = t.EmployeeId,
                    Specialization = t.Specialization,
                    Status = t.Status,
                    HireDate = t.HireDate,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt
                })
                .ToListAsync();

            return Ok(technicians);
        }

        // GET: api/Technicians/active
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<TechnicianDto>>> GetActiveTechnicians()
        {
            var technicians = await _context.Technicians
                .Where(t => t.Status == TechnicianStatus.Active)
                .Select(t => new TechnicianDto
                {
                    Id = t.Id,
                    FirstName = t.FirstName,
                    LastName = t.LastName,
                    FullName = t.FullName,
                    Email = t.Email,
                    PhoneNumber = t.PhoneNumber,
                    EmployeeId = t.EmployeeId,
                    Specialization = t.Specialization,
                    Status = t.Status,
                    HireDate = t.HireDate,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt
                })
                .ToListAsync();

            return Ok(technicians);
        }

        private bool TechnicianExists(int id)
        {
            return _context.Technicians.Any(e => e.Id == id);
        }
    }
}