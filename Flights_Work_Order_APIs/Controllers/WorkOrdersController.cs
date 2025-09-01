using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Flights_Work_Order_APIs.Data;
using Flights_Work_Order_APIs.Models;
using Flights_Work_Order_APIs.DTOs;

namespace Flights_Work_Order_APIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkOrdersController : ControllerBase
    {
        private readonly FlightWorkOrderContext _context;
        private readonly ILogger<WorkOrdersController> _logger;

        public WorkOrdersController(FlightWorkOrderContext context, ILogger<WorkOrdersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/WorkOrders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WorkOrderDto>>> GetWorkOrders()
        {
            var workOrders = await _context.WorkOrders
                .Include(w => w.Aircraft)
                .Include(w => w.Flight)
                .Include(w => w.AssignedTechnician)
                .Select(w => new WorkOrderDto
                {
                    Id = w.Id,
                    WorkOrderNumber = w.WorkOrderNumber,
                    Title = w.Title,
                    Description = w.Description,
                    Priority = w.Priority,
                    Status = w.Status,
                    Type = w.Type,
                    AircraftId = w.AircraftId,
                    AircraftRegistration = w.Aircraft.RegistrationNumber,
                    FlightId = w.FlightId,
                    FlightNumber = w.Flight != null ? w.Flight.FlightNumber : null,
                    AssignedTechnicianId = w.AssignedTechnicianId,
                    AssignedTechnicianName = w.AssignedTechnician != null ? w.AssignedTechnician.FullName : null,
                    CreatedDate = w.CreatedDate,
                    ScheduledDate = w.ScheduledDate,
                    StartedDate = w.StartedDate,
                    CompletedDate = w.CompletedDate,
                    DueDate = w.DueDate,
                    Notes = w.Notes,
                    EstimatedHours = w.EstimatedHours,
                    ActualHours = w.ActualHours,
                    CreatedAt = w.CreatedAt,
                    UpdatedAt = w.UpdatedAt
                })
                .ToListAsync();

            return Ok(workOrders);
        }

        // GET: api/WorkOrders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<WorkOrderDto>> GetWorkOrder(int id)
        {
            var workOrder = await _context.WorkOrders
                .Include(w => w.Aircraft)
                .Include(w => w.Flight)
                .Include(w => w.AssignedTechnician)
                .FirstOrDefaultAsync(w => w.Id == id);

            if (workOrder == null)
            {
                return NotFound();
            }

            var workOrderDto = new WorkOrderDto
            {
                Id = workOrder.Id,
                WorkOrderNumber = workOrder.WorkOrderNumber,
                Title = workOrder.Title,
                Description = workOrder.Description,
                Priority = workOrder.Priority,
                Status = workOrder.Status,
                Type = workOrder.Type,
                AircraftId = workOrder.AircraftId,
                AircraftRegistration = workOrder.Aircraft.RegistrationNumber,
                FlightId = workOrder.FlightId,
                FlightNumber = workOrder.Flight?.FlightNumber,
                AssignedTechnicianId = workOrder.AssignedTechnicianId,
                AssignedTechnicianName = workOrder.AssignedTechnician?.FullName,
                CreatedDate = workOrder.CreatedDate,
                ScheduledDate = workOrder.ScheduledDate,
                StartedDate = workOrder.StartedDate,
                CompletedDate = workOrder.CompletedDate,
                DueDate = workOrder.DueDate,
                Notes = workOrder.Notes,
                EstimatedHours = workOrder.EstimatedHours,
                ActualHours = workOrder.ActualHours,
                CreatedAt = workOrder.CreatedAt,
                UpdatedAt = workOrder.UpdatedAt
            };

            return Ok(workOrderDto);
        }

        // POST: api/WorkOrders
        [HttpPost]
        public async Task<ActionResult<WorkOrderDto>> CreateWorkOrder(CreateWorkOrderDto createWorkOrderDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if work order number already exists
            if (await _context.WorkOrders.AnyAsync(w => w.WorkOrderNumber == createWorkOrderDto.WorkOrderNumber))
            {
                return Conflict("A work order with this number already exists.");
            }

            // Check if aircraft exists
            if (!await _context.Aircraft.AnyAsync(a => a.Id == createWorkOrderDto.AircraftId))
            {
                return BadRequest("Aircraft not found.");
            }

            // Check if flight exists (if provided)
            if (createWorkOrderDto.FlightId.HasValue &&
                !await _context.Flights.AnyAsync(f => f.Id == createWorkOrderDto.FlightId.Value))
            {
                return BadRequest("Flight not found.");
            }

            // Check if technician exists (if provided)
            if (createWorkOrderDto.AssignedTechnicianId.HasValue &&
                !await _context.Technicians.AnyAsync(t => t.Id == createWorkOrderDto.AssignedTechnicianId.Value))
            {
                return BadRequest("Technician not found.");
            }

            var workOrder = new WorkOrder
            {
                WorkOrderNumber = createWorkOrderDto.WorkOrderNumber,
                Title = createWorkOrderDto.Title,
                Description = createWorkOrderDto.Description,
                Priority = createWorkOrderDto.Priority,
                Status = createWorkOrderDto.Status,
                Type = createWorkOrderDto.Type,
                AircraftId = createWorkOrderDto.AircraftId,
                FlightId = createWorkOrderDto.FlightId,
                AssignedTechnicianId = createWorkOrderDto.AssignedTechnicianId,
                ScheduledDate = createWorkOrderDto.ScheduledDate,
                DueDate = createWorkOrderDto.DueDate,
                Notes = createWorkOrderDto.Notes,
                EstimatedHours = createWorkOrderDto.EstimatedHours
            };

            _context.WorkOrders.Add(workOrder);
            await _context.SaveChangesAsync();

            // Reload with related data
            await _context.Entry(workOrder)
                .Reference(w => w.Aircraft)
                .LoadAsync();
            await _context.Entry(workOrder)
                .Reference(w => w.Flight)
                .LoadAsync();
            await _context.Entry(workOrder)
                .Reference(w => w.AssignedTechnician)
                .LoadAsync();

            var workOrderDto = new WorkOrderDto
            {
                Id = workOrder.Id,
                WorkOrderNumber = workOrder.WorkOrderNumber,
                Title = workOrder.Title,
                Description = workOrder.Description,
                Priority = workOrder.Priority,
                Status = workOrder.Status,
                Type = workOrder.Type,
                AircraftId = workOrder.AircraftId,
                AircraftRegistration = workOrder.Aircraft.RegistrationNumber,
                FlightId = workOrder.FlightId,
                FlightNumber = workOrder.Flight?.FlightNumber,
                AssignedTechnicianId = workOrder.AssignedTechnicianId,
                AssignedTechnicianName = workOrder.AssignedTechnician?.FullName,
                CreatedDate = workOrder.CreatedDate,
                ScheduledDate = workOrder.ScheduledDate,
                StartedDate = workOrder.StartedDate,
                CompletedDate = workOrder.CompletedDate,
                DueDate = workOrder.DueDate,
                Notes = workOrder.Notes,
                EstimatedHours = workOrder.EstimatedHours,
                ActualHours = workOrder.ActualHours,
                CreatedAt = workOrder.CreatedAt,
                UpdatedAt = workOrder.UpdatedAt
            };

            return CreatedAtAction(nameof(GetWorkOrder), new { id = workOrder.Id }, workOrderDto);
        }

        // PUT: api/WorkOrders/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWorkOrder(int id, UpdateWorkOrderDto updateWorkOrderDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var workOrder = await _context.WorkOrders.FindAsync(id);
            if (workOrder == null)
            {
                return NotFound();
            }

            // Check if work order number already exists (excluding current work order)
            if (!string.IsNullOrEmpty(updateWorkOrderDto.WorkOrderNumber) &&
                await _context.WorkOrders.AnyAsync(w => w.WorkOrderNumber == updateWorkOrderDto.WorkOrderNumber && w.Id != id))
            {
                return Conflict("A work order with this number already exists.");
            }

            // Update only provided fields
            if (!string.IsNullOrEmpty(updateWorkOrderDto.WorkOrderNumber))
                workOrder.WorkOrderNumber = updateWorkOrderDto.WorkOrderNumber;
            if (!string.IsNullOrEmpty(updateWorkOrderDto.Title))
                workOrder.Title = updateWorkOrderDto.Title;
            if (!string.IsNullOrEmpty(updateWorkOrderDto.Description))
                workOrder.Description = updateWorkOrderDto.Description;
            if (updateWorkOrderDto.Priority.HasValue)
                workOrder.Priority = updateWorkOrderDto.Priority.Value;
            if (updateWorkOrderDto.Status.HasValue)
            {
                workOrder.Status = updateWorkOrderDto.Status.Value;
                
                // Auto-set timestamps based on status changes
                if (updateWorkOrderDto.Status.Value == WorkOrderStatus.InProgress && workOrder.StartedDate == null)
                    workOrder.StartedDate = DateTime.UtcNow;
                if (updateWorkOrderDto.Status.Value == WorkOrderStatus.Completed && workOrder.CompletedDate == null)
                    workOrder.CompletedDate = DateTime.UtcNow;
            }
            if (updateWorkOrderDto.Type.HasValue)
                workOrder.Type = updateWorkOrderDto.Type.Value;
            if (updateWorkOrderDto.AircraftId.HasValue)
            {
                if (!await _context.Aircraft.AnyAsync(a => a.Id == updateWorkOrderDto.AircraftId.Value))
                {
                    return BadRequest("Aircraft not found.");
                }
                workOrder.AircraftId = updateWorkOrderDto.AircraftId.Value;
            }
            if (updateWorkOrderDto.FlightId.HasValue)
            {
                if (!await _context.Flights.AnyAsync(f => f.Id == updateWorkOrderDto.FlightId.Value))
                {
                    return BadRequest("Flight not found.");
                }
                workOrder.FlightId = updateWorkOrderDto.FlightId.Value;
            }
            if (updateWorkOrderDto.AssignedTechnicianId.HasValue)
            {
                if (!await _context.Technicians.AnyAsync(t => t.Id == updateWorkOrderDto.AssignedTechnicianId.Value))
                {
                    return BadRequest("Technician not found.");
                }
                workOrder.AssignedTechnicianId = updateWorkOrderDto.AssignedTechnicianId.Value;
            }
            if (updateWorkOrderDto.ScheduledDate.HasValue)
                workOrder.ScheduledDate = updateWorkOrderDto.ScheduledDate.Value;
            if (updateWorkOrderDto.StartedDate.HasValue)
                workOrder.StartedDate = updateWorkOrderDto.StartedDate.Value;
            if (updateWorkOrderDto.CompletedDate.HasValue)
                workOrder.CompletedDate = updateWorkOrderDto.CompletedDate.Value;
            if (updateWorkOrderDto.DueDate.HasValue)
                workOrder.DueDate = updateWorkOrderDto.DueDate.Value;
            if (updateWorkOrderDto.Notes != null)
                workOrder.Notes = updateWorkOrderDto.Notes;
            if (updateWorkOrderDto.EstimatedHours.HasValue)
                workOrder.EstimatedHours = updateWorkOrderDto.EstimatedHours.Value;
            if (updateWorkOrderDto.ActualHours.HasValue)
                workOrder.ActualHours = updateWorkOrderDto.ActualHours.Value;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WorkOrderExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/WorkOrders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWorkOrder(int id)
        {
            var workOrder = await _context.WorkOrders.FindAsync(id);
            if (workOrder == null)
            {
                return NotFound();
            }

            _context.WorkOrders.Remove(workOrder);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/WorkOrders/by-aircraft/5
        [HttpGet("by-aircraft/{aircraftId}")]
        public async Task<ActionResult<IEnumerable<WorkOrderDto>>> GetWorkOrdersByAircraft(int aircraftId)
        {
            var workOrders = await _context.WorkOrders
                .Include(w => w.Aircraft)
                .Include(w => w.Flight)
                .Include(w => w.AssignedTechnician)
                .Where(w => w.AircraftId == aircraftId)
                .Select(w => new WorkOrderDto
                {
                    Id = w.Id,
                    WorkOrderNumber = w.WorkOrderNumber,
                    Title = w.Title,
                    Description = w.Description,
                    Priority = w.Priority,
                    Status = w.Status,
                    Type = w.Type,
                    AircraftId = w.AircraftId,
                    AircraftRegistration = w.Aircraft.RegistrationNumber,
                    FlightId = w.FlightId,
                    FlightNumber = w.Flight != null ? w.Flight.FlightNumber : null,
                    AssignedTechnicianId = w.AssignedTechnicianId,
                    AssignedTechnicianName = w.AssignedTechnician != null ? w.AssignedTechnician.FullName : null,
                    CreatedDate = w.CreatedDate,
                    ScheduledDate = w.ScheduledDate,
                    StartedDate = w.StartedDate,
                    CompletedDate = w.CompletedDate,
                    DueDate = w.DueDate,
                    Notes = w.Notes,
                    EstimatedHours = w.EstimatedHours,
                    ActualHours = w.ActualHours,
                    CreatedAt = w.CreatedAt,
                    UpdatedAt = w.UpdatedAt
                })
                .ToListAsync();

            return Ok(workOrders);
        }

        // GET: api/WorkOrders/by-technician/5
        [HttpGet("by-technician/{technicianId}")]
        public async Task<ActionResult<IEnumerable<WorkOrderDto>>> GetWorkOrdersByTechnician(int technicianId)
        {
            var workOrders = await _context.WorkOrders
                .Include(w => w.Aircraft)
                .Include(w => w.Flight)
                .Include(w => w.AssignedTechnician)
                .Where(w => w.AssignedTechnicianId == technicianId)
                .Select(w => new WorkOrderDto
                {
                    Id = w.Id,
                    WorkOrderNumber = w.WorkOrderNumber,
                    Title = w.Title,
                    Description = w.Description,
                    Priority = w.Priority,
                    Status = w.Status,
                    Type = w.Type,
                    AircraftId = w.AircraftId,
                    AircraftRegistration = w.Aircraft.RegistrationNumber,
                    FlightId = w.FlightId,
                    FlightNumber = w.Flight != null ? w.Flight.FlightNumber : null,
                    AssignedTechnicianId = w.AssignedTechnicianId,
                    AssignedTechnicianName = w.AssignedTechnician != null ? w.AssignedTechnician.FullName : null,
                    CreatedDate = w.CreatedDate,
                    ScheduledDate = w.ScheduledDate,
                    StartedDate = w.StartedDate,
                    CompletedDate = w.CompletedDate,
                    DueDate = w.DueDate,
                    Notes = w.Notes,
                    EstimatedHours = w.EstimatedHours,
                    ActualHours = w.ActualHours,
                    CreatedAt = w.CreatedAt,
                    UpdatedAt = w.UpdatedAt
                })
                .ToListAsync();

            return Ok(workOrders);
        }

        // GET: api/WorkOrders/by-status/{status}
        [HttpGet("by-status/{status}")]
        public async Task<ActionResult<IEnumerable<WorkOrderDto>>> GetWorkOrdersByStatus(WorkOrderStatus status)
        {
            var workOrders = await _context.WorkOrders
                .Include(w => w.Aircraft)
                .Include(w => w.Flight)
                .Include(w => w.AssignedTechnician)
                .Where(w => w.Status == status)
                .Select(w => new WorkOrderDto
                {
                    Id = w.Id,
                    WorkOrderNumber = w.WorkOrderNumber,
                    Title = w.Title,
                    Description = w.Description,
                    Priority = w.Priority,
                    Status = w.Status,
                    Type = w.Type,
                    AircraftId = w.AircraftId,
                    AircraftRegistration = w.Aircraft.RegistrationNumber,
                    FlightId = w.FlightId,
                    FlightNumber = w.Flight != null ? w.Flight.FlightNumber : null,
                    AssignedTechnicianId = w.AssignedTechnicianId,
                    AssignedTechnicianName = w.AssignedTechnician != null ? w.AssignedTechnician.FullName : null,
                    CreatedDate = w.CreatedDate,
                    ScheduledDate = w.ScheduledDate,
                    StartedDate = w.StartedDate,
                    CompletedDate = w.CompletedDate,
                    DueDate = w.DueDate,
                    Notes = w.Notes,
                    EstimatedHours = w.EstimatedHours,
                    ActualHours = w.ActualHours,
                    CreatedAt = w.CreatedAt,
                    UpdatedAt = w.UpdatedAt
                })
                .ToListAsync();

            return Ok(workOrders);
        }

        private bool WorkOrderExists(int id)
        {
            return _context.WorkOrders.Any(e => e.Id == id);
        }
    }
}