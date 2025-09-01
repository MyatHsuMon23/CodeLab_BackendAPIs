using System.ComponentModel.DataAnnotations;

namespace Flights_Work_Order_APIs.Models
{
    public class WorkOrder
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string WorkOrderNumber { get; set; } = string.Empty;
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;
        
        public WorkOrderPriority Priority { get; set; }
        
        public WorkOrderStatus Status { get; set; }
        
        public WorkOrderType Type { get; set; }
        
        public int AircraftId { get; set; }
        
        public int? FlightId { get; set; }
        
        public int? AssignedTechnicianId { get; set; }
        
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        
        public DateTime? ScheduledDate { get; set; }
        
        public DateTime? StartedDate { get; set; }
        
        public DateTime? CompletedDate { get; set; }
        
        public DateTime? DueDate { get; set; }
        
        [StringLength(2000)]
        public string? Notes { get; set; }
        
        public decimal? EstimatedHours { get; set; }
        
        public decimal? ActualHours { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual Aircraft Aircraft { get; set; } = null!;
        public virtual Flight? Flight { get; set; }
        public virtual Technician? AssignedTechnician { get; set; }
    }
    
    public enum WorkOrderPriority
    {
        Low = 1,
        Normal = 2,
        High = 3,
        Critical = 4,
        Emergency = 5
    }
    
    public enum WorkOrderStatus
    {
        Created,
        Assigned,
        InProgress,
        OnHold,
        Completed,
        Cancelled,
        Rejected
    }
    
    public enum WorkOrderType
    {
        Preventive,
        Corrective,
        Inspection,
        Repair,
        Overhaul,
        Emergency,
        Upgrade
    }
}