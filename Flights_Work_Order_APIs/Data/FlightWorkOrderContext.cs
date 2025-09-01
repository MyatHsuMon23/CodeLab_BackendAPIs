using Microsoft.EntityFrameworkCore;
using Flights_Work_Order_APIs.Models;

namespace Flights_Work_Order_APIs.Data
{
    public class FlightWorkOrderContext : DbContext
    {
        public FlightWorkOrderContext(DbContextOptions<FlightWorkOrderContext> options) : base(options)
        {
        }
        
        public DbSet<Aircraft> Aircraft { get; set; }
        public DbSet<Flight> Flights { get; set; }
        public DbSet<WorkOrder> WorkOrders { get; set; }
        public DbSet<Technician> Technicians { get; set; }
        public DbSet<User> Users { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Aircraft configuration
            modelBuilder.Entity<Aircraft>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.RegistrationNumber).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Model).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Manufacturer).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.RegistrationNumber).IsUnique();
            });
            
            // Flight configuration
            modelBuilder.Entity<Flight>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FlightNumber).IsRequired().HasMaxLength(10);
                entity.Property(e => e.Origin).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Destination).IsRequired().HasMaxLength(100);
                
                entity.HasOne(e => e.Aircraft)
                    .WithMany(a => a.Flights)
                    .HasForeignKey(e => e.AircraftId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            
            // Technician configuration
            modelBuilder.Entity<Technician>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.EmployeeId).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Specialization).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.EmployeeId).IsUnique();
            });
            
            // WorkOrder configuration
            modelBuilder.Entity<WorkOrder>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.WorkOrderNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(2000);
                entity.Property(e => e.Notes).HasMaxLength(2000);
                entity.Property(e => e.EstimatedHours).HasPrecision(5, 2);
                entity.Property(e => e.ActualHours).HasPrecision(5, 2);
                entity.HasIndex(e => e.WorkOrderNumber).IsUnique();
                
                entity.HasOne(e => e.Aircraft)
                    .WithMany(a => a.WorkOrders)
                    .HasForeignKey(e => e.AircraftId)
                    .OnDelete(DeleteBehavior.Restrict);
                    
                entity.HasOne(e => e.Flight)
                    .WithMany(f => f.WorkOrders)
                    .HasForeignKey(e => e.FlightId)
                    .OnDelete(DeleteBehavior.Restrict);
                    
                entity.HasOne(e => e.AssignedTechnician)
                    .WithMany(t => t.WorkOrders)
                    .HasForeignKey(e => e.AssignedTechnicianId)
                    .OnDelete(DeleteBehavior.SetNull);
            });
            
            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255);
                entity.Property(e => e.FirstName).HasMaxLength(100);
                entity.Property(e => e.LastName).HasMaxLength(100);
                entity.Property(e => e.Email).HasMaxLength(255);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
            });
        }
        
        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }
        
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return await base.SaveChangesAsync(cancellationToken);
        }
        
        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is Aircraft || e.Entity is Flight || e.Entity is WorkOrder || e.Entity is Technician || e.Entity is User)
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);
                
            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    if (entry.Property("CreatedAt") != null)
                        entry.Property("CreatedAt").CurrentValue = DateTime.UtcNow;
                }
                
                if (entry.Property("UpdatedAt") != null)
                    entry.Property("UpdatedAt").CurrentValue = DateTime.UtcNow;
            }
        }
    }
}