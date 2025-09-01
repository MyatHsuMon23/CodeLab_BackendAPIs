using System.ComponentModel.DataAnnotations;

namespace Flights_Work_Order_APIs.DTOs
{
    public class WorkOrderCommandDto
    {
        [Required]
        public string CommandString { get; set; } = string.Empty;
        
        public int FlightId { get; set; }
    }

    public class ParsedWorkOrderCommandDto
    {
        public int? CheckInMinutes { get; set; }
        public int? BaggageMinutes { get; set; }
        public int? CleaningMinutes { get; set; }
        public int? JetBridgeAngle { get; set; }
        public List<string> ValidationErrors { get; set; } = new List<string>();
        public bool IsValid => ValidationErrors.Count == 0;
        public string OriginalCommand { get; set; } = string.Empty;
    }

    public class WorkOrderCommandResultDto
    {
        public ParsedWorkOrderCommandDto ParsedCommand { get; set; } = new();
        public int FlightId { get; set; }
        public string FlightNumber { get; set; } = string.Empty;
        public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
    }
}