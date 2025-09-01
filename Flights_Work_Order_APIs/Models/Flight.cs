using System.ComponentModel.DataAnnotations;

namespace Flights_Work_Order_APIs.Models
{
    public class Flight
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(10)]
        public string FlightNumber { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string Origin { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string Destination { get; set; } = string.Empty;
        
        public DateTime DepartureTime { get; set; }
        
        public DateTime ArrivalTime { get; set; }
        
        public FlightStatus Status { get; set; }
        
        public int AircraftId { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual Aircraft Aircraft { get; set; } = null!;
        public virtual ICollection<WorkOrder> WorkOrders { get; set; } = new List<WorkOrder>();
    }
    
    public enum FlightStatus
    {
        Scheduled,
        Boarding,
        Departed,
        InFlight,
        Landed,
        Cancelled,
        Delayed
    }
}