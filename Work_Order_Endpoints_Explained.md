# Work Order Endpoints: Purpose, Usage, and Flight Integration

## Why Work Order Endpoints Were Added

### Business Need
The work order endpoints were added to bridge **flight operations** and **aircraft maintenance**. In aviation, maintenance needs often arise from flight operations:

1. **Pre-flight inspections** may reveal issues requiring immediate attention
2. **Post-flight reports** may indicate maintenance needs
3. **Flight incidents** require documented repair work
4. **Regulatory compliance** demands complete maintenance tracking

### Technical Purpose
- **Link maintenance to specific flights** that triggered the work
- **Create audit trails** connecting flight events to maintenance actions
- **Enable automated workflows** where flight issues automatically generate work orders
- **Provide comprehensive reporting** on flight-related maintenance

## How to Use the Work Order Endpoints

### Creating a Work Order for a Flight
Your curl example creates a work order linked to flight ID 1:

```bash
curl -X 'POST' \
  'http://localhost:5112/api/Flights/1/work-orders' \
  -H 'accept: text/plain' \
  -H 'Authorization: Bearer YOUR_JWT_TOKEN' \
  -H 'Content-Type: application/json' \
  -d '{
    "aircraftRegistration": "N123AB",
    "taskDescription": "Engine maintenance",
    "priority": 1,
    "assignedTechnician": "John Smith",
    "scheduledDate": "2025-01-15T10:00:00Z",
    "notes": "Routine maintenance"
  }'
```

**What this does**:
- Creates a work order specifically for flight ID 1
- Links the maintenance task to that flight's record
- Automatically prefixes the task description with "Flight {FlightNumber}: "
- Generates a unique work order number
- Sets the status to "Open"

### Retrieving Work Orders by Flight ID

**NEW ENDPOINT ADDED**: You can now retrieve all work orders for a specific flight:

```bash
curl -X 'GET' \
  'http://localhost:5112/api/Flights/1/work-orders' \
  -H 'accept: application/json' \
  -H 'Authorization: Bearer YOUR_JWT_TOKEN'
```

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
      "flightId": 1,
      "flightNumber": "UA123"
    }
  ]
}
```

## How Work Orders Connect with Flights

### Data Relationship
```
Flight (ID: 1, FlightNumber: "UA123")
    ↓ [creates work order for]
FlightWorkOrder (TaskDescription: "Flight UA123: Engine maintenance")
    ↓ [assigned to]
Aircraft (Registration: "N123AB")
    ↓ [performed by]
Technician ("John Smith")
```

### Connection Mechanism
1. **Flight ID Link**: Work orders created via `/api/Flights/{flightId}/work-orders` are linked to that specific flight
2. **Flight Number Embedding**: The flight number is automatically embedded in the task description
3. **Retrieval by Flight**: Use `/api/Flights/{flightId}/work-orders` to get all work orders for a flight
4. **Cross-Reference**: Work orders can be found both through flight endpoints and standalone work order endpoints

## Complete Workflow Example

### Scenario: Flight Issue Requires Maintenance

1. **Flight Arrives with Issue**
   ```bash
   # Get flight details
   curl -X 'GET' 'http://localhost:5112/api/Flights/1' \
     -H 'Authorization: Bearer YOUR_JWT_TOKEN'
   ```

2. **Create Work Order for the Flight**
   ```bash
   curl -X 'POST' 'http://localhost:5112/api/Flights/1/work-orders' \
     -H 'Authorization: Bearer YOUR_JWT_TOKEN' \
     -H 'Content-Type: application/json' \
     -d '{
       "aircraftRegistration": "N123AB",
       "taskDescription": "Hydraulic system leak detected during post-flight inspection",
       "priority": 2,
       "assignedTechnician": "Jane Smith",
       "scheduledDate": "2025-01-05T08:00:00Z",
       "notes": "High priority - affects flight safety"
     }'
   ```

3. **Retrieve All Work Orders for the Flight**
   ```bash
   curl -X 'GET' 'http://localhost:5112/api/Flights/1/work-orders' \
     -H 'Authorization: Bearer YOUR_JWT_TOKEN'
   ```

4. **Update Work Order Status** (using standalone work order endpoint)
   ```bash
   curl -X 'PUT' 'http://localhost:5112/api/WorkOrders/123' \
     -H 'Authorization: Bearer YOUR_JWT_TOKEN' \
     -H 'Content-Type: application/json' \
     -d '{
       "status": 1,
       "notes": "Work in progress - hydraulic lines being replaced"
     }'
   ```

5. **Complete Work Order**
   ```bash
   curl -X 'PUT' 'http://localhost:5112/api/WorkOrders/123' \
     -H 'Authorization: Bearer YOUR_JWT_TOKEN' \
     -H 'Content-Type: application/json' \
     -d '{
       "status": 2,
       "notes": "Hydraulic system repaired and tested - aircraft cleared for service"
     }'
   ```

## Why Two Work Order Controllers?

### `/api/WorkOrders` - Standalone Work Order Management
**Purpose**: General aircraft maintenance not tied to specific flights
- Scheduled periodic maintenance (100-hour inspections)
- Fleet-wide maintenance operations
- General aircraft repairs
- Maintenance planning and statistics

### `/api/Flights/{flightId}/work-orders` - Flight-Specific Work Orders
**Purpose**: Maintenance directly related to flight operations
- Issues discovered during pre-flight checks
- Post-flight maintenance requirements
- Flight incident repairs
- Maintenance needed before flight departure

## Key Benefits

1. **Traceability**: Every maintenance action can be traced back to the flight that triggered it
2. **Compliance**: Meets aviation regulatory requirements for maintenance documentation
3. **Integration**: Seamlessly connects flight operations with maintenance workflows
4. **Reporting**: Generate comprehensive reports on flight-related maintenance
5. **Automation**: Flight events can automatically trigger maintenance work orders

## Authentication Note

All endpoints require JWT authentication. Make sure to include your valid JWT token in the Authorization header:
```
Authorization: Bearer YOUR_JWT_TOKEN
```

This integrated approach ensures that maintenance activities are properly tracked, linked to their operational triggers, and managed efficiently within the broader flight operations system.