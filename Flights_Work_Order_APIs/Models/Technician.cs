using System.ComponentModel.DataAnnotations;

namespace Flights_Work_Order_APIs.Models
{
    public class Technician
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;
        
        [Phone]
        [StringLength(20)]
        public string? PhoneNumber { get; set; }
        
        [Required]
        [StringLength(20)]
        public string EmployeeId { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string Specialization { get; set; } = string.Empty;
        
        public TechnicianStatus Status { get; set; }
        
        public DateTime HireDate { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual ICollection<WorkOrder> WorkOrders { get; set; } = new List<WorkOrder>();
        
        // Full name property for convenience
        public string FullName => $"{FirstName} {LastName}";
    }
    
    public enum TechnicianStatus
    {
        Active,
        OnLeave,
        Inactive,
        Training
    }
}