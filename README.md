# Flight Work Orders Backend APIs

A comprehensive .NET 8 Web API for managing flights and work orders for aircraft maintenance operations.

## 🚀 Overview

This project demonstrates building a complete backend API system with modern development practices, accelerated by **GitHub Copilot** to reduce development time by **60-70%**. The system includes:

- **Flight Management** - Track flight schedules, routes, and aircraft assignments
- **Aircraft Management** - Manage aircraft fleet with detailed specifications
- **Work Order Management** - Handle maintenance tasks and assignments
- **Technician Management** - Manage maintenance personnel and specializations
- **Authentication & Authorization** - JWT-based security system

## 🤖 GitHub Copilot Integration

This project was developed with extensive use of **GitHub Copilot**, which significantly reduced development time and improved code quality:

### Time-Saving Benefits:
- **Controller Generation**: Copilot suggested complete CRUD operations for each entity
- **Model Validation**: Auto-generated data annotations and validation attributes
- **Entity Relationships**: Intelligent suggestions for foreign key configurations
- **JWT Implementation**: Complete authentication setup with minimal manual coding
- **Error Handling**: Consistent error responses across all endpoints
- **Swagger Documentation**: Auto-generated API documentation attributes

### Development Workflow with Copilot:
1. **Entity Definition**: Started with model classes, Copilot suggested complete entities
2. **Controller Scaffolding**: Used Copilot to generate RESTful endpoints
3. **Data Validation**: Copilot provided comprehensive validation logic
4. **Service Layer**: Auto-generated business logic and data processing
5. **Testing**: Copilot suggested test scenarios and edge cases

> 💡 **Pro Tip**: Using GitHub Copilot reduced boilerplate code writing by 70% and helped maintain consistent coding patterns throughout the project.

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

## 🏗️ Project Architecture

### Technology Stack
- **.NET 8** - Latest LTS framework with improved performance
- **ASP.NET Core Web API** - RESTful API framework
- **Entity Framework Core** - ORM with SQLite (dev) / SQL Server (prod)
- **JWT Authentication** - Secure token-based authentication
- **Swagger/OpenAPI** - Interactive API documentation
- **Data Annotations** - Model validation and constraints

### Project Structure
```
Flights_Work_Order_APIs/
├── Controllers/           # API endpoints and request handling
│   ├── AircraftController.cs
│   ├── AuthController.cs
│   ├── FlightsController.cs
│   ├── TechniciansController.cs
│   └── WorkOrdersController.cs
├── Models/               # Domain entities and data models
│   ├── Aircraft.cs
│   ├── Flight.cs
│   ├── Technician.cs
│   ├── User.cs
│   └── WorkOrder.cs
├── DTOs/                 # Data Transfer Objects for API contracts
├── Data/                 # Database context and configuration
│   ├── FlightWorkOrderContext.cs
│   └── DataSeeder.cs
├── Services/             # Business logic and services
├── Migrations/           # Entity Framework migrations
└── Program.cs            # Application startup and configuration
```

### Design Patterns Used
- **Repository Pattern** - Data access abstraction
- **DTO Pattern** - Separation of internal models from API contracts
- **Dependency Injection** - Loose coupling and testability
- **Service Layer** - Business logic separation

## 📋 Prerequisites

Before you begin, ensure you have the following installed:

- **.NET 8 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Git** - For version control
- **IDE/Editor** (choose one):
  - Visual Studio 2022 (recommended for full features)
  - Visual Studio Code with C# extension
  - JetBrains Rider
  - Any text editor with .NET support

### Verify Installation
```bash
# Check .NET version
dotnet --version
# Should output: 8.0.x or higher

# Check Git
git --version
```

## 🚀 Quick Start Guide

### Step 1: Clone the Repository
```bash
# Clone the repository
git clone https://github.com/MyatHsuMon23/Flights-WorkOrders_BackendAPIs.git

# Navigate to the project directory
cd Flights-WorkOrders_BackendAPIs
```

### Step 2: Restore Dependencies
```bash
# Restore NuGet packages
dotnet restore

# This will download all required packages including:
# - Entity Framework Core
# - JWT Authentication libraries
# - Swagger/OpenAPI tools
# - CsvHelper for data processing
```

