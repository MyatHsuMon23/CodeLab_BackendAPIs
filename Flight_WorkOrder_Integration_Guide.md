# Flight Work Order Integration Guide

## Overview

This guide explains the work order endpoints, their purpose, and how they integrate with the flight management system. The API provides two complementary work order systems designed to handle different maintenance scenarios.

## Work Order Systems Architecture

### 1. Standalone Work Order System (`/api/WorkOrders`)
**Purpose**: General aircraft maintenance management independent of specific flights.

**Use Cases**:
- Scheduled periodic maintenance (annual inspections, 100-hour checks)
- General aircraft repairs not tied to a specific flight
- Proactive maintenance planning
- Fleet-wide maintenance operations

### 2. Flight-Specific Work Order System (`/api/Flights/{flightId}/work-orders`)
**Purpose**: Maintenance tasks directly triggered by or related to specific flights.

**Use Cases**:
- Issues discovered during pre-flight inspections
- Post-flight maintenance requirements
- Flight-specific incident repairs
- Maintenance needed before a specific flight can depart

## Why Work Orders Were Added

### Business Requirements
1. **Safety Compliance**: Aviation regulations require comprehensive maintenance tracking
2. **Operational Efficiency**: Link maintenance needs directly to flight operations
3. **Audit Trail**: Complete documentation of all maintenance activities
4. **Resource Planning**: Better allocation of maintenance crews and resources
5. **Integration**: Connect flight operations with maintenance workflows

### Technical Benefits
1. **Traceability**: Link maintenance work directly to flights that triggered it
2. **Automation**: Flight events can automatically create maintenance tasks
3. **Reporting**: Generate maintenance reports based on flight operations
4. **Scheduling**: Coordinate maintenance timing with flight schedules

## API Endpoints Reference

### Flight-Specific Work Order Endpoints

#### 1. Create Work Order for Flight
```
POST /api/Flights/{flightId}/work-orders
```

**Purpose**: Creates a new work order specifically associated with a flight.

**Parameters**:
- `flightId` (path): The ID of the flight

**Request Body**:
```json
{
  "aircraftRegistration": "N123AB",
  "taskDescription": "Engine maintenance",
  "priority": "Medium",
  "assignedTechnician": "John Smith",
  "scheduledDate": "2025-01-15T10:00:00Z",
  "notes": "Routine maintenance required"
}
```

**Response**:
```json
{
  "success": true,
  "message": "Work order created for flight successfully",
  "data": {
    "workOrderId": 123,
    "workOrderNumber": "WO-20250104082244-456",
    "flightId": 1,
    "flightNumber": "UA123",
    "taskDescription": "Flight UA123: Engine maintenance",
    "priority": "Medium",
    "status": "Open",
    "createdBy": "maintenance_user",
    "createdDate": "2025-01-04T08:22:44Z"
  }
}
```

#### 2. Get Work Orders for Flight
```
POST /api/Flights/{flightId}/work-orders
```

**Purpose**: Creates a maintenance work order linked to a specific flight.

**Parameters**:
- `flightId` (path): The ID of the flight requiring maintenance

**Request Body**:
```json
{
  "aircraftRegistration": "N123AB",
  "taskDescription": "Engine maintenance",
  "priority": 1,
  "assignedTechnician": "John Smith",
  "scheduledDate": "2025-01-15T10:00:00Z",
  "notes": "Routine maintenance"
}
```

**Response**:
```json
{
  "success": true,
  "message": "Work order created for flight successfully",
  "data": {
    "workOrderId": 123,
    "workOrderNumber": "WO-20250104082244-456",
    "flightId": 1,
    "flightNumber": "UA123",
    "taskDescription": "Flight UA123: Engine maintenance",
    "priority": "Medium",
    "status": "Open",
    "createdBy": "maintenance_user",
    "createdDate": "2025-01-04T08:22:44Z"
  }
}
```

#### 2. Get Work Orders for Flight
```
GET /api/Flights/{flightId}/work-orders
```

**Purpose**: Retrieves all work orders associated with a specific flight.

**Parameters**:
- `flightId` (path): The ID of the flight

**Response**:
```json
{
  "success": true,
  "message": "Work orders for flight UA123 retrieved successfully",
  "data": [
    {
      "id": 123,
      "workOrderNumber": "WO-20250104082244-456",
      "aircraftRegistration": "N123AB",
      "taskDescription": "Flight UA123: Engine maintenance",
      "status": "Open",
      "priority": "Medium",
      "assignedTechnician": "John Smith",
      "createdDate": "2025-01-04T08:22:44Z",
      "scheduledDate": "2025-01-15T10:00:00Z",
      "completedDate": null,
      "notes": "Created for flight UA123. Routine maintenance",
      "createdBy": "maintenance_user",
      "flightId": 1,
      "flightNumber": "UA123"
    }
  ]
}
```

