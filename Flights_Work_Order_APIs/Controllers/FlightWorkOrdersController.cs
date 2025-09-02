using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Flights_Work_Order_APIs.Models;
using System.Security.Claims;

namespace Flights_Work_Order_APIs.Controllers
{
    /// <summary>
    /// Flight Work Orders management controller
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Requires JWT authentication
    public class WorkOrdersController : ControllerBase
    {
        private readonly ILogger<WorkOrdersController> _logger;
        
        // In-memory storage for demo purposes (replace with database in production)
        private static List<FlightWorkOrder> _workOrders = new List<FlightWorkOrder>();
        private static int _nextId = 1;

        public WorkOrdersController(ILogger<WorkOrdersController> logger)
        {
            _logger = logger;
            
            // Initialize with sample data if empty
            if (!_workOrders.Any())
            {
                InitializeSampleData();
            }
        }

        /// <summary>
        /// Get all work orders
        /// </summary>
        [HttpGet]
        public ActionResult<ApiResponse<IEnumerable<FlightWorkOrder>>> GetWorkOrders(
            [FromQuery] WorkOrderStatus? status = null,
            [FromQuery] string? aircraftRegistration = null)
        {
            try
            {
                var query = _workOrders.AsQueryable();

                if (status.HasValue)
                    query = query.Where(w => w.Status == status.Value);

                if (!string.IsNullOrEmpty(aircraftRegistration))
                    query = query.Where(w => w.AircraftRegistration.Contains(aircraftRegistration, StringComparison.OrdinalIgnoreCase));

                var workOrders = query.OrderByDescending(w => w.CreatedDate).ToList();

                return Ok(ApiResponse<IEnumerable<FlightWorkOrder>>.CreateSuccess(workOrders, "Work orders retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving work orders");
                return StatusCode(500, ApiResponse<IEnumerable<FlightWorkOrder>>.CreateError("Failed to retrieve work orders"));
            }
        }

        /// <summary>
        /// Get work order by ID
        /// </summary>
        [HttpGet("{id}")]
        public ActionResult<ApiResponse<FlightWorkOrder>> GetWorkOrder(int id)
        {
            try
            {
                var workOrder = _workOrders.FirstOrDefault(w => w.Id == id);
                if (workOrder == null)
                {
                    return NotFound(ApiResponse<FlightWorkOrder>.CreateError("Work order not found"));
                }

                return Ok(ApiResponse<FlightWorkOrder>.CreateSuccess(workOrder, "Work order retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving work order {Id}", id);
                return StatusCode(500, ApiResponse<FlightWorkOrder>.CreateError("Failed to retrieve work order"));
            }
        }

        /// <summary>
        /// Create new work order
        /// </summary>
        [HttpPost]
        public ActionResult<ApiResponse<FlightWorkOrder>> CreateWorkOrder([FromBody] CreateWorkOrderRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<FlightWorkOrder>.CreateError("Invalid request data"));
                }

                var currentUser = User.Identity?.Name ?? "Unknown";
                var workOrderNumber = GenerateWorkOrderNumber();

                var workOrder = new FlightWorkOrder
                {
                    Id = _nextId++,
                    WorkOrderNumber = workOrderNumber,
                    AircraftRegistration = request.AircraftRegistration,
                    TaskDescription = request.TaskDescription,
                    Priority = request.Priority,
                    AssignedTechnician = request.AssignedTechnician,
                    ScheduledDate = request.ScheduledDate,
                    Notes = request.Notes,
                    CreatedBy = currentUser,
                    Status = WorkOrderStatus.Open
                };

                _workOrders.Add(workOrder);

                _logger.LogInformation("Work order {WorkOrderNumber} created by {User}", workOrderNumber, currentUser);
                return CreatedAtAction(nameof(GetWorkOrder), new { id = workOrder.Id }, 
                    ApiResponse<FlightWorkOrder>.CreateSuccess(workOrder, "Work order created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating work order");
                return StatusCode(500, ApiResponse<FlightWorkOrder>.CreateError("Failed to create work order"));
            }
        }

