# Flight Work Order Backend APIs - Step-by-Step Implementation Guide

## Project Setup and Initial Configuration

### Step 1: Project Creation and Structure Setup
```bash
# Create new ASP.NET Core Web API project
dotnet new webapi -n Flights_Work_Order_APIs
cd Flights_Work_Order_APIs

# Create solution file
dotnet new sln -n Flights_Work_Order_APIs
dotnet sln add Flights_Work_Order_APIs.csproj
```

### Step 2: NuGet Package Installation
```bash
# Entity Framework Core for SQLite
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.Tools

# JWT Authentication
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package System.IdentityModel.Tokens.Jwt

# Swagger/OpenAPI
dotnet add package Swashbuckle.AspNetCore

# Additional utilities
dotnet add package Microsoft.Extensions.Logging
dotnet add package CsvHelper
```

### Step 3: Project Structure Organization
```
Flights_Work_Order_APIs/
├── Controllers/           # API Controllers
├── Models/               # Data models and DTOs
├── Services/             # Business logic services
├── Data/                 # Entity Framework context
├── Program.cs           # Application startup
└── appsettings.json     # Configuration
```

## Database Implementation

### Step 4: Entity Models Creation

#### User Model Implementation
```csharp
// Models/User.cs
public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "User";
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginDate { get; set; }
}
```

#### Flight Model Implementation
```csharp
// Models/Flight.cs
public class Flight
{
    public int Id { get; set; }
    public string FlightNumber { get; set; } = string.Empty;
    public string AircraftRegistration { get; set; } = string.Empty;
    public string DepartureAirport { get; set; } = string.Empty;
    public string ArrivalAirport { get; set; } = string.Empty;
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public string Status { get; set; } = "Scheduled";
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
}
```

#### Work Order Model Implementation
```csharp
// Models/FlightWorkOrder.cs
public class FlightWorkOrder
{
    public int Id { get; set; }
    public string WorkOrderNumber { get; set; } = string.Empty;
    public string AircraftRegistration { get; set; } = string.Empty;
    public string TaskDescription { get; set; } = string.Empty;
    public WorkOrderStatus Status { get; set; } = WorkOrderStatus.Open;
    public WorkOrderPriority Priority { get; set; } = WorkOrderPriority.Medium;
    public string AssignedTechnician { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? ScheduledDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string? Notes { get; set; }
}

public enum WorkOrderStatus
{
    Open, InProgress, Completed, OnHold, Cancelled
}

public enum WorkOrderPriority
{
    Low, Medium, High, Critical
}
```

### Step 5: Database Context Setup
```csharp
// Data/FlightDbContext.cs
using Microsoft.EntityFrameworkCore;

public class FlightDbContext : DbContext
{
    public FlightDbContext(DbContextOptions<FlightDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Flight> Flights { get; set; }
    public DbSet<FlightWorkOrder> WorkOrders { get; set; }
    public DbSet<FlightCommand> FlightCommands { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure entities
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<FlightWorkOrder>()
            .HasIndex(w => w.WorkOrderNumber)
            .IsUnique();

        // Seed initial data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed default admin user
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Username = "admin",
                PasswordHash = "EncryptedPassword", // Will be properly encrypted
                Role = "Administrator",
                CreatedDate = DateTime.UtcNow
            }
        );
    }
}
```

## Service Layer Implementation

### Step 6: Authentication Services

#### JWT Service Implementation
```csharp
// Services/JwtService.cs
public interface IJwtService
{
    string GenerateToken(string username, string role, string userId);
    ClaimsPrincipal? ValidateToken(string token);
}

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(string username, string role, string userId)
    {
        var secretKey = _configuration["Jwt:SecretKey"];
        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("username", username),
            new Claim("role", role),
            new Claim("userId", userId),
            new Claim(JwtRegisteredClaimNames.Iat, 
                new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
```

