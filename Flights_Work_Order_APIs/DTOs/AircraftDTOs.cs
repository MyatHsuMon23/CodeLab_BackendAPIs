using System.ComponentModel.DataAnnotations;
using Flights_Work_Order_APIs.Models;

namespace Flights_Work_Order_APIs.DTOs
{
    public class AircraftDto
    {
        public int Id { get; set; }
        public string RegistrationNumber { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string Manufacturer { get; set; } = string.Empty;
        public int ManufactureYear { get; set; }
        public int PassengerCapacity { get; set; }
        public AircraftStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
    
    public class CreateAircraftDto
    {
        [Required]
        [StringLength(20)]
        public string RegistrationNumber { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string Model { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string Manufacturer { get; set; } = string.Empty;
        
        [Range(1900, 2100)]
        public int ManufactureYear { get; set; }
        
        [Range(1, 1000)]
        public int PassengerCapacity { get; set; }
        
        public AircraftStatus Status { get; set; } = AircraftStatus.Active;
    }
    
    public class UpdateAircraftDto
    {
        [StringLength(20)]
        public string? RegistrationNumber { get; set; }
        
        [StringLength(50)]
        public string? Model { get; set; }
        
        [StringLength(50)]
        public string? Manufacturer { get; set; }
        
        [Range(1900, 2100)]
        public int? ManufactureYear { get; set; }
        
        [Range(1, 1000)]
        public int? PassengerCapacity { get; set; }
        
        public AircraftStatus? Status { get; set; }
    }
}