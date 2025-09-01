using System.ComponentModel.DataAnnotations;
using Flights_Work_Order_APIs.Models;

namespace Flights_Work_Order_APIs.DTOs
{
    public class FlightDto
    {
        public int Id { get; set; }
        public string FlightNumber { get; set; } = string.Empty;
        public string Origin { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public FlightStatus Status { get; set; }
        public int AircraftId { get; set; }
        public string? AircraftRegistration { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
    
    public class CreateFlightDto
    {
        [Required]
        [StringLength(10)]
        public string FlightNumber { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string Origin { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string Destination { get; set; } = string.Empty;
        
        [Required]
        public DateTime DepartureTime { get; set; }
        
        [Required]
        public DateTime ArrivalTime { get; set; }
        
        public FlightStatus Status { get; set; } = FlightStatus.Scheduled;
        
        [Required]
        public int AircraftId { get; set; }
    }
    
    public class UpdateFlightDto
    {
        [StringLength(10)]
        public string? FlightNumber { get; set; }
        
        [StringLength(100)]
        public string? Origin { get; set; }
        
        [StringLength(100)]
        public string? Destination { get; set; }
        
        public DateTime? DepartureTime { get; set; }
        
        public DateTime? ArrivalTime { get; set; }
        
        public FlightStatus? Status { get; set; }
        
        public int? AircraftId { get; set; }
    }
}