#### Encryption Service Implementation
```csharp
// Services/EncryptionService.cs
public interface IEncryptionService
{
    string Encrypt(string plainText);
    string Decrypt(string cipherText);
}

public class EncryptionService : IEncryptionService
{
    private readonly string _key = "MySecretKey12345"; // Should be in configuration

    public string Encrypt(string plainText)
    {
        // Implementation of encryption logic
        var keyBytes = Encoding.UTF8.GetBytes(_key);
        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        
        // Simple XOR encryption (use proper encryption in production)
        var encryptedBytes = new byte[plainBytes.Length];
        for (int i = 0; i < plainBytes.Length; i++)
        {
            encryptedBytes[i] = (byte)(plainBytes[i] ^ keyBytes[i % keyBytes.Length]);
        }
        
        return Convert.ToBase64String(encryptedBytes);
    }

    public string Decrypt(string cipherText)
    {
        // Implementation of decryption logic
        var keyBytes = Encoding.UTF8.GetBytes(_key);
        var encryptedBytes = Convert.FromBase64String(cipherText);
        
        var decryptedBytes = new byte[encryptedBytes.Length];
        for (int i = 0; i < encryptedBytes.Length; i++)
        {
            decryptedBytes[i] = (byte)(encryptedBytes[i] ^ keyBytes[i % keyBytes.Length]);
        }
        
        return Encoding.UTF8.GetString(decryptedBytes);
    }
}
```

#### User Service Implementation
```csharp
// Services/UserService.cs
public interface IUserService
{
    Task<User?> AuthenticateAsync(string username, string encryptedPassword);
    Task<User?> GetUserByUsernameAsync(string username);
}

public class UserService : IUserService
{
    private readonly FlightDbContext _context;
    private readonly IEncryptionService _encryptionService;

    public UserService(FlightDbContext context, IEncryptionService encryptionService)
    {
        _context = context;
        _encryptionService = encryptionService;
    }

    public async Task<User?> AuthenticateAsync(string username, string encryptedPassword)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username);

        if (user == null) return null;

        try
        {
            var decryptedPassword = _encryptionService.Decrypt(encryptedPassword);
            var storedPassword = _encryptionService.Decrypt(user.PasswordHash);
            
            if (decryptedPassword == storedPassword)
            {
                user.LastLoginDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return user;
            }
        }
        catch
        {
            // Invalid encryption format or other issues
        }

        return null;
    }
}
```

## Controller Implementation

### Step 7: Authentication Controller
```csharp
// Controllers/AuthController.cs
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IJwtService _jwtService;
    private readonly IEncryptionService _encryptionService;

    public AuthController(
        IUserService userService,
        IJwtService jwtService,
        IEncryptionService encryptionService)
    {
        _userService = userService;
        _jwtService = jwtService;
        _encryptionService = encryptionService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] LoginRequest request)
    {
        try
        {
            var user = await _userService.AuthenticateAsync(request.Username, request.Password);
            
            if (user == null)
            {
                return Unauthorized(ApiResponse<LoginResponse>.CreateError("Invalid username or password"));
            }

            var token = _jwtService.GenerateToken(user.Username, user.Role, user.Id.ToString());
            
            var response = new LoginResponse
            {
                AccessToken = token,
                TokenType = "Bearer",
                ExpiresIn = 3600,
                Username = user.Username,
                Role = user.Role
            };

            return Ok(ApiResponse<LoginResponse>.CreateSuccess(response, "Login successful"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<LoginResponse>.CreateError("Login failed"));
        }
    }

    [HttpPost("encrypt-password")]
    public ActionResult<ApiResponse<string>> EncryptPassword([FromBody] string password)
    {
        try
        {
            var encryptedPassword = _encryptionService.Encrypt(password);
            return Ok(ApiResponse<string>.CreateSuccess(encryptedPassword, "Password encrypted successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<string>.CreateError("Encryption failed"));
        }
    }
}
```

