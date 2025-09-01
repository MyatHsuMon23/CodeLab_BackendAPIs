# Flight Work Orders Backend APIs

A comprehensive .NET 8 Web API for managing flights and work orders for aircraft maintenance operations.

## Overview

This code lab demonstrates building a complete backend API system with:
- **Flight Management** - Track flight schedules, routes, and aircraft assignments
- **Aircraft Management** - Manage aircraft fleet with detailed specifications
- **Work Order Management** - Handle maintenance tasks and assignments
- **Technician Management** - Manage maintenance personnel and specializations

## Features

### Core Entities
- **Aircraft**: Registration numbers, models, manufacturers, capacity, and status
- **Flights**: Flight numbers, origins, destinations, schedules, and aircraft assignments  
- **Work Orders**: Maintenance tasks with priorities, types, assignments, and tracking
- **Technicians**: Maintenance personnel with specializations and status

### API Capabilities
- Full CRUD operations for all entities
- Advanced filtering endpoints (by aircraft, technician, status, specialization)
- Comprehensive data validation and error handling
- Entity relationships with proper foreign key constraints
- Automated timestamp tracking (CreatedAt, UpdatedAt)

### Technology Stack
- **.NET 8** - Latest LTS framework
- **ASP.NET Core Web API** - RESTful API framework
- **Entity Framework Core** - ORM with In-Memory database
- **Swagger/OpenAPI** - Interactive API documentation
- **Data Annotations** - Model validation

## Quick Start

### Prerequisites
- .NET 8 SDK
- Any IDE (Visual Studio, VS Code, Rider)

### Running the Application

1. **Clone and Navigate**
   ```bash
   git clone <repository-url>
   cd Flights-WorkOrders_BackendAPIs/Flights_Work_Order_APIs
   ```

2. **Restore and Build**
   ```bash
   dotnet restore
   dotnet build
   ```

3. **Run the Application**
   ```bash
   dotnet run
   ```

4. **Access Swagger UI**
   - Open browser to `http://localhost:5112`
   - Interactive API documentation with test capabilities

## API Endpoints

### Aircraft Management
- `GET /api/Aircraft` - List all aircraft
- `POST /api/Aircraft` - Create new aircraft
- `GET /api/Aircraft/{id}` - Get specific aircraft
- `PUT /api/Aircraft/{id}` - Update aircraft
- `DELETE /api/Aircraft/{id}` - Delete aircraft

### Flight Management
- `GET /api/Flights` - List all flights
- `POST /api/Flights` - Create new flight
- `GET /api/Flights/{id}` - Get specific flight
- `PUT /api/Flights/{id}` - Update flight
- `DELETE /api/Flights/{id}` - Delete flight
- `GET /api/Flights/by-aircraft/{aircraftId}` - Get flights by aircraft

### Technician Management
- `GET /api/Technicians` - List all technicians
- `POST /api/Technicians` - Create new technician
- `GET /api/Technicians/{id}` - Get specific technician
- `PUT /api/Technicians/{id}` - Update technician
- `DELETE /api/Technicians/{id}` - Delete technician
- `GET /api/Technicians/active` - Get active technicians
- `GET /api/Technicians/by-specialization/{specialization}` - Filter by specialization

### Work Order Management
- `GET /api/WorkOrders` - List all work orders
- `POST /api/WorkOrders` - Create new work order
- `GET /api/WorkOrders/{id}` - Get specific work order
- `PUT /api/WorkOrders/{id}` - Update work order
- `DELETE /api/WorkOrders/{id}` - Delete work order
- `GET /api/WorkOrders/by-aircraft/{aircraftId}` - Get work orders by aircraft
- `GET /api/WorkOrders/by-technician/{technicianId}` - Get work orders by technician
- `GET /api/WorkOrders/by-status/{status}` - Filter by status

## Sample Data

The application automatically seeds with sample data including:
- 3 Aircraft (Boeing 737, Airbus A320, Boeing 777)
- 3 Technicians with different specializations
- 4 Flights with various routes and schedules
- 4 Work Orders demonstrating different priorities and statuses

## Data Models

### Aircraft Status
- Active, Maintenance, Retired, OutOfService

### Flight Status  
- Scheduled, Boarding, Departed, InFlight, Landed, Cancelled, Delayed

### Work Order Priority
- Low, Normal, High, Critical, Emergency

### Work Order Status
- Created, Assigned, InProgress, OnHold, Completed, Cancelled, Rejected

### Work Order Types
- Preventive, Corrective, Inspection, Repair, Overhaul, Emergency, Upgrade

## Architecture Highlights

### Clean Architecture
- **Models**: Domain entities with validation attributes
- **DTOs**: Separate data transfer objects for API contracts
- **Controllers**: RESTful endpoints with proper HTTP status codes
- **Data Layer**: Entity Framework context with relationships

### Best Practices
- Input validation with data annotations
- Proper error handling and meaningful error messages
- Separation of concerns with DTOs
- Consistent API design patterns
- Comprehensive documentation

### Database Design
- Proper foreign key relationships
- Unique constraints on business keys
- Cascade delete prevention for data integrity
- Automatic timestamp management

## Learning Objectives

This code lab demonstrates:
1. **RESTful API Design** - Proper HTTP verbs, status codes, and resource modeling
2. **Entity Framework Core** - Code-first approach, relationships, and querying
3. **Data Validation** - Input validation and business rule enforcement
4. **Error Handling** - Graceful error responses and user-friendly messages
5. **API Documentation** - Swagger integration for interactive documentation
6. **Clean Code** - Separation of concerns and maintainable architecture

## Next Steps

Potential enhancements for learning:
- Add authentication and authorization
- Implement logging and monitoring
- Add unit and integration tests
- Connect to a real database (SQL Server, PostgreSQL)
- Add caching for performance optimization
- Implement background services for automated tasks

## License

This project is for educational purposes as part of a comprehensive backend API development code lab.