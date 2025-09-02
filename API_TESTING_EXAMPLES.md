# 📋 API Testing Examples

## Overview
This document provides comprehensive examples for testing all API endpoints using various methods.

## 🔐 Authentication Examples

### Login User
```bash
curl -X POST "http://localhost:5112/api/Auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "john.doe@example.com",
    "password": "SecurePassword123!"
  }'
```

**Expected Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiration": "2024-01-01T12:00:00Z",
  "user": {
    "id": 1,
    "email": "john.doe@example.com",
    "firstName": "John",
    "lastName": "Doe"
  }
}
```

## ✈️ Aircraft Management Examples

### Get All Aircraft
```bash
curl -X GET "http://localhost:5112/api/Aircraft" \
  -H "accept: application/json"
```

**Expected Response:**
```json
[
  {
    "id": 1,
    "registrationNumber": "N737BA",
    "model": "Boeing 737-800",
    "manufacturer": "Boeing",
    "capacity": 180,
    "status": "Active",
    "createdAt": "2024-01-01T00:00:00Z",
    "updatedAt": "2024-01-01T00:00:00Z"
  }
]
```

### Create New Aircraft (Authenticated)
```bash
curl -X POST "http://localhost:5112/api/Aircraft" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -d '{
    "registrationNumber": "N999XX",
    "model": "Airbus A350",
    "manufacturer": "Airbus",
    "capacity": 300,
    "status": "Active"
  }'
```

### Get Specific Aircraft
```bash
curl -X GET "http://localhost:5112/api/Aircraft/1" \
  -H "accept: application/json"
```

### Update Aircraft (Authenticated)
```bash
curl -X PUT "http://localhost:5112/api/Aircraft/1" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -d '{
    "registrationNumber": "N737BA",
    "model": "Boeing 737-800 MAX",
    "manufacturer": "Boeing",
    "capacity": 185,
    "status": "Active"
  }'
```

### Delete Aircraft (Authenticated)
```bash
curl -X DELETE "http://localhost:5112/api/Aircraft/1" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

## 🛫 Flight Management Examples

### Get All Flights
```bash
curl -X GET "http://localhost:5112/api/Flights" \
  -H "accept: application/json"
```

### Create New Flight (Authenticated)
```bash
curl -X POST "http://localhost:5112/api/Flights" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -d '{
    "flightNumber": "AA505",
    "origin": "LAX",
    "destination": "JFK",
    "departureTime": "2024-01-15T10:00:00Z",
    "arrivalTime": "2024-01-15T18:00:00Z",
    "aircraftId": 1,
    "status": "Scheduled"
  }'
```

### Get Flights by Aircraft
```bash
curl -X GET "http://localhost:5112/api/Flights/by-aircraft/1" \
  -H "accept: application/json"
```

**Expected Response:**
```json
[
  {
    "id": 1,
    "flightNumber": "AA101",
    "origin": "JFK",
    "destination": "LAX",
    "departureTime": "2024-01-01T08:00:00Z",
    "arrivalTime": "2024-01-01T11:00:00Z",
    "aircraftId": 1,
    "aircraft": {
      "registrationNumber": "N737BA",
      "model": "Boeing 737-800"
    },
    "status": "Scheduled"
  }
]
```

## 👨‍🔧 Technician Management Examples

### Get All Technicians
```bash
curl -X GET "http://localhost:5112/api/Technicians" \
  -H "accept: application/json"
```

### Create New Technician (Authenticated)
```bash
curl -X POST "http://localhost:5112/api/Technicians" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -d '{
    "firstName": "Sarah",
    "lastName": "Wilson",
    "email": "sarah.wilson@airline.com",
    "phoneNumber": "+1-555-0199",
    "specialization": "Hydraulics",
    "certifications": "FAA A&P, Type Rating Boeing 777",
    "experienceYears": 8,
    "status": "Active"
  }'
```

### Get Active Technicians
```bash
curl -X GET "http://localhost:5112/api/Technicians/active" \
  -H "accept: application/json"
```

### Get Technicians by Specialization
```bash
curl -X GET "http://localhost:5112/api/Technicians/by-specialization/Engine" \
  -H "accept: application/json"
```

## 🔧 Work Order Management Examples

### Get All Work Orders
```bash
curl -X GET "http://localhost:5112/api/WorkOrders" \
  -H "accept: application/json"
```

### Create New Work Order (Authenticated)
```bash
curl -X POST "http://localhost:5112/api/WorkOrders" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -d '{
    "title": "Annual Inspection",
    "description": "Comprehensive annual safety inspection as required by FAA regulations",
    "aircraftId": 1,
    "technicianId": 1,
    "priority": "High",
    "type": "Inspection",
    "estimatedHours": 24,
    "scheduledDate": "2024-01-20T09:00:00Z"
  }'
```