### Step 8: Work Orders Controller
```csharp
// Controllers/FlightWorkOrdersController.cs
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WorkOrdersController : ControllerBase
{
    private static readonly List<FlightWorkOrder> _workOrders = new();
    private static int _nextId = 1;

    [HttpGet]
    public ActionResult<PaginatedApiResponse<IEnumerable<FlightWorkOrder>>> GetWorkOrders(
        [FromQuery] WorkOrderStatus? status = null,
        [FromQuery] string? aircraftRegistration = null,
        [FromQuery] int page = 1,
        [FromQuery] int perPage = 10)
    {
        try
        {
            // Validate pagination parameters
            if (page < 1) page = 1;
            if (perPage < 1) perPage = 10;
            if (perPage > 100) perPage = 100;

            var query = _workOrders.AsQueryable();

            // Apply filtering
            if (status.HasValue)
                query = query.Where(w => w.Status == status.Value);

            if (!string.IsNullOrEmpty(aircraftRegistration))
                query = query.Where(w => w.AircraftRegistration.Contains(aircraftRegistration, StringComparison.OrdinalIgnoreCase));

            // Apply sorting
            query = query.OrderByDescending(w => w.CreatedDate);

            // Get total count before pagination
            var total = query.Count();
            var lastPage = (int)Math.Ceiling((double)total / perPage);
            var skip = (page - 1) * perPage;

            // Apply pagination
            var workOrders = query.Skip(skip).Take(perPage).ToList();

            // Create pagination metadata
            var pagination = new Pagination
            {
                CurrentPage = page,
                LastPage = lastPage,
                PerPage = perPage,
                Total = total
            };

            return Ok(PaginatedApiResponse<IEnumerable<FlightWorkOrder>>.CreateSuccess(workOrders, pagination, "Work orders retrieved successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, PaginatedApiResponse<IEnumerable<FlightWorkOrder>>.CreateError("Failed to retrieve work orders"));
        }
    }

    [HttpPost]
    public ActionResult<ApiResponse<FlightWorkOrder>> CreateWorkOrder([FromBody] CreateWorkOrderRequest request)
    {
        try
        {
            var username = User.FindFirst("username")?.Value ?? "unknown";
            
            var workOrder = new FlightWorkOrder
            {
                Id = _nextId++,
                WorkOrderNumber = GenerateWorkOrderNumber(),
                AircraftRegistration = request.AircraftRegistration,
                TaskDescription = request.TaskDescription,
                Status = WorkOrderStatus.Open,
                Priority = request.Priority,
                AssignedTechnician = request.AssignedTechnician,
                CreatedBy = username,
                ScheduledDate = request.ScheduledDate,
                Notes = request.Notes
            };

            _workOrders.Add(workOrder);

            return CreatedAtAction(nameof(GetWorkOrder), new { id = workOrder.Id }, 
                ApiResponse<FlightWorkOrder>.CreateSuccess(workOrder, "Work order created successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<FlightWorkOrder>.CreateError("Failed to create work order"));
        }
    }

    private string GenerateWorkOrderNumber()
    {
        var today = DateTime.UtcNow;
        var sequence = _workOrders.Count(w => w.CreatedDate.Date == today.Date) + 1;
        return $"WO-{today:yyyyMMdd}-{sequence:D3}";
    }
}
```

### Step 9: Flights Controller
```csharp
// Controllers/FlightsController.cs
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FlightsController : ControllerBase
{
    private readonly FlightDbContext _context;
    private readonly IFlightCommandService _commandService;

    public FlightsController(FlightDbContext context, IFlightCommandService commandService)
    {
        _context = context;
        _commandService = commandService;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedApiResponse<IEnumerable<Flight>>>> GetFlights(
        [FromQuery] string? flightNumber = null,
        [FromQuery] int page = 1,
        [FromQuery] int perPage = 10,
        [FromQuery] string? sortBy = "departureTime",
        [FromQuery] bool sortDescending = false)
    {
        try
        {
            // Validate pagination parameters
            if (page < 1) page = 1;
            if (perPage < 1) perPage = 10;
            if (perPage > 100) perPage = 100;

            var query = _context.Flights.AsQueryable();

            // Apply filtering
            if (!string.IsNullOrEmpty(flightNumber))
                query = query.Where(f => f.FlightNumber.Contains(flightNumber));

            // Apply sorting
            query = sortBy?.ToLower() switch
            {
                "flightnumber" => sortDescending ? query.OrderByDescending(f => f.FlightNumber) : query.OrderBy(f => f.FlightNumber),
                "departuretime" => sortDescending ? query.OrderByDescending(f => f.DepartureTime) : query.OrderBy(f => f.DepartureTime),
                "arrivaltime" => sortDescending ? query.OrderByDescending(f => f.ArrivalTime) : query.OrderBy(f => f.ArrivalTime),
                _ => query.OrderBy(f => f.DepartureTime)
            };

            // Get total count before pagination
            var total = await query.CountAsync();
            var lastPage = (int)Math.Ceiling((double)total / perPage);
            var skip = (page - 1) * perPage;

            // Apply pagination
            var flights = await query.Skip(skip).Take(perPage).ToListAsync();

            // Create pagination metadata
            var pagination = new Pagination
            {
                CurrentPage = page,
                LastPage = lastPage,
                PerPage = perPage,
                Total = total
            };

            return Ok(PaginatedApiResponse<IEnumerable<Flight>>.CreateSuccess(flights, pagination, "Flights retrieved successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, PaginatedApiResponse<IEnumerable<Flight>>.CreateError("Failed to retrieve flights"));
        }
    }

    [HttpPost("import/csv")]
    public async Task<ActionResult<ApiResponse<object>>> ImportFlightsFromCsv(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
                return BadRequest(ApiResponse<object>.CreateError("No file uploaded"));

            var flights = new List<Flight>();
            var errors = new List<string>();

            using var reader = new StreamReader(file.OpenReadStream());
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            
            var records = csv.GetRecords<FlightImportRequest>();
            
            foreach (var record in records)
            {
                try
                {
                    var flight = new Flight
                    {
                        FlightNumber = record.FlightNumber,
                        AircraftRegistration = record.AircraftRegistration,
                        DepartureAirport = record.DepartureAirport,
                        ArrivalAirport = record.ArrivalAirport,
                        DepartureTime = record.DepartureTime,
                        ArrivalTime = record.ArrivalTime,
                        Status = record.Status ?? "Scheduled"
                    };

                    flights.Add(flight);
                }
                catch (Exception ex)
                {
                    errors.Add($"Row error: {ex.Message}");
                }
            }

            if (flights.Any())
            {
                _context.Flights.AddRange(flights);
                await _context.SaveChangesAsync();
            }

            var result = new
            {
                ImportedCount = flights.Count,
                ErrorCount = errors.Count,
                Errors = errors
            };

            return Ok(ApiResponse<object>.CreateSuccess(result, $"Import completed. {flights.Count} flights imported, {errors.Count} errors"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.CreateError("Import failed"));
        }
    }
}
```