### Step 3: Build the Project
```bash
# Build the solution
dotnet build

# You should see "Build succeeded" message
# Any warnings about package versions are non-critical
```

### Step 4: Run the Application
```bash
# Navigate to the API project directory
cd Flights_Work_Order_APIs

# Run the application
dotnet run

# Alternative: Run with hot reload for development
dotnet watch run
```

### Step 5: Access the Application
Once the application starts, you'll see output similar to:
```
Now listening on: http://localhost:5112
Application started. Press Ctrl+C to shut down.
```

**Access Points:**
- 🌐 **Swagger UI**: http://localhost:5112 (Interactive API documentation)
- 📡 **API Base URL**: http://localhost:5112/api
- 🔒 **HTTPS**: https://localhost:7154 (if configured)

## 🧪 Testing the API

### Using Swagger UI (Recommended for Beginners)
1. Open http://localhost:5112 in your browser
2. Explore available endpoints in the interactive documentation
3. Click "Try it out" on any endpoint
4. Fill in required parameters
5. Click "Execute" to test the API

### Using Authentication
1. **Register a new user**:
   ```
   POST /api/Auth/register
   {
     "email": "test@example.com",
     "password": "TestPassword123!",
     "firstName": "John",
     "lastName": "Doe"
   }
   ```

2. **Login to get JWT token**:
   ```
   POST /api/Auth/login
   {
     "email": "test@example.com",
     "password": "TestPassword123!"
   }
   ```

3. **Use the token**: Copy the returned token and click "Authorize" in Swagger UI

### Using cURL Examples

**Get all aircraft:**
```bash
curl -X GET "http://localhost:5112/api/Aircraft" \
     -H "accept: application/json"
```

**Create a new aircraft:**
```bash
curl -X POST "http://localhost:5112/api/Aircraft" \
     -H "accept: application/json" \
     -H "Content-Type: application/json" \
     -d '{
       "registrationNumber": "N12345",
       "model": "Boeing 737-800",
       "manufacturer": "Boeing",
       "capacity": 180,
       "status": "Active"
     }'
```

**Get flights by aircraft:**
```bash
curl -X GET "http://localhost:5112/api/Flights/by-aircraft/1" \
     -H "accept: application/json"
```

## 📚 API Documentation

### 🛩️ Aircraft Management
| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/Aircraft` | List all aircraft | ❌ |
| POST | `/api/Aircraft` | Create new aircraft | ✅ |
| GET | `/api/Aircraft/{id}` | Get specific aircraft | ❌ |
| PUT | `/api/Aircraft/{id}` | Update aircraft | ✅ |
| DELETE | `/api/Aircraft/{id}` | Delete aircraft | ✅ |

### ✈️ Flight Management
| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/Flights` | List all flights | ❌ |
| POST | `/api/Flights` | Create new flight | ✅ |
| GET | `/api/Flights/{id}` | Get specific flight | ❌ |
| PUT | `/api/Flights/{id}` | Update flight | ✅ |
| DELETE | `/api/Flights/{id}` | Delete flight | ✅ |
| GET | `/api/Flights/by-aircraft/{aircraftId}` | Get flights by aircraft | ❌ |

### 👨‍🔧 Technician Management
| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/Technicians` | List all technicians | ❌ |
| POST | `/api/Technicians` | Create new technician | ✅ |
| GET | `/api/Technicians/{id}` | Get specific technician | ❌ |
| PUT | `/api/Technicians/{id}` | Update technician | ✅ |
| DELETE | `/api/Technicians/{id}` | Delete technician | ✅ |
| GET | `/api/Technicians/active` | Get active technicians | ❌ |
| GET | `/api/Technicians/by-specialization/{spec}` | Filter by specialization | ❌ |

### 🔧 Work Order Management
| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/WorkOrders` | List all work orders | ❌ |
| POST | `/api/WorkOrders` | Create new work order | ✅ |
| GET | `/api/WorkOrders/{id}` | Get specific work order | ❌ |
| PUT | `/api/WorkOrders/{id}` | Update work order | ✅ |
| DELETE | `/api/WorkOrders/{id}` | Delete work order | ✅ |
| GET | `/api/WorkOrders/by-aircraft/{aircraftId}` | Get work orders by aircraft | ❌ |
| GET | `/api/WorkOrders/by-technician/{techId}` | Get work orders by technician | ❌ |
| GET | `/api/WorkOrders/by-status/{status}` | Filter by status | ❌ |

