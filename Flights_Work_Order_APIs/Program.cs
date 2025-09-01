using Microsoft.EntityFrameworkCore;
using Flights_Work_Order_APIs.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<FlightWorkOrderContext>(options =>
    options.UseInMemoryDatabase("FlightWorkOrderDB"));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo 
    { 
        Title = "Flight Work Order APIs", 
        Version = "v1",
        Description = "A comprehensive API for managing flights and work orders for aircraft maintenance"
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Flight Work Order APIs v1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
    });
    
    // Seed the database with sample data
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<FlightWorkOrderContext>();
        await DataSeeder.SeedDataAsync(context);
    }
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