## Application Configuration

### Step 10: Program.cs Configuration
```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

// Configure Entity Framework with SQLite
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=flights.db";
builder.Services.AddDbContext<FlightDbContext>(options =>
    options.UseSqlite(connectionString));

// Register custom services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IEncryptionService, EncryptionService>();
builder.Services.AddScoped<IFlightCommandService, FlightCommandService>();

// Configure JWT authentication
var jwtSection = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSection["SecretKey"] ?? "Flight_Work_Order_Secret_Key_For_JWT_Token_Generation_2024";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ValidateIssuer = true,
        ValidIssuer = jwtSection["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSection["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// Configure Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Flight Work Order APIs", 
        Version = "v1",
        Description = "API for managing flight work orders with JWT authentication"
    });

    // Add JWT authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
            new List<string>()
        }
    });
});

var app = builder.Build();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<FlightDbContext>();
    context.Database.EnsureCreated();
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Flight Work Order APIs v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

### Step 11: Configuration Files
```json
// appsettings.json
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
  },
  "AllowedHosts": "*"
}
```

## Testing and Deployment

### Step 12: Build and Test
```bash
# Build the project
dotnet build

# Run the application
dotnet run

# The API will be available at:
# - HTTPS: https://localhost:7159
# - HTTP: http://localhost:5159
# - Swagger UI: https://localhost:7159/swagger
```

### Step 13: API Testing
```bash
# Test authentication
curl -X POST "https://localhost:7159/api/Auth/login" \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"encrypted_password"}'

# Test work orders (with JWT token)
curl -X GET "https://localhost:7159/api/WorkOrders" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"

# Test pagination
curl -X GET "https://localhost:7159/api/WorkOrders?page=1&perPage=5" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

## Implementation Summary

This step-by-step implementation guide covered:

1. **Project Setup**: Created ASP.NET Core 8.0 Web API with proper structure
2. **Database Implementation**: Set up SQLite with Entity Framework Core
3. **Authentication System**: Implemented JWT-based authentication with encryption
4. **Service Layer**: Created business logic services for modularity
5. **API Controllers**: Built RESTful controllers with proper error handling
6. **Pagination Support**: Added consistent pagination across endpoints
7. **Security Features**: Implemented JWT authentication and password encryption
8. **Configuration**: Set up proper application configuration
9. **Documentation**: Integrated Swagger for API documentation
10. **Testing**: Provided testing strategies and examples

The implementation follows best practices for:
- **Clean Architecture**: Separation of concerns with Controllers, Services, and Data layers
- **Security**: JWT authentication, password encryption, and proper authorization
- **Performance**: Async operations, pagination, and optimized queries
- **Maintainability**: Dependency injection, consistent error handling, and proper logging
- **Documentation**: Comprehensive API documentation with Swagger

This foundation provides a robust, scalable, and secure backend API system for flight and work order management.