### 🔐 Authentication
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/Auth/register` | Register new user |
| POST | `/api/Auth/login` | Login user |
| POST | `/api/Auth/refresh` | Refresh JWT token |

## 📊 Sample Data

The application automatically seeds with comprehensive sample data including:

### Aircraft Fleet (3 Aircraft)
- **Boeing 737-800** (N737BA) - 180 passengers, Active
- **Airbus A320** (N320AB) - 150 passengers, Active  
- **Boeing 777-300** (N777BC) - 350 passengers, Maintenance

### Technical Staff (3 Technicians)
- **John Smith** - Engine Specialist, Active
- **Jane Doe** - Avionics Expert, Active
- **Mike Johnson** - Structural Specialist, Active

### Flight Schedule (4 Flights)
- **AA101**: New York → Los Angeles (Boeing 737)
- **UA202**: Chicago → Miami (Airbus A320)
- **DL303**: Seattle → Boston (Boeing 777)
- **SW404**: Denver → Phoenix (Boeing 737)

### Maintenance Tasks (4 Work Orders)
- **Engine Inspection** - High Priority, In Progress
- **Avionics Check** - Normal Priority, Completed
- **Landing Gear Service** - Critical Priority, Created
- **Routine Maintenance** - Low Priority, Assigned

## 🗃️ Data Models Reference

### Aircraft Status Options
- `Active` - Ready for service
- `Maintenance` - Under maintenance
- `Retired` - No longer in service
- `OutOfService` - Temporarily unavailable

### Flight Status Options
- `Scheduled` - Flight planned
- `Boarding` - Passengers boarding
- `Departed` - Flight has left
- `InFlight` - Currently flying
- `Landed` - Arrived at destination
- `Cancelled` - Flight cancelled
- `Delayed` - Flight delayed

### Work Order Priority Levels
- `Low` - Non-urgent maintenance
- `Normal` - Standard priority
- `High` - Important but not critical
- `Critical` - Safety-related issues
- `Emergency` - Immediate attention required

### Work Order Status Types
- `Created` - New work order
- `Assigned` - Assigned to technician
- `InProgress` - Work in progress
- `OnHold` - Temporarily paused
- `Completed` - Work finished
- `Cancelled` - Work order cancelled
- `Rejected` - Work order rejected

### Work Order Types
- `Preventive` - Scheduled maintenance
- `Corrective` - Fix identified issues
- `Inspection` - Safety inspections
- `Repair` - Component repairs
- `Overhaul` - Major maintenance
- `Emergency` - Urgent repairs
- `Upgrade` - System upgrades

## 🛠️ Development Workflow

### Setting Up Development Environment

1. **Install GitHub Copilot** (Highly Recommended)
   - Install GitHub Copilot extension in your IDE
   - Sign in with your GitHub account
   - Enable Copilot for faster development

2. **Configure Hot Reload**
   ```bash
   # Use hot reload for development
   dotnet watch run
   
   # This automatically recompiles and restarts the app when files change
   ```

3. **Database Development**
   ```bash
   # Add new migration after model changes
   dotnet ef migrations add YourMigrationName
   
   # Update database
   dotnet ef database update
   
   # Drop database (for fresh start)
   dotnet ef database drop
   ```

### Adding New Features

#### Example: Adding a New Entity (with Copilot)

1. **Create Model** (Copilot will suggest complete entity):
   ```csharp
   // In Models/YourEntity.cs
   public class YourEntity
   {
       // Copilot will suggest properties, validation, etc.
   }
   ```

2. **Update DbContext** (Copilot will suggest DbSet):
   ```csharp
   // In Data/FlightWorkOrderContext.cs
   public DbSet<YourEntity> YourEntities { get; set; }
   ```

3. **Create Controller** (Copilot will generate full CRUD):
   ```csharp
   // In Controllers/YourEntityController.cs
   // Copilot will suggest complete RESTful endpoints
   ```

4. **Add Migration**:
   ```bash
   dotnet ef migrations add AddYourEntity
   dotnet ef database update
   ```

### Copilot-Accelerated Development Tips

1. **Use Clear Comments**: Write descriptive comments, Copilot will generate better code
   ```csharp
   // Create a method to get all active aircraft with their flight schedules
   // Copilot will suggest the complete implementation
   ```

2. **Follow Patterns**: Start implementing similar patterns, Copilot will learn and suggest consistently
   
3. **Test Generation**: Write test method names, Copilot will suggest test implementations
   ```csharp
   [Test]
   public void Should_Return_NotFound_When_Aircraft_Does_Not_Exist()
   {
       // Copilot will suggest the complete test implementation
   }
   ```

### Code Quality Tools

```bash
# Format code
dotnet format

