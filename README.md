# Flight Work Order Backend APIs - Complete Documentation

## 📋 Overview

This repository contains comprehensive backend APIs for flight and work order management systems. The APIs provide complete functionality for managing flight schedules, maintenance work orders, user authentication, and system operations.

## 🔗 Documentation Navigation

### API Workflow Documentation
- **[Work Orders API Workflow](./WorkOrder_API_Workflow.md)** - Complete work order management workflows
- **[Flights API Workflow](./Flights_API_Workflow.md)** - Flight management and import operations
- **[Authentication API Workflow](./Authentication_API_Workflow.md)** - User authentication and security
- **[Technical Documentation](./Technical_Documentation.md)** - Complete system architecture and technical details

### Implementation Details
- **[Implementation Summary](./IMPLEMENTATION_SUMMARY.md)** - Pagination implementation and features summary

## 🚀 Quick Start

### Prerequisites
- .NET 8.0 SDK
- SQL Server (LocalDB for development)
- Visual Studio 2022 or VS Code

### Running the Application
```bash
# Clone the repository
git clone <repository-url>
cd CodeLab_BackendAPIs

# Restore dependencies
dotnet restore

# Build the project
dotnet build

# Run the application
dotnet run --project Flights_Work_Order_APIs
```

The API will be available at:
- **HTTPS**: `https://localhost:7159`
- **HTTP**: `http://localhost:5159`
- **Swagger UI**: `https://localhost:7159` (root URL)

## 📚 API Overview

### 🔐 Authentication API (`/api/Auth`)
Handles user authentication and JWT token management.

**Endpoints:**
- `POST /api/Auth/login` - User authentication
- `POST /api/Auth/encrypt-password` - Password encryption helper

**Key Features:**
- JWT token generation and validation
- Secure password encryption
- Role-based authentication

### ✈️ Flights API (`/api/Flights`)
Manages flight data, imports, and operational commands.

**Endpoints:**
- `GET /api/Flights` - Retrieve flights with pagination and filtering
- `GET /api/Flights/{id}` - Get specific flight
- `POST /api/Flights/import` - Import flights from JSON
- `POST /api/Flights/import/csv` - Import flights from CSV file
- `POST /api/Flights/{flightId}/commands` - Add flight commands (with validation)
- `GET /api/Flights/{flightId}/commands` - Get flight commands
- `POST /api/Flights/{flightId}/work-orders` - Create work order for flight

**Key Features:**
- Advanced filtering and sorting
- Bulk import capabilities (CSV/JSON)
- Flight command management with validation
- Flight-work order integration
- Pagination support

### 🔧 Work Orders API (`/api/WorkOrders`)
Comprehensive work order management for aircraft maintenance.

**Endpoints:**
- `GET /api/WorkOrders` - Retrieve work orders with pagination
- `GET /api/WorkOrders/{id}` - Get specific work order
- `POST /api/WorkOrders` - Create new work order
- `PUT /api/WorkOrders/{id}` - Update work order
- `DELETE /api/WorkOrders/{id}` - Delete work order
- `GET /api/WorkOrders/statistics` - Get work order statistics

**Key Features:**
- Complete lifecycle management (Open → InProgress → Completed)
- Priority-based task management
- Technician assignment and tracking
- Statistics and reporting

### 🌤️ Weather Forecast API (`/api/WeatherForecast`)
Demo endpoint showing API patterns and authentication.

**Endpoints:**
- `GET /api/WeatherForecast` - Get sample weather data

## 🔑 Authentication

All APIs (except login) require JWT authentication:

```bash
# 1. Login to get token
curl -X POST "https://localhost:7159/api/Auth/login" \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"encrypted_password"}'

# 2. Use token in subsequent requests
curl -X GET "https://localhost:7159/api/WorkOrders" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

## 📊 Response Format

All APIs follow a consistent response format:

```json
{
  "success": true,
  "message": "Operation completed successfully",
  "data": {...},
  "pagination": {
    "currentPage": 1,
    "from": 1,
    "to": 10,
    "lastPage": 5,
    "perPage": 10,
    "total": 45
  }
}
```

## 🔄 Common Workflows

### 1. Basic Authentication Flow
```
1. POST /api/Auth/login → Get JWT token
2. Include "Authorization: Bearer <token>" in all requests
3. Token expires after configured time
4. Re-authenticate when token expires
```

### 2. Work Order Management Flow
```
1. Create work order → POST /api/WorkOrders
2. Assign to technician → PUT /api/WorkOrders/{id}
3. Update status to InProgress → PUT /api/WorkOrders/{id}
4. Complete work → PUT /api/WorkOrders/{id} (status: Completed)
5. View statistics → GET /api/WorkOrders/statistics
```

### 3. Flight Data Import Flow
```
1. Prepare CSV/JSON file with flight data
2. Upload via POST /api/Flights/import/csv
3. Review import results and errors
4. Query imported flights → GET /api/Flights
```

## 🎯 Key Features

### ✅ Pagination Support
- Consistent pagination across WorkOrders and Flights APIs
- Configurable page sizes (max 100 items)
- Complete pagination metadata

### ✅ Advanced Filtering
- Filter work orders by status and aircraft registration
- Filter flights by flight number
- Flexible sorting options

### ✅ Bulk Import Capabilities
- CSV file import with validation
- JSON array import
- Detailed error reporting for failed imports

### ✅ Security Features
- JWT authentication for all protected endpoints
- Password encryption
- Role-based authorization
- Secure token validation

### ✅ Data Validation
- Comprehensive input validation
- Error handling with descriptive messages
- Data integrity enforcement

## 📁 Project Structure

```
Flights_Work_Order_APIs/
├── Controllers/           # API controllers
│   ├── AuthController.cs
│   ├── FlightsController.cs
│   ├── FlightWorkOrdersController.cs
│   └── WeatherForecastController.cs
├── Models/               # Data models and DTOs
├── Services/             # Business logic services
├── Data/                 # Entity Framework context
└── Program.cs           # Application startup
```

## 🛠️ Configuration

### Database Connection
Configure in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=FlightWorkOrderDb;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

### JWT Settings
```json
{
  "JwtSettings": {
    "SecretKey": "your-secret-key-here",
    "Issuer": "FlightWorkOrderSystem",
    "Audience": "FlightWorkOrderClient",
    "ExpirationMinutes": 60
  }
}
```

## 🧪 Testing

### Using Swagger UI
1. Navigate to `https://localhost:7159`
2. Use the interactive Swagger interface
3. Authenticate using the "Authorize" button
4. Test all endpoints interactively

### Using Sample Data
The repository includes sample files:
- `sample_flights.csv` - Sample flight data for import testing
- `sample_flights.json` - Sample JSON flight data

### Authentication Testing
```bash
# Get encrypted password for testing
curl -X POST "https://localhost:7159/api/Auth/encrypt-password" \
  -H "Content-Type: application/json" \
  -d '"admin"'

# Use encrypted password to login
curl -X POST "https://localhost:7159/api/Auth/login" \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"<encrypted_password>"}'
```

## 📈 Performance Considerations

- **Pagination**: Always use pagination for large datasets
- **Filtering**: Use specific filters to reduce result sets
- **Async Operations**: All database operations are asynchronous
- **Connection Pooling**: Entity Framework manages database connections

## 🚨 Error Handling

Standard HTTP status codes:
- `200 OK` - Successful operation
- `201 Created` - Resource created successfully
- `400 Bad Request` - Invalid request data
- `401 Unauthorized` - Authentication required
- `403 Forbidden` - Insufficient permissions
- `404 Not Found` - Resource not found
- `500 Internal Server Error` - Server error

## 🔒 Security Best Practices

1. **Always use HTTPS** in production
2. **Validate all input data** before processing
3. **Use strong JWT secret keys**
4. **Implement rate limiting** for production
5. **Monitor authentication failures**
6. **Remove helper endpoints** in production

## 📖 Additional Resources

- **[ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)**
- **[Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)**
- **[JWT.io](https://jwt.io/)** - JWT token debugging
- **[Swagger Documentation](https://swagger.io/docs/)**

## 🤝 Contributing

1. Follow existing code patterns and conventions
2. Add tests for new functionality
3. Update documentation for API changes
4. Ensure backward compatibility

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

---

**Need Help?** Check the detailed workflow documentation for specific API usage patterns and examples.