**Expected Response:**
```json
{
  "id": 5,
  "title": "Annual Inspection",
  "description": "Comprehensive annual safety inspection as required by FAA regulations",
  "aircraftId": 1,
  "aircraft": {
    "registrationNumber": "N737BA",
    "model": "Boeing 737-800"
  },
  "technicianId": 1,
  "technician": {
    "firstName": "John",
    "lastName": "Smith",
    "specialization": "Engine"
  },
  "priority": "High",
  "type": "Inspection",
  "status": "Created",
  "estimatedHours": 24,
  "scheduledDate": "2024-01-20T09:00:00Z",
  "createdAt": "2024-01-01T12:00:00Z",
  "updatedAt": "2024-01-01T12:00:00Z"
}
```

### Get Work Orders by Aircraft
```bash
curl -X GET "http://localhost:5112/api/WorkOrders/by-aircraft/1" \
  -H "accept: application/json"
```

### Get Work Orders by Technician
```bash
curl -X GET "http://localhost:5112/api/WorkOrders/by-technician/1" \
  -H "accept: application/json"
```

### Get Work Orders by Status
```bash
curl -X GET "http://localhost:5112/api/WorkOrders/by-status/InProgress" \
  -H "accept: application/json"
```

### Update Work Order Status (Authenticated)
```bash
curl -X PUT "http://localhost:5112/api/WorkOrders/1" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -d '{
    "title": "Engine Inspection",
    "description": "Routine engine inspection and maintenance - Updated with additional checks",
    "aircraftId": 1,
    "technicianId": 1,
    "priority": "High",
    "type": "Inspection",
    "status": "InProgress",
    "estimatedHours": 12,
    "actualHours": 8,
    "scheduledDate": "2024-01-01T09:00:00Z",
    "startedDate": "2024-01-01T09:15:00Z"
  }'
```

## 🧪 Complex Testing Scenarios

### Scenario 1: Complete Flight Operation Workflow
```bash
# 1. Create aircraft
curl -X POST "http://localhost:5112/api/Aircraft" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -d '{"registrationNumber":"N123TEST","model":"Test Aircraft","manufacturer":"Test Corp","capacity":150,"status":"Active"}'

# 2. Schedule flight for the aircraft
curl -X POST "http://localhost:5112/api/Flights" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -d '{"flightNumber":"TEST001","origin":"LAX","destination":"JFK","departureTime":"2024-02-01T10:00:00Z","arrivalTime":"2024-02-01T18:00:00Z","aircraftId":4,"status":"Scheduled"}'

# 3. Create preventive maintenance work order
curl -X POST "http://localhost:5112/api/WorkOrders" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -d '{"title":"Pre-flight Inspection","description":"Standard pre-flight safety inspection","aircraftId":4,"technicianId":1,"priority":"High","type":"Inspection","estimatedHours":2,"scheduledDate":"2024-01-31T08:00:00Z"}'
```

### Scenario 2: Maintenance Workflow
```bash
# 1. Get all work orders for an aircraft
curl -X GET "http://localhost:5112/api/WorkOrders/by-aircraft/1"

# 2. Get available technicians with specific specialization
curl -X GET "http://localhost:5112/api/Technicians/by-specialization/Engine"

# 3. Update work order status to In Progress
curl -X PUT "http://localhost:5112/api/WorkOrders/1" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -d '{"status":"InProgress","startedDate":"2024-01-01T09:00:00Z"}'

# 4. Complete the work order
curl -X PUT "http://localhost:5112/api/WorkOrders/1" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -d '{"status":"Completed","completedDate":"2024-01-01T15:00:00Z","actualHours":6}'
```

## 📊 Response Status Codes

| Status Code | Meaning | Common Scenarios |
|-------------|---------|------------------|
| 200 | OK | Successful GET, PUT operations |
| 201 | Created | Successful POST operations |
| 400 | Bad Request | Invalid input data, validation errors |
| 401 | Unauthorized | Missing or invalid JWT token |
| 404 | Not Found | Resource doesn't exist |
| 409 | Conflict | Duplicate registration numbers, constraint violations |
| 500 | Internal Server Error | Server-side errors |

## 🔍 Error Response Examples

### Validation Error (400)
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "RegistrationNumber": ["Registration number is required"],
    "Capacity": ["Capacity must be between 1 and 1000"]
  }
}
```

### Not Found Error (404)
```json
{
  "message": "Aircraft with ID 999 not found"
}
```

### Unauthorized Error (401)
```json
{
  "message": "Authorization required for this endpoint"
}
```

## 🛠️ Testing Tools

### Recommended Tools
1. **Swagger UI** (http://localhost:5112) - Built-in interactive testing
2. **Postman** - Advanced API testing with collections
3. **curl** - Command-line testing (examples above)
4. **HTTPie** - User-friendly command-line HTTP client

### Postman Collection
You can import these examples into Postman by creating a new collection and adding requests with the URLs and payloads shown above.

---

**💡 Pro Tip**: Use the Swagger UI for initial exploration and testing, then use curl or Postman for automated testing and scripting scenarios.