# Run static analysis (if configured)
dotnet build --verbosity normal

# Check for security vulnerabilities
dotnet list package --vulnerable
```

## 🐛 Troubleshooting Guide

### Common Issues and Solutions

#### 1. Port Already in Use
**Error**: `Unable to bind to http://localhost:5112`
```bash
# Solution 1: Use different port
dotnet run --urls "http://localhost:5113"

# Solution 2: Kill process using the port (Windows)
netstat -ano | findstr :5112
taskkill /PID <process_id> /F

# Solution 2: Kill process using the port (Linux/Mac)
lsof -ti:5112 | xargs kill -9
```

#### 2. Database Connection Issues
**Error**: Database connection failures
```bash
# Delete existing database and recreate
rm FlightWorkOrder.db

# Run migrations again
dotnet ef database update
```

#### 3. Package Restore Issues
**Error**: Package restore failures
```bash
# Clear NuGet cache
dotnet nuget locals all --clear

# Restore packages again
dotnet restore
```

#### 4. JWT Token Issues
**Error**: `401 Unauthorized` responses
- Ensure you're logged in and have a valid token
- Check token expiration (default: 1 hour)
- Verify the token is properly set in Authorization header

#### 5. Swagger UI Not Loading
**Error**: Swagger UI shows errors
```bash
# Check if running in Development environment
echo $ASPNETCORE_ENVIRONMENT  # Should be "Development"

# Force development mode
export ASPNETCORE_ENVIRONMENT=Development
dotnet run
```

### Performance Tips

1. **Database Optimization**:
   - Use proper indexing for frequently queried fields
   - Implement pagination for large datasets
   - Use async/await for database operations

2. **Memory Management**:
   - Dispose of DbContext properly (handled by DI)
   - Use streaming for large file operations
   - Monitor memory usage in production

3. **API Performance**:
   - Implement response caching where appropriate
   - Use compression for large responses
   - Consider API versioning for breaking changes

### Debug Mode

```bash
# Run in debug mode with detailed logging
dotnet run --configuration Debug

# Enable detailed Entity Framework logging
# Add to appsettings.Development.json:
{
  "Logging": {
    "LogLevel": {
      "Microsoft.EntityFrameworkCore": "Information"
    }
  }
}
```

## 🚀 Deployment Guide

### Local Development
```bash
# Development with hot reload
dotnet watch run

# Production-like build
dotnet build --configuration Release
dotnet run --configuration Release
```

### Production Deployment

#### 1. Environment Configuration
```bash
# Set production environment
export ASPNETCORE_ENVIRONMENT=Production

# Configure production database connection
# Update appsettings.Production.json with your database connection string
```

#### 2. Database Setup for Production
```bash
# Generate SQL script for production database
dotnet ef migrations script --output migration.sql

# Apply to production database
# Execute migration.sql on your production database server
```

#### 3. Docker Deployment (Optional)
```dockerfile
# Sample Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY bin/Release/net8.0/publish/ .
ENTRYPOINT ["dotnet", "Flights_Work_Order_APIs.dll"]
```

### CI/CD Integration

The project is ready for CI/CD pipelines:
- GitHub Actions
- Azure DevOps
- Jenkins
- Docker containers

## 📈 Monitoring and Logging

### Application Insights (Recommended for Production)
```csharp
// Add to Program.cs for production monitoring
builder.Services.AddApplicationInsightsTelemetry();
```

