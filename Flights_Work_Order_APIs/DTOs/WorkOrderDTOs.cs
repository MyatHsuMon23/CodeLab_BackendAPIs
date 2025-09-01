using System.ComponentModel.DataAnnotations;
using Flights_Work_Order_APIs.Models;

namespace Flights_Work_Order_APIs.DTOs
{
    public class WorkOrderDto
    {
        public int Id { get; set; }
        public string WorkOrderNumber { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public WorkOrderPriority Priority { get; set; }
        public WorkOrderStatus Status { get; set; }
        public WorkOrderType Type { get; set; }
        public int AircraftId { get; set; }
        public string? AircraftRegistration { get; set; }
        public int? FlightId { get; set; }
        public string? FlightNumber { get; set; }
        public int? AssignedTechnicianId { get; set; }
        public string? AssignedTechnicianName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ScheduledDate { get; set; }
        public DateTime? StartedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string? Notes { get; set; }
        public decimal? EstimatedHours { get; set; }
        public decimal? ActualHours { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
    
    public class CreateWorkOrderDto
    {
        [Required]
        [StringLength(50)]
        public string WorkOrderNumber { get; set; } = string.Empty;
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;
        
        public WorkOrderPriority Priority { get; set; } = WorkOrderPriority.Normal;
        
        public WorkOrderStatus Status { get; set; } = WorkOrderStatus.Created;
        
        public WorkOrderType Type { get; set; } = WorkOrderType.Preventive;
        
        [Required]
        public int AircraftId { get; set; }
        
        public int? FlightId { get; set; }
        
        public int? AssignedTechnicianId { get; set; }
        
        public DateTime? ScheduledDate { get; set; }
        
        public DateTime? DueDate { get; set; }
        
        [StringLength(2000)]
        public string? Notes { get; set; }
        
        [Range(0.1, 1000)]
        public decimal? EstimatedHours { get; set; }
    }
    
    public class UpdateWorkOrderDto
    {
        [StringLength(50)]
        public string? WorkOrderNumber { get; set; }
        
        [StringLength(200)]
        public string? Title { get; set; }
        
        [StringLength(2000)]
        public string? Description { get; set; }
        
        public WorkOrderPriority? Priority { get; set; }
        
        public WorkOrderStatus? Status { get; set; }
        
        public WorkOrderType? Type { get; set; }
        
        public int? AircraftId { get; set; }
        
        public int? FlightId { get; set; }
        
        public int? AssignedTechnicianId { get; set; }
        
        public DateTime? ScheduledDate { get; set; }
        
        public DateTime? StartedDate { get; set; }
        
        public DateTime? CompletedDate { get; set; }
        
        public DateTime? DueDate { get; set; }
        
        [StringLength(2000)]
        public string? Notes { get; set; }
        
        [Range(0.1, 1000)]
        public decimal? EstimatedHours { get; set; }
        
        [Range(0.1, 1000)]
        public decimal? ActualHours { get; set; }
    }
}