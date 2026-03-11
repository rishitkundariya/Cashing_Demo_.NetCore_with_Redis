using CachingDemo.Application.Interfaces;
using CachingDemo.Infrastructure.cache;
using CachingDemo.Infrastructure.Data;
using CachingDemo.Infrastructure.Repositories;
using CachingDemo.Presentation.Endpoints;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register base repositories
builder.Services.AddScoped<DepartmentRepository>();
builder.Services.AddScoped<EmployeeRepository>();

// Register Redis cache BEFORE the cache repos that depend on it
var redisConnection = builder.Configuration.GetConnectionString("RedisConnection");
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnection;
    options.InstanceName = "Employee_"; 
});

// Register in-memory cache
builder.Services.AddMemoryCache();

// Register cache-decorated repositories
builder.Services.AddScoped<IDepartmentRepository, DepartmentCashRepo>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeCacheRepo>();

var app = builder.Build();

// Initialize database with migrations on startup
var logger = app.Services.GetRequiredService<ILogger<Program>>();
using (var scope = app.Services.CreateScope())
{
    try
    {
        logger.LogInformation("🚀 Starting database initialization...");
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        // Run migration with retry logic and proper error handling
        await DbInitializationService.InitializeDatabaseAsync(
            dbContext,
            logger,
            maxRetries: 5,
            delayMilliseconds: 2000
        );
    }
    catch (Exception ex)
    {
        logger.LogCritical($"❌ Application startup failed due to database initialization error: {ex.Message}");
        throw;
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseHttpsRedirection();

// Map endpoint groups
app.MapDepartmentEndpoints();
app.MapEmployeeEndpoints();

app.Run();
