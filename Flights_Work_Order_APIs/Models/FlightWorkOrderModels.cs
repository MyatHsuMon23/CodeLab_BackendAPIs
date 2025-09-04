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

    /// <summary>
    /// Flight model
    /// </summary>
    public class Flight
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string FlightNumber { get; set; } = string.Empty;

        public DateTime ScheduledArrivalTimeUtc { get; set; }

        [Required]
        [MaxLength(10)]
        public string OriginAirport { get; set; } = string.Empty;

        [Required]
        [MaxLength(10)]
        public string DestinationAirport { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property for flight work orders
        public virtual ICollection<FlightWorkOrderSubmission> WorkOrderSubmissions { get; set; } = new List<FlightWorkOrderSubmission>();
    }

    /// <summary>
    /// Flight work order submission model - links flights to work order commands
    /// </summary>
    public class FlightWorkOrderSubmission
    {
        public int Id { get; set; }

        public int FlightId { get; set; }
        public virtual Flight Flight { get; set; } = null!;

        [Required]
        [MaxLength(500)]
        public string CommandString { get; set; } = string.Empty;

        [Required]
        [MaxLength(1000)]
        public string ParsedCommandsJson { get; set; } = string.Empty;

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(100)]
        public string SubmittedBy { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Notes { get; set; }
    }

    /// <summary>
    /// Parsed flight command model for command validation and display
    /// </summary>
    public class FlightCommand
    {
        public string Type { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string DisplayText { get; set; } = string.Empty;
        public bool IsValid { get; set; } = true;
        public string? ErrorMessage { get; set; }
    }

    /// <summary>
    /// Command submission request model
    /// </summary>
    public class SubmitFlightCommandRequest
    {
        [Required]
        public int FlightId { get; set; }

        [Required]
        [MaxLength(500)]
        public string CommandString { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Notes { get; set; }
    }

    /// <summary>
    /// Flight import request model
    /// </summary>
    public class FlightImportRequest
    {
        [Required]
        [MaxLength(20)]
        public string FlightNumber { get; set; } = string.Empty;

        public DateTime ScheduledArrivalTimeUtc { get; set; }

        [Required]
        [MaxLength(10)]
        public string OriginAirport { get; set; } = string.Empty;

        [Required]
        [MaxLength(10)]
        public string DestinationAirport { get; set; } = string.Empty;
    }

    /// <summary>
    /// Create flight work order request model
    /// </summary>
    public class CreateFlightWorkOrderRequest
    {
        [Required]
        [MaxLength(50)]
        public string AircraftRegistration { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string TaskDescription { get; set; } = string.Empty;

        public WorkOrderPriority Priority { get; set; } = WorkOrderPriority.Medium;

        [MaxLength(100)]
        public string? AssignedTechnician { get; set; }

        public DateTime? ScheduledDate { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }
    }

    /// <summary>
    /// Flight detail DTO for comprehensive flight information
    /// </summary>
    public class FlightDetailDto
    {
        public int Id { get; set; }
        public string FlightNumber { get; set; } = string.Empty;
        public DateTime ScheduledArrivalTimeUtc { get; set; }
        public string OriginAirport { get; set; } = string.Empty;
        public string DestinationAirport { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<WorkOrderSubmissionDto> WorkOrderSubmissions { get; set; } = new List<WorkOrderSubmissionDto>();
    }

    /// <summary>
    /// Work order submission DTO for clean frontend consumption
    /// </summary>
    public class WorkOrderSubmissionDto
    {
        public int Id { get; set; }
        public string CommandString { get; set; } = string.Empty;
        public List<FlightCommand> ParsedCommands { get; set; } = new List<FlightCommand>();
        public string HumanReadableCommands { get; set; } = string.Empty;
        public DateTime SubmittedAt { get; set; }
        public string SubmittedBy { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }

    /// <summary>
    /// Flight work order DTO for clean frontend consumption
    /// </summary>
    public class FlightWorkOrderDto
    {
        public int Id { get; set; }
        public string WorkOrderNumber { get; set; } = string.Empty;
        public string AircraftRegistration { get; set; } = string.Empty;
        public string TaskDescription { get; set; } = string.Empty;
        public WorkOrderStatus Status { get; set; }
        public WorkOrderPriority Priority { get; set; }
        public string AssignedTechnician { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime? ScheduledDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string? Notes { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
    }

    /// <summary>
    /// Comprehensive flight DTO that includes work orders and command submissions
    /// </summary>
    public class FlightWithWorkOrdersDto
    {
        public int Id { get; set; }
        public string FlightNumber { get; set; } = string.Empty;
        public DateTime ScheduledArrivalTimeUtc { get; set; }
        public string OriginAirport { get; set; } = string.Empty;
        public string DestinationAirport { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<FlightWorkOrderDto> WorkOrders { get; set; } = new List<FlightWorkOrderDto>();
        public List<WorkOrderSubmissionDto> CommandSubmissions { get; set; } = new List<WorkOrderSubmissionDto>();
    }
}