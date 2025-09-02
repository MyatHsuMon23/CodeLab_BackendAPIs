# Authentication API Workflow Documentation

## Overview

The Authentication API (`/api/Auth`) provides secure authentication services using JWT (JSON Web Tokens) with password encryption. This API handles user login, token generation, and password security operations for the Flight Work Order system.

## API Endpoints

### 1. User Login
**Endpoint:** `POST /api/Auth/login`

**Description:** Authenticates users and generates JWT access and refresh tokens.

**Request Body:**
```json
{
  "username": "admin",
  "password": "encrypted_password_string"
}
```

**Response Format:**
```json
{
  "success": true,
  "message": "Login successful",
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "refresh_token_string",
    "expiresAt": "2024-12-01T16:00:00Z",
    "tokenType": "Bearer"
  }
}
```

### 2. Encrypt Password (Development Helper)
**Endpoint:** `POST /api/Auth/encrypt-password`

**Description:** Helper endpoint to encrypt passwords for testing purposes. Should be removed in production.

**Request Body:**
```json
"plain_text_password"
```

**Response Format:**
```json
{
  "success": true,
  "message": "Password encrypted successfully",
  "data": "encrypted_password_string"
}
```

## Authentication Workflow

### 1. User Registration/Setup Flow
```
1. Admin creates user account with plain text password
2. System encrypts password using encryption service
3. Encrypted password stored in user database
4. User credentials are ready for authentication
```

### 2. Login Authentication Flow
```
1. Client sends username and encrypted password → POST /api/Auth/login
2. System decrypts the provided password
3. System validates decrypted password against stored credentials
4. If valid, system generates JWT claims with user info
5. System creates access token and refresh token
6. System returns tokens with expiry information
7. Client stores tokens for subsequent API calls
```

### 3. API Request Authorization Flow
```
1. Client includes Bearer token in Authorization header
2. System validates JWT signature and expiry
3. System extracts user claims from token
4. System authorizes request based on user role
5. If valid, request proceeds to controller
6. If invalid, returns 401 Unauthorized
```

### 4. Token Management Flow
```
1. Access token has limited lifespan
2. Client monitors token expiry
3. Before expiry, client can use refresh token
4. System generates new access token
5. Client updates stored token
6. Process repeats for continued access
```

## Security Features

### 1. Password Encryption
- **Two-way encryption**: Passwords can be decrypted for validation
- **Encryption service**: Centralized encryption/decryption handling
- **Key management**: Encryption keys managed securely
- **Error handling**: Failed decryption indicates invalid format

### 2. JWT Token Security
- **Signature validation**: Tokens are cryptographically signed
- **Expiry enforcement**: Tokens have configurable expiration
- **Claims-based**: Tokens contain user identity and role information
- **Stateless**: No server-side session storage required

### 3. Authorization Levels
- **Bearer token required**: All protected endpoints require valid JWT
- **Role-based access**: User roles determine endpoint access
- **Claim validation**: User claims verified on each request
- **Automatic token validation**: Framework handles token verification

## JWT Claims Structure

### Standard Claims
```json
{
  "username": "admin",
  "role": "Administrator",
  "userId": "admin",
  "iat": 1701234567,
  "exp": 1701320967,
  "iss": "FlightWorkOrderSystem",
  "aud": "FlightWorkOrderClient"
}
```

### Custom Claims
- **username**: User's login name
- **role**: User's role (Administrator, Supervisor, Technician)
- **userId**: Unique user identifier

## Error Handling

### Authentication Errors
```json
{
  "success": false,
  "message": "Invalid username or password",
  "data": null
}
```

### Common Error Responses
- `200 OK`: Successful authentication
- `400 Bad Request`: Invalid request format or missing required fields
- `401 Unauthorized`: Invalid credentials or expired token
- `500 Internal Server Error`: System error during authentication

### Detailed Error Scenarios

#### Invalid Password Format
```json
{
  "success": false,
  "message": "Invalid password format",
  "data": null
}
```

#### Missing Credentials
```json
{
  "success": false,
  "message": "Invalid request data",
  "data": null
}
```

#### System Error
```json
{
  "success": false,
  "message": "An error occurred during login",
  "data": null
}
```

## Configuration

### JWT Settings
- **Secret Key**: Configured in appsettings.json
- **Issuer**: "FlightWorkOrderSystem"
- **Audience**: "FlightWorkOrderClient"
- **Token Lifetime**: Configurable expiration time
- **Clock Skew**: Zero tolerance for time differences

### Encryption Settings
- **Encryption Algorithm**: Configured in encryption service
- **Key Storage**: Secure key management
- **Error Handling**: Graceful handling of encryption/decryption failures

## Usage Examples

### Example 1: User Login
```bash
POST /api/Auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "encrypted_admin_password"
}
```

### Example 2: Using Access Token
```bash
GET /api/WorkOrders
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Example 3: Encrypt Password for Testing
```bash
POST /api/Auth/encrypt-password
Content-Type: application/json

"my_plain_password"
```

## Security Best Practices

### Client-Side Security
1. **Secure Token Storage**: Store tokens securely (not in localStorage)
2. **Token Rotation**: Implement token refresh before expiry
3. **HTTPS Only**: Always use HTTPS for authentication requests
4. **Error Handling**: Don't expose sensitive error details to users

### Server-Side Security
1. **Strong Encryption**: Use robust encryption algorithms
2. **Key Rotation**: Regularly rotate encryption keys
3. **Rate Limiting**: Implement login attempt rate limiting
4. **Audit Logging**: Log authentication events for security monitoring

### Development vs Production
1. **Remove Helper Endpoints**: Remove encrypt-password endpoint in production
2. **Secure Configuration**: Use environment variables for secrets
3. **HTTPS Enforcement**: Require HTTPS in production
4. **Token Expiry**: Use shorter token lifetimes in production

## Integration with Other APIs

### Protected Endpoints
All other APIs (WorkOrders, Flights, WeatherForecast) require authentication:
- Include Bearer token in Authorization header
- Token must be valid and not expired
- User role determines access level

### Token Validation Flow
```
1. Request hits protected endpoint
2. Framework extracts Bearer token from header
3. JWT validation middleware processes token
4. If valid, request proceeds with user context
5. If invalid, returns 401 Unauthorized immediately
```

## Troubleshooting

### Common Issues

#### Login Fails with Valid Credentials
- Check password encryption format
- Verify encryption service configuration
- Check user exists in system
- Review server logs for detailed errors

#### Token Validation Fails
- Verify token format and Bearer prefix
- Check token expiration
- Validate JWT configuration matches
- Ensure HTTPS is used if required

#### Password Encryption Issues
- Verify encryption service is properly configured
- Check encryption key availability
- Review error logs for encryption failures
- Ensure password format is correct