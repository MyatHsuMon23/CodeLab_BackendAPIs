using System.ComponentModel.DataAnnotations;
using Flights_Work_Order_APIs.Models;

namespace Flights_Work_Order_APIs.DTOs
{
    public class TechnicianDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string EmployeeId { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public TechnicianStatus Status { get; set; }
        public DateTime HireDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
    
    public class CreateTechnicianDto
    {
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
        
        public TechnicianStatus Status { get; set; } = TechnicianStatus.Active;
        
        [Required]
        public DateTime HireDate { get; set; }
    }
    
    public class UpdateTechnicianDto
    {
        [StringLength(100)]
        public string? FirstName { get; set; }
        
        [StringLength(100)]
        public string? LastName { get; set; }
        
        [EmailAddress]
        [StringLength(255)]
        public string? Email { get; set; }
        
        [Phone]
        [StringLength(20)]
        public string? PhoneNumber { get; set; }
        
        [StringLength(20)]
        public string? EmployeeId { get; set; }
        
        [StringLength(100)]
        public string? Specialization { get; set; }
        
        public TechnicianStatus? Status { get; set; }
        
        public DateTime? HireDate { get; set; }
    }
}