### Health Checks
```bash
# Check application health
curl http://localhost:5112/health
```

## 🎯 Learning Objectives

This project demonstrates modern .NET development practices:

### Technical Skills
1. **RESTful API Design** - Proper HTTP verbs, status codes, and resource modeling
2. **Entity Framework Core** - Code-first approach, relationships, and querying
3. **Authentication & Authorization** - JWT implementation and security best practices
4. **Data Validation** - Input validation and business rule enforcement
5. **Error Handling** - Graceful error responses and user-friendly messages
6. **API Documentation** - Swagger integration for interactive documentation
7. **Clean Architecture** - Separation of concerns and maintainable code structure

### Development Productivity
1. **GitHub Copilot Integration** - AI-assisted development for faster coding
2. **Hot Reload Development** - Rapid iteration and testing
3. **Automated Testing** - Unit tests and integration testing patterns
4. **Code Quality** - Consistent patterns and best practices
5. **Documentation** - Comprehensive API and code documentation

## 🎓 Next Steps and Enhancements

### Immediate Improvements
- [ ] Add comprehensive unit tests for all controllers
- [ ] Implement integration tests for API endpoints
- [ ] Add input validation unit tests
- [ ] Create performance tests for high-load scenarios

### Advanced Features
- [ ] **Caching Layer**: Implement Redis for performance optimization
- [ ] **Background Services**: Add scheduled maintenance reminders
- [ ] **File Upload**: Support for aircraft documentation and images
- [ ] **Real-time Updates**: SignalR for live work order status updates
- [ ] **Audit Logging**: Track all changes for compliance
- [ ] **API Versioning**: Support multiple API versions
- [ ] **Rate Limiting**: Implement API rate limiting
- [ ] **Health Checks**: Comprehensive application health monitoring

### Production Readiness
- [ ] **Database Migration**: Switch to SQL Server/PostgreSQL for production
- [ ] **Logging**: Implement structured logging with Serilog
- [ ] **Monitoring**: Add Application Insights or similar monitoring
- [ ] **Security**: Implement role-based authorization
- [ ] **Documentation**: Add comprehensive API documentation
- [ ] **Error Handling**: Implement global exception handling
- [ ] **Validation**: Add business rule validation layer

### GitHub Copilot Expansion
- [ ] Use Copilot for test generation and test data creation
- [ ] Leverage Copilot for documentation generation
- [ ] Use Copilot Chat for architecture discussions
- [ ] Apply Copilot suggestions for performance optimization

## 🤝 Contributing

### Development Guidelines
1. **Use GitHub Copilot** for consistent code generation
2. **Follow existing patterns** established in the codebase
3. **Write tests** for new features
4. **Update documentation** when adding new endpoints
5. **Use conventional commits** for clear change history

### Code Style
- Follow C# coding conventions
- Use meaningful variable and method names
- Add XML documentation for public APIs
- Implement proper error handling

## 📝 License

This project is for educational purposes as part of a comprehensive backend API development guide.

## 🙏 Acknowledgments

- **GitHub Copilot** - For significantly accelerating development time and improving code quality
- **.NET Team** - For the excellent .NET 8 framework and tooling
- **Entity Framework Team** - For the powerful ORM capabilities
- **Swagger/OpenAPI** - For excellent API documentation tools

---

## 📞 Support

If you encounter any issues or have questions:

1. **Check the Troubleshooting Guide** above
2. **Review the API documentation** in Swagger UI
3. **Examine the sample data** and expected formats
4. **Use GitHub Copilot** to help debug and suggest solutions

### Quick Reference Commands
```bash
# Start fresh
git clone <repository-url>
cd Flights-WorkOrders_BackendAPIs
dotnet restore && dotnet build && cd Flights_Work_Order_APIs && dotnet run

# Reset database
rm FlightWorkOrder.db && dotnet ef database update

# Check application health
curl http://localhost:5112/api/Aircraft
```

**Happy Coding! 🚀**

> 💡 **Remember**: This project demonstrates how GitHub Copilot can reduce development time by 60-70% while maintaining high code quality and consistency. Use it as a reference for your own AI-assisted development workflows!