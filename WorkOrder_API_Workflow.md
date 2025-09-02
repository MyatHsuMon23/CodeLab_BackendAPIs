# WorkOrder API Workflow Documentation

## Overview

The WorkOrder API (`/api/WorkOrders`) provides comprehensive management of flight work orders, supporting the complete lifecycle from creation to completion. This API is designed for aircraft maintenance teams to track and manage maintenance tasks.

## API Endpoints

### 1. Get Work Orders (with Pagination)
**Endpoint:** `GET /api/WorkOrders`

**Description:** Retrieves a paginated list of work orders with optional filtering.

**Query Parameters:**
- `status` (optional): Filter by work order status (Open, InProgress, Completed, Cancelled, OnHold)
- `aircraftRegistration` (optional): Filter by aircraft registration number
- `page` (optional, default: 1): Page number for pagination
- `perPage` (optional, default: 10, max: 100): Number of items per page

**Response Format:**
```json
{
  "success": true,
  "message": "Work orders retrieved successfully",
  "data": [
    {
      "id": 1,
      "workOrderNumber": "WO-20241201-001",
      "aircraftRegistration": "N123AB",
      "taskDescription": "Engine inspection and oil change",
      "status": "Open",
      "priority": "High",
      "assignedTechnician": "John Smith",
      "createdDate": "2024-12-01T10:00:00Z",
      "scheduledDate": "2024-12-03T08:00:00Z",
      "completedDate": null,
      "notes": null,
      "createdBy": "admin"
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

### 2. Get Work Order by ID
**Endpoint:** `GET /api/WorkOrders/{id}`

**Description:** Retrieves a specific work order by its ID.

### 3. Create Work Order
**Endpoint:** `POST /api/WorkOrders`

**Description:** Creates a new work order.

**Request Body:**
```json
{
  "aircraftRegistration": "N123AB",
  "taskDescription": "Engine inspection and oil change",
  "priority": "High",
  "assignedTechnician": "John Smith",
  "scheduledDate": "2024-12-03T08:00:00Z",
  "notes": "Routine maintenance check"
}
```

### 4. Update Work Order
**Endpoint:** `PUT /api/WorkOrders/{id}`

**Description:** Updates an existing work order.

### 5. Delete Work Order
**Endpoint:** `DELETE /api/WorkOrders/{id}`

**Description:** Deletes a work order (Admin/Supervisor access only).

### 6. Get Statistics
**Endpoint:** `GET /api/WorkOrders/statistics`

**Description:** Retrieves work order statistics and metrics.

## WorkOrder Workflow

### 1. Work Order Creation Flow
```
1. User creates work order → POST /api/WorkOrders
2. System generates unique work order number (WO-YYYYMMDD-XXX)
3. Work order status set to "Open"
4. Work order assigned to technician
5. Scheduled date set for maintenance
```

### 2. Work Order Processing Flow
```
1. Open → InProgress (when technician starts work)
2. InProgress → Completed (when work is finished)
3. InProgress → OnHold (if work needs to be paused)
4. OnHold → InProgress (when work resumes)
5. Any Status → Cancelled (if work order is cancelled)
```

### 3. Work Order Lifecycle States

#### **Open**
- Initial state when work order is created
- Work has not yet started
- Can be assigned to technicians
- Can be scheduled for future dates

#### **InProgress**
- Technician has started working on the task
- Active work is being performed
- Progress can be tracked
- Notes can be added for updates

#### **Completed**
- Work has been finished successfully
- Completion date is automatically set
- Final notes and verification can be added
- No further changes allowed to core task details

#### **OnHold**
- Work is temporarily stopped
- Waiting for parts, approval, or other dependencies
- Can be resumed when ready
- Reason for hold should be documented in notes

#### **Cancelled**
- Work order has been cancelled
- No work will be performed
- Cancellation reason should be documented
- Cannot be resumed once cancelled

### 4. Priority Levels

- **Critical**: Immediate attention required, safety-related issues
- **High**: Important maintenance that should be completed soon
- **Medium**: Standard maintenance tasks
- **Low**: Non-urgent maintenance that can be scheduled flexibly

### 5. Authentication & Authorization

- **JWT Authentication**: All endpoints require valid JWT token
- **Role-based Access**: 
  - Standard Users: Can view, create, and update work orders
  - Supervisors: Can delete work orders and access all data
  - Admins: Full access to all operations

### 6. Integration with Flight Management

The WorkOrder API integrates with the Flight API through:
- Work orders can be associated with specific flights
- Flight maintenance requirements can trigger work order creation
- Work order completion affects flight readiness status

## Usage Examples

### Example 1: Get First Page of Work Orders
```bash
GET /api/WorkOrders?page=1&perPage=5
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Example 2: Filter by Status with Pagination
```bash
GET /api/WorkOrders?status=Open&page=1&perPage=10
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Example 3: Search by Aircraft Registration
```bash
GET /api/WorkOrders?aircraftRegistration=N123&page=1&perPage=20
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Example 4: Create High Priority Work Order
```bash
POST /api/WorkOrders
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json

{
  "aircraftRegistration": "N123AB",
  "taskDescription": "Emergency brake system inspection",
  "priority": "Critical",
  "assignedTechnician": "Jane Smith",
  "scheduledDate": "2024-12-01T14:00:00Z"
}
```

### Example 5: Update Work Order Status
```bash
PUT /api/WorkOrders/1
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json

{
  "id": 1,
  "workOrderNumber": "WO-20241201-001",
  "aircraftRegistration": "N123AB",
  "taskDescription": "Emergency brake system inspection",
  "status": "InProgress",
  "priority": "Critical",
  "assignedTechnician": "Jane Smith",
  "scheduledDate": "2024-12-01T14:00:00Z",
  "notes": "Started brake inspection - initial findings normal"
}
```

### Example 6: Get Work Order Statistics
```bash
GET /api/WorkOrders/statistics
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

## Error Handling

The API returns standard HTTP status codes:
- `200 OK`: Successful operation
- `201 Created`: Work order created successfully
- `400 Bad Request`: Invalid request data
- `401 Unauthorized`: Authentication required
- `403 Forbidden`: Insufficient permissions
- `404 Not Found`: Work order not found
- `500 Internal Server Error`: Server error

All error responses include descriptive messages in the response body to help with troubleshooting.