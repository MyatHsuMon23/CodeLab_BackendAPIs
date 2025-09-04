# Flight Work Order Backend APIs - Technical Documentation

## System Architecture Overview

### Technology Stack
- **Framework**: ASP.NET Core 8.0
- **Database**: SQLite with Entity Framework Core
- **Authentication**: JWT (JSON Web Tokens)
- **API Documentation**: Swagger/OpenAPI
- **Development Environment**: Visual Studio 2022/.NET CLI

### System Components

#### 1. Controllers Layer
- **AuthController**: Handles user authentication and JWT token management
- **FlightsController**: Manages flight data, imports, and operational commands
- **FlightWorkOrdersController**: Manages work orders for aircraft maintenance
- **WeatherForecastController**: Demo endpoint for API patterns

#### 2. Services Layer
- **UserService**: User management and authentication logic
- **JwtService**: JWT token generation and validation
- **EncryptionService**: Password encryption and decryption
- **FlightCommandService**: Flight command processing and validation

#### 3. Data Layer
- **FlightDbContext**: Entity Framework database context
- **Models**: Data transfer objects and entity models
- **SQLite Database**: Local database file (flights.db)

#### 4. Security Features
- **JWT Authentication**: Stateless token-based authentication
- **Password Encryption**: Two-way encryption for password security
- **CORS Configuration**: Cross-origin request handling
- **Role-based Authorization**: User role management

## Database Schema

### Core Entities

#### Users Table
```sql
Users (
    Id (Primary Key),
    Username (Unique),
    PasswordHash (Encrypted),
    Role,
    CreatedDate,
    LastLoginDate
)
```

#### Flights Table
```sql
Flights (
    Id (Primary Key),
    FlightNumber,
    AircraftRegistration,
    DepartureAirport,
    ArrivalAirport,
    DepartureTime,
    ArrivalTime,
    Status,
    CreatedDate,
    UpdatedDate
)
```

#### WorkOrders Table
```sql
WorkOrders (
    Id (Primary Key),
    WorkOrderNumber (Generated),
    AircraftRegistration,
    TaskDescription,
    Status (Open/InProgress/Completed/OnHold/Cancelled),
    Priority (Critical/High/Medium/Low),
    AssignedTechnician,
    CreatedBy,
    CreatedDate,
    ScheduledDate,
    CompletedDate,
    Notes
)
```

#### FlightCommands Table
```sql
FlightCommands (
    Id (Primary Key),
    FlightId (Foreign Key),
    CommandType,
    CommandData,
    SubmittedBy,
    SubmittedDate,
    Status
)
```

### Entity Relationships
- **One-to-Many**: Flights → FlightCommands
- **Many-to-Many**: Flights ↔ WorkOrders (through aircraft registration)
- **One-to-Many**: Users → WorkOrders (creator/assignee)

## API Architecture

### Authentication Flow
```
1. POST /api/Auth/login
   ↓
2. JWT Token Generated
   ↓
3. Bearer Token in Authorization Header
   ↓
4. Token Validation on Each Request
   ↓
5. Access Granted/Denied
```

### Request/Response Pipeline
```
HTTP Request
    ↓
CORS Middleware
    ↓
Authentication Middleware
    ↓
Authorization Middleware
    ↓
Controller Action
    ↓
Service Layer
    ↓
Entity Framework
    ↓
SQLite Database
    ↓
Response Serialization
    ↓
HTTP Response
```

### Response Format Standard
All APIs follow a consistent response wrapper:

```csharp
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }
}

public class PaginatedApiResponse<T> : ApiResponse<T>
{
    public Pagination Pagination { get; set; }
}

public class Pagination
{
    public int CurrentPage { get; set; }
    public int LastPage { get; set; }
    public int PerPage { get; set; }
    public int Total { get; set; }
}
```

## Configuration Management

