using System.ComponentModel.DataAnnotations;

namespace Flights_Work_Order_APIs.Models
{
    /// <summary>
    /// Flight work order model
    /// </summary>
    public class FlightWorkOrder
    {
        public int Id { get; set; }
        public string WorkOrderNumber { get; set; } = string.Empty;
        public string AircraftRegistration { get; set; } = string.Empty;
        public string TaskDescription { get; set; } = string.Empty;
        public WorkOrderStatus Status { get; set; } = WorkOrderStatus.Open;
        public WorkOrderPriority Priority { get; set; } = WorkOrderPriority.Medium;
        public string AssignedTechnician { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? ScheduledDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string? Notes { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
    }

    /// <summary>
    /// Create work order request model
    /// </summary>
    public class CreateWorkOrderRequest
    {
        [Required]
        [MaxLength(50)]
        public string AircraftRegistration { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string TaskDescription { get; set; } = string.Empty;

        public WorkOrderPriority Priority { get; set; } = WorkOrderPriority.Medium;

        [MaxLength(100)]
        public string AssignedTechnician { get; set; } = string.Empty;

        public DateTime? ScheduledDate { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }
    }

    /// <summary>
    /// Update work order request model
    /// </summary>
    public class UpdateWorkOrderRequest
    {
        [MaxLength(500)]
        public string? TaskDescription { get; set; }

        public WorkOrderStatus? Status { get; set; }

        public WorkOrderPriority? Priority { get; set; }

        [MaxLength(100)]
        public string? AssignedTechnician { get; set; }

        public DateTime? ScheduledDate { get; set; }

        public DateTime? CompletedDate { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }
    }

    /// <summary>
    /// Work order status enumeration
    /// </summary>
    public enum WorkOrderStatus
    {
        Open = 0,
        InProgress = 1,
        Completed = 2,
        Cancelled = 3,
        OnHold = 4
    }

    /// <summary>
    /// Work order priority enumeration
    /// </summary>
    public enum WorkOrderPriority
    {
        Low = 0,
        Medium = 1,
        High = 2,
        Critical = 3
    }

    /// <summary>
    /// Aircraft model
    /// </summary>
    public class Aircraft
    {
        public int Id { get; set; }
        public string Registration { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string Manufacturer { get; set; } = string.Empty;
        public int YearManufactured { get; set; }
        public string SerialNumber { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}