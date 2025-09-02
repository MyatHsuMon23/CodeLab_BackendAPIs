using Microsoft.EntityFrameworkCore;
using Flights_Work_Order_APIs.Models;

namespace Flights_Work_Order_APIs.Data
{
    /// <summary>
    /// Database context for flight management system
    /// </summary>
    public class FlightDbContext : DbContext
    {
        public FlightDbContext(DbContextOptions<FlightDbContext> options) : base(options)
        {
        }

        public DbSet<Flight> Flights { get; set; }
        public DbSet<FlightWorkOrder> WorkOrders { get; set; }
        public DbSet<FlightWorkOrderSubmission> FlightWorkOrderSubmissions { get; set; }
        public DbSet<Aircraft> Aircraft { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Flight entity
            modelBuilder.Entity<Flight>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FlightNumber).IsRequired().HasMaxLength(20);
                entity.Property(e => e.OriginAirport).IsRequired().HasMaxLength(10);
                entity.Property(e => e.DestinationAirport).IsRequired().HasMaxLength(10);
                entity.HasIndex(e => e.FlightNumber);
            });

            // Configure FlightWorkOrder entity
            modelBuilder.Entity<FlightWorkOrder>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.WorkOrderNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.AircraftRegistration).IsRequired().HasMaxLength(50);
                entity.Property(e => e.TaskDescription).IsRequired().HasMaxLength(500);
                entity.Property(e => e.AssignedTechnician).HasMaxLength(100);
                entity.Property(e => e.Notes).HasMaxLength(1000);
                entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(100);
            });

            // Configure FlightWorkOrderSubmission entity
            modelBuilder.Entity<FlightWorkOrderSubmission>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CommandString).IsRequired().HasMaxLength(500);
                entity.Property(e => e.ParsedCommandsJson).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.SubmittedBy).HasMaxLength(100);
                entity.Property(e => e.Notes).HasMaxLength(1000);
                
                // Configure relationship with Flight
                entity.HasOne(e => e.Flight)
                      .WithMany(f => f.WorkOrderSubmissions)
                      .HasForeignKey(e => e.FlightId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Aircraft entity
            modelBuilder.Entity<Aircraft>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Registration).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Model).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Manufacturer).IsRequired().HasMaxLength(100);
                entity.Property(e => e.SerialNumber).IsRequired().HasMaxLength(100);
            });
        }
    }
}