        /// <summary>
        /// Update work order
        /// </summary>
        [HttpPut("{id}")]
        public ActionResult<ApiResponse<FlightWorkOrder>> UpdateWorkOrder(int id, [FromBody] UpdateWorkOrderRequest request)
        {
            try
            {
                var workOrder = _workOrders.FirstOrDefault(w => w.Id == id);
                if (workOrder == null)
                {
                    return NotFound(ApiResponse<FlightWorkOrder>.CreateError("Work order not found"));
                }

                // Update fields if provided
                if (!string.IsNullOrEmpty(request.TaskDescription))
                    workOrder.TaskDescription = request.TaskDescription;

                if (request.Status.HasValue)
                {
                    workOrder.Status = request.Status.Value;
                    if (request.Status.Value == WorkOrderStatus.Completed && !workOrder.CompletedDate.HasValue)
                        workOrder.CompletedDate = DateTime.UtcNow;
                }

                if (request.Priority.HasValue)
                    workOrder.Priority = request.Priority.Value;

                if (!string.IsNullOrEmpty(request.AssignedTechnician))
                    workOrder.AssignedTechnician = request.AssignedTechnician;

                if (request.ScheduledDate.HasValue)
                    workOrder.ScheduledDate = request.ScheduledDate;

                if (request.CompletedDate.HasValue)
                    workOrder.CompletedDate = request.CompletedDate;

                if (!string.IsNullOrEmpty(request.Notes))
                    workOrder.Notes = request.Notes;

                var currentUser = User.Identity?.Name ?? "Unknown";
                _logger.LogInformation("Work order {WorkOrderNumber} updated by {User}", workOrder.WorkOrderNumber, currentUser);

                return Ok(ApiResponse<FlightWorkOrder>.CreateSuccess(workOrder, "Work order updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating work order {Id}", id);
                return StatusCode(500, ApiResponse<FlightWorkOrder>.CreateError("Failed to update work order"));
            }
        }

        /// <summary>
        /// Delete work order
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Supervisor")] // Only admins and supervisors can delete
        public ActionResult<ApiResponse> DeleteWorkOrder(int id)
        {
            try
            {
                var workOrder = _workOrders.FirstOrDefault(w => w.Id == id);
                if (workOrder == null)
                {
                    return NotFound(ApiResponse.CreateError("Work order not found"));
                }

                _workOrders.Remove(workOrder);

                var currentUser = User.Identity?.Name ?? "Unknown";
                _logger.LogInformation("Work order {WorkOrderNumber} deleted by {User}", workOrder.WorkOrderNumber, currentUser);

                return Ok(ApiResponse.CreateSuccess("Work order deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting work order {Id}", id);
                return StatusCode(500, ApiResponse.CreateError("Failed to delete work order"));
            }
        }

        /// <summary>
        /// Get work order statistics
        /// </summary>
        [HttpGet("statistics")]
        public ActionResult<ApiResponse<object>> GetStatistics()
        {
            try
            {
                var stats = new
                {
                    TotalWorkOrders = _workOrders.Count,
                    OpenWorkOrders = _workOrders.Count(w => w.Status == WorkOrderStatus.Open),
                    InProgressWorkOrders = _workOrders.Count(w => w.Status == WorkOrderStatus.InProgress),
                    CompletedWorkOrders = _workOrders.Count(w => w.Status == WorkOrderStatus.Completed),
                    HighPriorityWorkOrders = _workOrders.Count(w => w.Priority == WorkOrderPriority.High || w.Priority == WorkOrderPriority.Critical),
                    OverdueWorkOrders = _workOrders.Count(w => w.ScheduledDate.HasValue && w.ScheduledDate < DateTime.UtcNow && w.Status != WorkOrderStatus.Completed)
                };

                return Ok(ApiResponse<object>.CreateSuccess(stats, "Statistics retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving statistics");
                return StatusCode(500, ApiResponse<object>.CreateError("Failed to retrieve statistics"));
            }
        }

        private string GenerateWorkOrderNumber()
        {
            return $"WO-{DateTime.UtcNow:yyyyMMdd}-{_nextId:000}";
        }

        private void InitializeSampleData()
        {
            _workOrders.AddRange(new[]
            {
                new FlightWorkOrder
                {
                    Id = _nextId++,
                    WorkOrderNumber = GenerateWorkOrderNumber(),
                    AircraftRegistration = "N123AB",
                    TaskDescription = "Engine inspection and oil change",
                    Status = WorkOrderStatus.Open,
                    Priority = WorkOrderPriority.High,
                    AssignedTechnician = "John Smith",
                    CreatedBy = "admin",
                    ScheduledDate = DateTime.UtcNow.AddDays(2)
                },
                new FlightWorkOrder
                {
                    Id = _nextId++,
                    WorkOrderNumber = GenerateWorkOrderNumber(),
                    AircraftRegistration = "N456CD",
                    TaskDescription = "Landing gear maintenance check",
                    Status = WorkOrderStatus.InProgress,
                    Priority = WorkOrderPriority.Medium,
                    AssignedTechnician = "Jane Doe",
                    CreatedBy = "supervisor",
                    ScheduledDate = DateTime.UtcNow.AddDays(1)
                }
            });
        }
    }
}