using Flights_Work_Order_APIs.Models;
using Flights_Work_Order_APIs.Controllers;
using Microsoft.EntityFrameworkCore;

namespace Flights_Work_Order_APIs.Data
{
    public static class DataSeeder
    {
        public static async Task SeedDataAsync(FlightWorkOrderContext context)
        {
            // Check if data already exists
            if (await context.Users.AnyAsync())
            {
                return; // Data already seeded
            }

            // Seed Users first
            var users = new List<User>
            {
                new User
                {
                    Username = "admin",
                    PasswordHash = AuthController.GetHashedPassword("password123"),
                    FirstName = "Admin",
                    LastName = "User",
                    Email = "admin@flightworkorder.com",
                    IsActive = true
                },
                new User
                {
                    Username = "manager",
                    PasswordHash = AuthController.GetHashedPassword("manager123"),
                    FirstName = "Manager",
                    LastName = "User",
                    Email = "manager@flightworkorder.com",
                    IsActive = true
                }
            };

            context.Users.AddRange(users);
            await context.SaveChangesAsync();

            // Seed Aircraft
            var aircraft = new List<Aircraft>
            {
                new Aircraft
                {
                    RegistrationNumber = "N123AA",
                    Model = "Boeing 737",
                    Manufacturer = "Boeing",
                    ManufactureYear = 2018,
                    PassengerCapacity = 180,
                    Status = AircraftStatus.Active
                },
                new Aircraft
                {
                    RegistrationNumber = "N456BB",
                    Model = "Airbus A320",
                    Manufacturer = "Airbus",
                    ManufactureYear = 2020,
                    PassengerCapacity = 150,
                    Status = AircraftStatus.Active
                },
                new Aircraft
                {
                    RegistrationNumber = "N789CC",
                    Model = "Boeing 777",
                    Manufacturer = "Boeing",
                    ManufactureYear = 2016,
                    PassengerCapacity = 350,
                    Status = AircraftStatus.Maintenance
                }
            };

            context.Aircraft.AddRange(aircraft);
            await context.SaveChangesAsync();

            // Seed Technicians
            var technicians = new List<Technician>
            {
                new Technician
                {
                    FirstName = "John",
                    LastName = "Smith",
                    Email = "john.smith@airline.com",
                    PhoneNumber = "+1-555-0101",
                    EmployeeId = "TECH001",
                    Specialization = "Avionics",
                    Status = TechnicianStatus.Active,
                    HireDate = DateTime.UtcNow.AddYears(-5)
                },
                new Technician
                {
                    FirstName = "Sarah",
                    LastName = "Johnson",
                    Email = "sarah.johnson@airline.com",
                    PhoneNumber = "+1-555-0102",
                    EmployeeId = "TECH002",
                    Specialization = "Engine Maintenance",
                    Status = TechnicianStatus.Active,
                    HireDate = DateTime.UtcNow.AddYears(-3)
                },
                new Technician
                {
                    FirstName = "Mike",
                    LastName = "Williams",
                    Email = "mike.williams@airline.com",
                    PhoneNumber = "+1-555-0103",
                    EmployeeId = "TECH003",
                    Specialization = "Structural Repair",
                    Status = TechnicianStatus.Active,
                    HireDate = DateTime.UtcNow.AddYears(-2)
                }
            };

            context.Technicians.AddRange(technicians);
            await context.SaveChangesAsync();

            // Seed Flights
            var flights = new List<Flight>
            {
                new Flight
                {
                    FlightNumber = "AA1001",
                    Origin = "New York (JFK)",
                    Destination = "Los Angeles (LAX)",
                    DepartureTime = DateTime.UtcNow.AddDays(1).AddHours(8),
                    ArrivalTime = DateTime.UtcNow.AddDays(1).AddHours(14),
                    Status = FlightStatus.Scheduled,
                    AircraftId = aircraft[0].Id
                },
                new Flight
                {
                    FlightNumber = "AA1002",
                    Origin = "Los Angeles (LAX)",
                    Destination = "New York (JFK)",
                    DepartureTime = DateTime.UtcNow.AddDays(1).AddHours(16),
                    ArrivalTime = DateTime.UtcNow.AddDays(2).AddHours(0),
                    Status = FlightStatus.Scheduled,
                    AircraftId = aircraft[0].Id
                },
                new Flight
                {
                    FlightNumber = "AA2001",
                    Origin = "Chicago (ORD)",
                    Destination = "Miami (MIA)",
                    DepartureTime = DateTime.UtcNow.AddDays(2).AddHours(10),
                    ArrivalTime = DateTime.UtcNow.AddDays(2).AddHours(15),
                    Status = FlightStatus.Scheduled,
                    AircraftId = aircraft[1].Id
                },
                new Flight
                {
                    FlightNumber = "AA3001",
                    Origin = "Dallas (DFW)",
                    Destination = "London (LHR)",
                    DepartureTime = DateTime.UtcNow.AddDays(-1).AddHours(20),
                    ArrivalTime = DateTime.UtcNow.AddHours(10),
                    Status = FlightStatus.Landed,
                    AircraftId = aircraft[2].Id
                }
            };

            context.Flights.AddRange(flights);
            await context.SaveChangesAsync();

            // Seed Work Orders
            var workOrders = new List<WorkOrder>
            {
                new WorkOrder
                {
                    WorkOrderNumber = "WO-2024-001",
                    Title = "Routine Engine Inspection",
                    Description = "Perform routine engine inspection as per maintenance schedule. Check for wear and tear, fluid levels, and overall engine condition.",
                    Priority = WorkOrderPriority.Normal,
                    Status = WorkOrderStatus.Assigned,
                    Type = WorkOrderType.Inspection,
                    AircraftId = aircraft[0].Id,
                    AssignedTechnicianId = technicians[1].Id,
                    ScheduledDate = DateTime.UtcNow.AddDays(3),
                    DueDate = DateTime.UtcNow.AddDays(7),
                    EstimatedHours = 4.0m,
                    Notes = "Standard inspection procedure. All tools and parts available."
                },
                new WorkOrder
                {
                    WorkOrderNumber = "WO-2024-002",
                    Title = "Avionics System Update",
                    Description = "Update navigation system software to latest version. Test all avionics systems after update.",
                    Priority = WorkOrderPriority.High,
                    Status = WorkOrderStatus.InProgress,
                    Type = WorkOrderType.Upgrade,
                    AircraftId = aircraft[1].Id,
                    AssignedTechnicianId = technicians[0].Id,
                    ScheduledDate = DateTime.UtcNow.AddDays(1),
                    StartedDate = DateTime.UtcNow,
                    DueDate = DateTime.UtcNow.AddDays(2),
                    EstimatedHours = 6.0m,
                    ActualHours = 2.5m,
                    Notes = "Update in progress. No issues encountered so far."
                },
                new WorkOrder
                {
                    WorkOrderNumber = "WO-2024-003",
                    Title = "Emergency Landing Gear Repair",
                    Description = "Landing gear hydraulic system malfunction detected. Immediate repair required before next flight.",
                    Priority = WorkOrderPriority.Emergency,
                    Status = WorkOrderStatus.Created,
                    Type = WorkOrderType.Emergency,
                    AircraftId = aircraft[2].Id,
                    FlightId = flights[3].Id,
                    DueDate = DateTime.UtcNow.AddHours(24),
                    EstimatedHours = 8.0m,
                    Notes = "URGENT: Aircraft grounded until repair completed. Spare parts ordered."
                },
                new WorkOrder
                {
                    WorkOrderNumber = "WO-2024-004",
                    Title = "Interior Cleaning and Maintenance",
                    Description = "Deep cleaning of passenger cabin, seat repairs, and overhead bin adjustments.",
                    Priority = WorkOrderPriority.Low,
                    Status = WorkOrderStatus.Completed,
                    Type = WorkOrderType.Preventive,
                    AircraftId = aircraft[0].Id,
                    AssignedTechnicianId = technicians[2].Id,
                    ScheduledDate = DateTime.UtcNow.AddDays(-2),
                    StartedDate = DateTime.UtcNow.AddDays(-2),
                    CompletedDate = DateTime.UtcNow.AddDays(-1),
                    DueDate = DateTime.UtcNow.AddDays(-1),
                    EstimatedHours = 3.0m,
                    ActualHours = 3.5m,
                    Notes = "Completed on schedule. Minor seat repairs performed."
                }
            };

            context.WorkOrders.AddRange(workOrders);
            await context.SaveChangesAsync();
        }
    }
}