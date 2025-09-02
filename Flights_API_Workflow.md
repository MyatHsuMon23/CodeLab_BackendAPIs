# Flights API Workflow Documentation

## Overview

The Flights API (`/api/Flights`) provides comprehensive flight management capabilities, including flight data retrieval, bulk import operations, and flight command management. This API is designed for aviation systems to manage flight schedules, imports, and operational commands.

## API Endpoints

### 1. Get Flights (with Pagination and Filtering)
**Endpoint:** `GET /api/Flights`

**Description:** Retrieves a paginated list of flights with optional filtering and sorting.

**Query Parameters:**
- `flightNumber` (optional): Filter by flight number (partial match)
- `sortBy` (optional, default: "FlightNumber"): Sort field ("FlightNumber", "ScheduledArrivalTime", "OriginAirport", "DestinationAirport")
- `sortDescending` (optional, default: false): Sort direction
- `page` (optional, default: 1): Page number for pagination
- `perPage` (optional, default: 10, max: 100): Number of items per page

**Response Format:**
```json
{
  "success": true,
  "message": "Flights retrieved successfully",
  "data": [
    {
      "id": 1,
      "flightNumber": "UA123",
      "scheduledArrivalTimeUtc": "2024-12-01T15:30:00Z",
      "originAirport": "LAX",
      "destinationAirport": "JFK"
    }
  ],
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

### 2. Get Flight by ID
**Endpoint:** `GET /api/Flights/{id}`

**Description:** Retrieves a specific flight by its unique ID.

**Path Parameters:**
- `id` (required): Flight ID

**Response Format:**
```json
{
  "success": true,
  "message": "Flight retrieved successfully",
  "data": {
    "id": 1,
    "flightNumber": "UA123",
    "scheduledArrivalTimeUtc": "2024-12-01T15:30:00Z",
    "originAirport": "LAX",
    "destinationAirport": "JFK"
  }
}
```

### 3. Import Multiple Flights (JSON)
**Endpoint:** `POST /api/Flights/import`

**Description:** Imports multiple flights from a JSON array.

**Request Body:**
```json
[
  {
    "flightNumber": "UA123",
    "scheduledArrivalTimeUtc": "2024-12-01T15:30:00Z",
    "originAirport": "LAX",
    "destinationAirport": "JFK"
  },
  {
    "flightNumber": "AA456",
    "scheduledArrivalTimeUtc": "2024-12-01T16:00:00Z",
    "originAirport": "ORD",
    "destinationAirport": "MIA"
  }
]
```

**Response Format:**
```json
{
  "success": true,
  "message": "Import completed",
  "data": {
    "importedCount": 2,
    "errors": []
  }
}
```

### 4. Import Flights from CSV File
**Endpoint:** `POST /api/Flights/import/csv`

**Description:** Imports flights from CSV or JSON file upload.

**Request:** Multipart form data with file upload

**CSV Format:**
```
FlightNumber,ScheduledArrivalTimeUtc,OriginAirport,DestinationAirport
UA123,2024-12-01T15:30:00Z,LAX,JFK
AA456,2024-12-01T16:00:00Z,ORD,MIA
```

### 5. Add Commands to Flight
**Endpoint:** `POST /api/Flights/{flightId}/commands`

**Description:** Adds commands/instructions to a specific flight.

**Path Parameters:**
- `flightId` (required): Flight ID

**Request Body:**
```json
{
  "command": "CHANGE_GATE",
  "parameters": {
    "newGate": "A15",
    "reason": "Gate reassignment due to maintenance"
  }
}
```

### 6. Get Flight Commands
**Endpoint:** `GET /api/Flights/{flightId}/commands`

**Description:** Retrieves all commands associated with a specific flight.

**Path Parameters:**
- `flightId` (required): Flight ID

### 7. Validate Command
**Endpoint:** `POST /api/Flights/commands/validate`

**Description:** Validates a flight command before execution.

**Request Body:**
```json
{
  "commandString": "CHANGE_GATE A15"
}
```

## Flight Management Workflow

### 1. Flight Data Import Flow
```
1. Prepare flight data (CSV/JSON format)
2. Upload via POST /api/Flights/import or /api/Flights/import/csv
3. System validates data format
4. System processes each flight record
5. Returns import summary with success count and errors
6. Failed records are logged with specific error messages
```

### 2. Flight Query and Filtering Flow
```
1. Client requests flights → GET /api/Flights
2. System applies optional filters (flightNumber)
3. System applies sorting (by specified field and direction)
4. System calculates pagination
5. System returns paginated results with metadata
```

### 3. Flight Command Management Flow
```
1. Validate command → POST /api/Flights/commands/validate
2. If valid, add to flight → POST /api/Flights/{flightId}/commands
3. System processes command
4. Command stored with flight record
5. Retrieve commands → GET /api/Flights/{flightId}/commands
```

## Data Validation Rules

### Flight Data Requirements
- **FlightNumber**: Required, string, unique identifier
- **ScheduledArrivalTimeUtc**: Required, valid DateTime in UTC
- **OriginAirport**: Required, airport code (typically 3-letter IATA)
- **DestinationAirport**: Required, airport code (typically 3-letter IATA)

### Import File Validation
- **CSV Files**: Must have header row with exact column names
- **JSON Files**: Must be valid JSON array of flight objects
- **File Size**: Limited by server configuration
- **Data Types**: All fields must match expected types

### Command Validation
- Commands must follow predefined format
- Parameters must be valid for command type
- Flight must exist before adding commands

## Error Handling

### Common Error Responses
- `200 OK`: Successful operation
- `400 Bad Request`: Invalid request data or validation failure
- `401 Unauthorized`: Authentication required
- `404 Not Found`: Flight not found
- `500 Internal Server Error`: Server error during processing

### Import Error Handling
```json
{
  "success": true,
  "message": "Import completed with errors",
  "data": {
    "importedCount": 8,
    "errors": [
      "Line 3: Invalid date format",
      "Line 7: Missing required field: OriginAirport"
    ]
  }
}
```

## Authentication & Authorization

- **JWT Authentication**: Required for all endpoints
- **Token Format**: Bearer token in Authorization header
- **Token Validation**: Server validates token signature and expiry
- **Role-based Access**: All authenticated users have access to flight operations

## Integration Points

### With Work Orders
- Flight data can trigger work order creation
- Work orders can be associated with specific flights
- Flight maintenance status affects work order scheduling

### With Command System
- Flight commands can trigger automated processes
- Commands are logged for audit purposes
- Command validation prevents invalid operations

## Usage Examples

### Example 1: Get Flights with Filtering
```bash
GET /api/Flights?flightNumber=UA&page=1&perPage=20&sortBy=ScheduledArrivalTime&sortDescending=true
```

### Example 2: Import Flights from JSON
```bash
POST /api/Flights/import
Content-Type: application/json

[
  {
    "flightNumber": "DL789",
    "scheduledArrivalTimeUtc": "2024-12-02T09:15:00Z",
    "originAirport": "ATL",
    "destinationAirport": "BOS"
  }
]
```

### Example 3: Add Flight Command
```bash
POST /api/Flights/123/commands
Content-Type: application/json

{
  "command": "DELAY_FLIGHT",
  "parameters": {
    "delayMinutes": 30,
    "reason": "Weather conditions"
  }
}
```

## Best Practices

### Data Import
1. **Validate data locally** before uploading large files
2. **Use CSV format** for large datasets (better performance)
3. **Monitor import results** and handle errors appropriately
4. **Batch processing**: Split very large files into smaller chunks

### API Usage
1. **Use pagination** for large result sets
2. **Implement proper error handling** for all API calls
3. **Cache frequently accessed data** when appropriate
4. **Use appropriate filters** to reduce data transfer

### Performance Considerations
1. **Pagination**: Always use reasonable page sizes (≤100 items)
2. **Filtering**: Use specific filters to reduce result sets
3. **Sorting**: Be aware that some sort operations may be more expensive
4. **Indexing**: Database is optimized for common query patterns