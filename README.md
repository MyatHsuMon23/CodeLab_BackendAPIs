# Flight Work Order Backend APIs

A comprehensive .NET 8.0 Web API system for managing flight operations and aircraft maintenance workflows in the aviation industry.

## 🚀 Quick Start

### Prerequisites
- .NET 8.0 SDK
- Visual Studio 2022 or VS Code
- SQLite (included)

### Running the Application
```bash
# Clone the repository
git clone <repository-url>
cd CodeLab_BackendAPIs

# Restore dependencies and build
dotnet restore
dotnet build

# Run the application
dotnet run --project Flights_Work_Order_APIs
```

**Access Points:**
- **API Base URL**: `https://localhost:7159`
- **Swagger Documentation**: `https://localhost:7159/swagger`

## 📚 Documentation

### Core Documentation
1. **[Technical Documentation](./Technical_Documentation.md)**
   - System architecture and technology stack
   - Database schema and entity relationships
   - Security implementation and configuration
   - Performance considerations and deployment

2. **[Implementation Guide](./Implementation_Guide.md)**
   - Step-by-step implementation process
   - Code examples and development workflow
   - Configuration and setup instructions
   - Testing and validation procedures

3. **[Project Workflow & Objectives](./Project_Workflow_Objectives.md)**
   - Business purpose and value proposition
   - System workflows and integration patterns
   - User roles and permissions
   - Future roadmap and success metrics

## 🔌 API Overview

### Authentication API (`/api/Auth`)
- User login and JWT token management
- Password encryption utilities
- Role-based authentication

### Flight Management API (`/api/Flights`)
- Flight data management with pagination
- CSV/JSON bulk import capabilities
- Flight command processing
- Integration with work order systems

### Work Order Management API (`/api/WorkOrders`)
- Complete CRUD operations for maintenance work orders
- Status lifecycle management (Open → InProgress → Completed)
- Technician assignment and tracking
- Statistical reporting

### Weather Forecast API (`/api/WeatherForecast`)
- Demo endpoint for API patterns
- Authentication demonstration

## 🔐 Authentication

All protected endpoints require JWT authentication:

```bash
# 1. Login to get token
curl -X POST "https://localhost:7159/api/Auth/login" \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"encrypted_password"}'

# 2. Use token in subsequent requests
curl -X GET "https://localhost:7159/api/WorkOrders" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

## 🏗️ Technology Stack

- **Framework**: ASP.NET Core 8.0
- **Database**: SQLite (development) / SQL Server (production)
- **Authentication**: JWT (JSON Web Tokens)
- **ORM**: Entity Framework Core
- **Documentation**: Swagger/OpenAPI
- **Security**: Password encryption, CORS support

## 📊 Key Features

### ✅ Comprehensive API Coverage
- RESTful design with consistent response formats
- Complete CRUD operations for all entities
- Advanced filtering and pagination support
- Bulk import capabilities (CSV/JSON)

### ✅ Enterprise Security
- JWT-based authentication
- Password encryption
- Role-based authorization
- CORS configuration

### ✅ Developer Experience
- Interactive Swagger documentation
- Consistent error handling
- Comprehensive logging
- Clear response structures

### ✅ Production Ready
- Async operations throughout
- Database connection pooling
- Proper exception handling
- Scalable architecture

## 🔄 Common Workflows

### Work Order Management Flow
```
1. Create work order → POST /api/WorkOrders
2. Assign to technician → PUT /api/WorkOrders/{id}
3. Update status to InProgress → PUT /api/WorkOrders/{id}
4. Complete work → PUT /api/WorkOrders/{id} (status: Completed)
5. View statistics → GET /api/WorkOrders/statistics
```

### Flight Data Import Flow
```
1. Prepare CSV/JSON file with flight data
2. Upload via POST /api/Flights/import/csv
3. Review import results and errors
4. Query imported flights → GET /api/Flights
```

## 📝 Response Format

All APIs return consistent response structures:

```json
{
  "success": true,
  "message": "Operation completed successfully",
  "data": {...},
  "pagination": {
    "currentPage": 1,
    "lastPage": 5,
    "perPage": 10,
    "total": 45
  }
}
```

## 🧪 Testing

### Using Swagger UI
1. Navigate to `https://localhost:7159/swagger`
2. Use the "Authorize" button to add your JWT token
3. Test all endpoints interactively

### Sample Data
- `sample_flights.csv` - Sample flight data for import testing
- `sample_flights.json` - Sample JSON flight data

## 🔧 Configuration

### Database Connection
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=flights.db"
  }
}
```

### JWT Settings
```json
{
  "Jwt": {
    "SecretKey": "your-secret-key-here",
    "Issuer": "FlightWorkOrderAPI",
    "Audience": "FlightWorkOrderClient",
    "ExpirationMinutes": 60
  }
}
```

## 🏢 Project Structure

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

## 🚨 Error Handling

Standard HTTP status codes:
- `200 OK` - Successful operation
- `201 Created` - Resource created successfully
- `400 Bad Request` - Invalid request data
- `401 Unauthorized` - Authentication required
- `404 Not Found` - Resource not found
- `500 Internal Server Error` - Server error

## 📈 Future Enhancements

- Real-time notifications with SignalR
- Advanced reporting and analytics
- Mobile API optimizations
- Multi-tenant architecture support
- Integration with external aviation systems

## 🤝 Contributing

1. Follow existing code patterns and conventions
2. Add comprehensive tests for new functionality
3. Update documentation for any API changes
4. Ensure backward compatibility

## 📄 License

This project is licensed under the MIT License.

---

**Need Help?** Check the detailed documentation files for comprehensive information about the system architecture, implementation details, and business workflows.