#### 3. Get Flight Commands/History
```
GET /api/Flights/{flightId}/commands
```

**Purpose**: Retrieves command history for a flight (operational commands, not maintenance work orders).

#### 4. Get All Flights with Work Orders and Commands
```
GET /api/Flights/with-work-orders?flightNumber=UA&page=1&perPage=10&sortBy=FlightNumber&sortDescending=false
```

**Purpose**: Retrieves a paginated list of all flights with their associated work orders and command submissions.

**Parameters**:
- `flightNumber` (query, optional): Filter flights by flight number (contains match)
- `page` (query, optional): Page number for pagination (default: 1)
- `perPage` (query, optional): Items per page (default: 10, max: 100)
- `sortBy` (query, optional): Sort field - FlightNumber, ScheduledArrivalTime, OriginAirport, DestinationAirport (default: FlightNumber)
- `sortDescending` (query, optional): Sort direction (default: false)

**Response**:
```json
{
  "success": true,
  "message": "Flights with work orders retrieved successfully",
  "data": [
    {
      "id": 1,
      "flightNumber": "UA123",
      "scheduledArrivalTimeUtc": "2025-01-15T14:30:00Z",
      "originAirport": "JFK",
      "destinationAirport": "LAX",
      "createdAt": "2025-01-04T08:00:00Z",
      "workOrders": [
        {
          "id": 123,
          "workOrderNumber": "WO-20250104082244-456",
          "aircraftRegistration": "N123AB",
          "taskDescription": "Flight UA123: Engine maintenance",
          "status": 0,
          "priority": 2,
          "assignedTechnician": "John Smith",
          "createdDate": "2025-01-04T08:22:44Z",
          "scheduledDate": "2025-01-15T10:00:00Z",
          "completedDate": null,
          "notes": "Created for flight UA123. Routine maintenance",
          "createdBy": "maintenance_user"
        }
      ],
      "commandSubmissions": [
        {
          "id": 1,
          "commandString": "CHECK_ENGINE,INSPECT_LANDING_GEAR",
          "parsedCommands": [
            {
              "type": "CHECK_ENGINE",
              "value": "",
              "displayText": "Check Engine",
              "isValid": true
            }
          ],
          "humanReadableCommands": "Check Engine, Inspect Landing Gear",
          "submittedAt": "2025-01-04T08:22:44Z",
          "submittedBy": "maintenance_user",
          "notes": "Pre-flight inspection commands"
        }
      ]
    }
  ],
  "pagination": {
    "currentPage": 1,
    "lastPage": 1,
    "perPage": 10,
    "total": 1
  }
}
```

### Standalone Work Order Endpoints

#### 1. Get All Work Orders
```
GET /api/WorkOrders?status=Open&aircraftRegistration=N123&page=1&perPage=10
```

#### 2. Create Standalone Work Order
```
POST /api/WorkOrders
```

#### 3. Update Work Order
```
PUT /api/WorkOrders/{id}
```

#### 4. Get Work Order Statistics
```
GET /api/WorkOrders/statistics
```

## Complete Usage Workflow

### Scenario 1: Flight-Triggered Maintenance

1. **Flight Operation**: A flight experiences an issue or requires maintenance
2. **Create Flight Work Order**: Use flight-specific endpoint to create work order
3. **Track Progress**: Use standalone work order endpoints to manage the work
4. **Retrieve by Flight**: Use flight work order retrieval to see all maintenance for that flight

### Scenario 2: Comprehensive Fleet Management

1. **Overview**: Get all flights with their maintenance status
2. **Use**: `GET /api/Flights/with-work-orders` to see comprehensive view
3. **Filter**: Use query parameters to focus on specific flights or areas
4. **Monitor**: Track work orders and command submissions across the fleet

#### Step-by-Step Example:

**Step 1: Create Work Order for Flight**
```bash
curl -X 'POST' \
  'http://localhost:5112/api/Flights/1/work-orders' \
  -H 'accept: application/json' \
  -H 'Authorization: Bearer YOUR_JWT_TOKEN' \
  -H 'Content-Type: application/json' \
  -d '{
    "aircraftRegistration": "N123AB",
    "taskDescription": "Pre-flight inspection revealed hydraulic leak",
    "priority": 2,
    "assignedTechnician": "John Smith",
    "scheduledDate": "2025-01-05T08:00:00Z",
    "notes": "High priority - flight departure delayed"
  }'
```

**Step 2: Retrieve Work Orders for Flight**
```bash
curl -X 'GET' \
  'http://localhost:5112/api/Flights/1/work-orders' \
  -H 'accept: application/json' \
  -H 'Authorization: Bearer YOUR_JWT_TOKEN'
```

**Step 3: Update Work Order Status**
```bash
curl -X 'PUT' \
  'http://localhost:5112/api/WorkOrders/123' \
  -H 'accept: application/json' \
  -H 'Authorization: Bearer YOUR_JWT_TOKEN' \
  -H 'Content-Type: application/json' \
  -d '{
    "status": 1,
    "notes": "Hydraulic system repaired and tested"
  }'
```

