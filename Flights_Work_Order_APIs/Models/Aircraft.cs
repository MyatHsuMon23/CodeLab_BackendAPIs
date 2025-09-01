using System.ComponentModel.DataAnnotations;

namespace Flights_Work_Order_APIs.Models
{
    public class Aircraft
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(20)]
        public string RegistrationNumber { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string Model { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string Manufacturer { get; set; } = string.Empty;
        
        public int ManufactureYear { get; set; }
        
        public int PassengerCapacity { get; set; }
        
        public AircraftStatus Status { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual ICollection<Flight> Flights { get; set; } = new List<Flight>();
        public virtual ICollection<WorkOrder> WorkOrders { get; set; } = new List<WorkOrder>();
    }
    
    public enum AircraftStatus
    {
        Active,
        Maintenance,
        Retired,
        OutOfService
    }
}