### Application Settings Structure
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=flights.db"
  },
  "Jwt": {
    "SecretKey": "Flight_Work_Order_Secret_Key_For_JWT_Token_Generation_2024",
    "Issuer": "FlightWorkOrderAPI",
    "Audience": "FlightWorkOrderClient",
    "ExpirationMinutes": 60
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### Environment Configuration
- **Development**: Swagger UI enabled, detailed error messages
- **Production**: Swagger UI disabled, minimal error exposure
- **Database**: SQLite for development, can be configured for SQL Server/PostgreSQL

## Security Implementation

### JWT Token Configuration
- **Algorithm**: HMAC SHA-256
- **Validation Parameters**:
  - ValidateIssuerSigningKey: true
  - ValidateIssuer: true
  - ValidateAudience: true
  - ValidateLifetime: true
  - ClockSkew: Zero tolerance

### Password Security
- **Encryption**: Two-way encryption using EncryptionService
- **Storage**: Encrypted passwords in database
- **Validation**: Decrypt and compare during authentication

### Authorization Attributes
```csharp
[Authorize] // Requires valid JWT token
[Route("api/[controller]")]
[ApiController]
```

## Performance Considerations

### Database Optimization
- **Async Operations**: All database calls use async/await
- **Entity Framework**: Optimized queries with proper includes
- **Pagination**: Efficient data retrieval for large datasets
- **Connection Pooling**: Automatic connection management

### Memory Management
- **Dependency Injection**: Proper service lifetime management
- **Disposal Pattern**: Automatic resource cleanup
- **JSON Serialization**: Optimized with ReferenceHandler.IgnoreCycles

### Scalability Features
- **Stateless Authentication**: JWT tokens enable horizontal scaling
- **CORS Support**: Multi-client application support
- **API Versioning Ready**: URL-based versioning support
- **Microservice Architecture**: Service layer separation

## Error Handling Strategy

### Exception Management
```csharp
try
{
    // Business logic
    return Ok(ApiResponse<T>.CreateSuccess(data, message));
}
catch (UnauthorizedAccessException ex)
{
    return Unauthorized(ApiResponse.CreateError(ex.Message));
}
catch (ValidationException ex)
{
    return BadRequest(ApiResponse.CreateError(ex.Message));
}
catch (Exception ex)
{
    _logger.LogError(ex, "Unexpected error occurred");
    return StatusCode(500, ApiResponse.CreateError("Internal server error"));
}
```

### HTTP Status Code Standards
- **200 OK**: Successful GET, PUT operations
- **201 Created**: Successful POST operations
- **400 Bad Request**: Validation errors, malformed requests
- **401 Unauthorized**: Missing or invalid authentication
- **403 Forbidden**: Insufficient permissions
- **404 Not Found**: Resource not found
- **500 Internal Server Error**: Unexpected server errors

## Deployment Architecture

### Development Environment
```
Local Machine
├── .NET 8.0 SDK
├── SQLite Database (flights.db)
├── Visual Studio 2022/VS Code
└── Swagger UI (https://localhost:7159/swagger)
```

### Production Considerations
```
Production Server
├── IIS/Kestrel Web Server
├── SQL Server/PostgreSQL Database
├── HTTPS Certificate
├── Application Insights/Logging
├── Load Balancer (if scaled)
└── Reverse Proxy (nginx/IIS)
```

### Build and Deployment Pipeline
```
Source Control (Git)
    ↓
Build Pipeline (.NET CLI)
    ↓
Unit Tests (if available)
    ↓
Package Creation
    ↓
Deployment to Target Environment
    ↓
Database Migration
    ↓
Health Check Validation
```

## Monitoring and Logging

### Built-in Logging
- **Microsoft.Extensions.Logging**: Integrated logging framework
- **Log Levels**: Information, Warning, Error, Critical
- **Structured Logging**: JSON-formatted log entries

### Health Monitoring
- **Startup Validation**: Database connection verification
- **Middleware Pipeline**: Request/response logging
- **Exception Tracking**: Automatic error capture and logging

### Performance Metrics
- **Response Times**: Built-in ASP.NET Core metrics
- **Memory Usage**: .NET runtime monitoring
- **Database Performance**: Entity Framework query logging

## Testing Strategy

### Unit Testing Framework
```csharp
// Example test structure (framework ready)
[TestClass]
public class AuthControllerTests
{
    [TestMethod]
    public async Task Login_ValidCredentials_ReturnsJwtToken()
    {
        // Arrange, Act, Assert
    }
}
```

### Integration Testing
- **API Endpoint Testing**: Full request/response cycle
- **Database Integration**: Entity Framework operations
- **Authentication Flow**: JWT token lifecycle

### Testing Tools
- **Swagger UI**: Manual API testing interface
- **Postman**: API collection testing
- **Unit Test Framework**: MSTest/xUnit ready
- **Sample Data**: CSV/JSON files for import testing

## Future Enhancement Opportunities

### Scalability Improvements
- **Caching Layer**: Redis/In-Memory caching
- **Message Queues**: Background job processing
- **Database Sharding**: Multi-tenant architecture
- **CDN Integration**: Static content delivery

### Feature Enhancements
- **Real-time Updates**: SignalR integration
- **File Storage**: Azure Blob/AWS S3 integration
- **Email Notifications**: SMTP/SendGrid integration
- **Audit Logging**: Comprehensive change tracking

### Security Enhancements
- **Rate Limiting**: API throttling implementation
- **OAuth 2.0**: Third-party authentication providers
- **Data Encryption**: Column-level encryption
- **API Gateway**: Centralized security and routing

## Development Guidelines

### Code Standards
- **C# Conventions**: Microsoft coding standards
- **Async/Await**: Consistent asynchronous programming
- **Dependency Injection**: Constructor injection pattern
- **Error Handling**: Consistent exception management

### API Design Principles
- **RESTful Design**: HTTP verb conventions
- **Resource Naming**: Plural nouns for collections
- **Versioning Strategy**: URL-based versioning
- **Documentation**: Comprehensive XML comments

### Database Design Principles
- **Normalization**: Proper table relationships
- **Indexing Strategy**: Performance optimization
- **Migration Management**: Code-first approach
- **Data Integrity**: Constraint enforcement

This technical documentation provides a comprehensive overview of the Flight Work Order Backend APIs system architecture, implementation details, and operational considerations.