### Scenario 2: Scheduled Maintenance

1. **Create Standalone Work Order**: For scheduled maintenance not tied to a specific flight
2. **Assign to Aircraft**: Use aircraft registration to track
3. **Schedule Appropriately**: Coordinate with flight schedule

```bash
curl -X 'POST' \
  'http://localhost:5112/api/WorkOrders' \
  -H 'accept: application/json' \
  -H 'Authorization: Bearer YOUR_JWT_TOKEN' \
  -H 'Content-Type: application/json' \
  -d '{
    "aircraftRegistration": "N123AB",
    "taskDescription": "100-hour inspection",
    "priority": 1,
    "assignedTechnician": "Jane Doe",
    "scheduledDate": "2025-01-10T09:00:00Z",
    "notes": "Scheduled maintenance - book aircraft out of service"
  }'
```

## Data Flow and Relationships

### Flight → Work Order Relationship
```
Flight (ID: 1, FlightNumber: "UA123")
    ↓ (triggers maintenance)
FlightWorkOrder (ID: 123, TaskDescription: "Flight UA123: Engine maintenance")
    ↓ (links to)
Aircraft (Registration: "N123AB")
    ↓ (assigned to)
Technician ("John Smith")
```

### Work Order Lifecycle
```
1. Flight Issue Detected
   ↓
2. Create Flight Work Order (POST /api/Flights/{id}/work-orders)
   ↓
3. Work Order Created (Status: Open)
   ↓
4. Assign Technician & Schedule
   ↓
5. Update Status (PUT /api/WorkOrders/{id}) - InProgress
   ↓
6. Complete Work (Status: Completed)
   ↓
7. Flight Cleared for Operation
```

## Priority Levels Explained

| Priority | Value | Description | Use Case |
|----------|-------|-------------|----------|
| **Critical** | 3 | Immediate safety concern | Grounded aircraft, safety issues |
| **High** | 2 | Important, affects operations | Pre-flight issues, operational impact |
| **Medium** | 1 | Standard maintenance | Routine maintenance, minor repairs |
| **Low** | 0 | Non-urgent | Cosmetic issues, future planning |

## Status Workflow

| Status | Description | Next Actions |
|--------|-------------|--------------|
| **Open** | Work order created, not started | Assign technician, schedule work |
| **InProgress** | Work actively being performed | Continue work, update progress |
| **Completed** | Work finished successfully | Close work order, clear aircraft |
| **OnHold** | Work paused temporarily | Resume when ready, document reason |
| **Cancelled** | Work order cancelled | Document reason, archive |

## Integration Points

### 1. Flight Operations Integration
- Flight issues automatically trigger work order creation
- Work order completion updates flight readiness status
- Maintenance scheduling considers flight schedules

### 2. Aircraft Management Integration
- Work orders link to specific aircraft registrations
- Aircraft maintenance history tracked through work orders
- Fleet-wide maintenance planning and reporting

### 3. Resource Management Integration
- Technician assignment and workload management
- Parts and tools allocation for work orders
- Maintenance bay scheduling and resource allocation

## Best Practices

### 1. Work Order Creation
- **Always link to flights** when maintenance is flight-related
- **Use clear task descriptions** that identify the specific issue
- **Set appropriate priority** based on safety and operational impact
- **Include detailed notes** for better tracking and handoff

### 2. Status Management
- **Update status promptly** to reflect current work state
- **Complete work orders** when tasks are finished
- **Document completion** with final notes and verification

### 3. Resource Planning
- **Schedule maintenance** during aircraft downtime
- **Coordinate with flight operations** to minimize disruption
- **Plan for parts availability** before starting work

### 4. Safety and Compliance
- **Follow all regulatory requirements** for maintenance documentation
- **Maintain complete audit trail** of all maintenance activities
- **Ensure proper authorization** for all maintenance work

## Error Handling

### Common Error Scenarios
- **Flight not found** (404): Verify flight ID exists
- **Unauthorized** (401): Check JWT token validity
- **Invalid data** (400): Validate request body format
- **Server error** (500): Check logs for system issues

### Troubleshooting Tips
1. **Always authenticate** with valid JWT token
2. **Verify flight exists** before creating work orders
3. **Check data validation** for required fields
4. **Monitor logs** for detailed error information

## Summary

The work order system provides comprehensive maintenance management with two complementary approaches:

1. **Flight-specific work orders** for issues directly related to flight operations
2. **Standalone work orders** for general aircraft maintenance

This dual approach ensures all maintenance needs are captured and tracked while maintaining clear relationships between flight operations and maintenance activities.

Use the flight-specific endpoints when maintenance is triggered by or related to a specific flight, and use the standalone endpoints for general aircraft maintenance management.