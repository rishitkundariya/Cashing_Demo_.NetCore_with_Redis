using Microsoft.EntityFrameworkCore;

namespace CachingDemo.Infrastructure.Data;

/// <summary>
/// Service for managing database initialization and migrations
/// Ensures the database is ready before the application starts
/// </summary>
public static class DbInitializationService
{
    /// <summary>
    /// Initializes the database with retries and proper error handling
    /// Waits for database availability and runs pending migrations
    /// </summary>
    public static async Task InitializeDatabaseAsync(
        ApplicationDbContext dbContext,
        ILogger<Program> logger,
        int maxRetries = 5,
        int delayMilliseconds = 2000)
    {
        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                logger.LogInformation($"Database Initialization - Attempt {i + 1}/{maxRetries}");

                // Check if database connection is available
                var canConnect = await dbContext.Database.CanConnectAsync();
                
                if (!canConnect)
                {
                    logger.LogWarning($"Cannot connect to database. Retrying in {delayMilliseconds}ms...");
                    await Task.Delay(delayMilliseconds);
                    continue;
                }

                logger.LogInformation("Database connection successful. Running migrations...");

                // Get pending migrations
                var pendingMigrations = (await dbContext.Database.GetPendingMigrationsAsync()).ToList();
                
                if (pendingMigrations.Count > 0)
                {
                    logger.LogInformation($"Found {pendingMigrations.Count} pending migration(s)");
                    foreach (var migration in pendingMigrations)
                    {
                        logger.LogInformation($"  - {migration}");
                    }
                    
                    // Apply migrations
                    await dbContext.Database.MigrateAsync();
                    logger.LogInformation("✅ All migrations applied successfully");
                }
                else
                {
                    logger.LogInformation("✅ Database is up to date. No migrations needed.");
                }

                return; // Success - exit method
            }
            catch (Exception ex)
            {
                logger.LogError($"Migration attempt {i + 1} failed: {ex.Message}");
                
                if (i == maxRetries - 1)
                {
                    // Last attempt failed
                    logger.LogCritical($"❌ Failed to initialize database after {maxRetries} attempts");
                    logger.LogCritical($"Error: {ex.Message}");
                    throw; // Re-throw the exception to fail the startup
                }
                
                // Wait before retry
                logger.LogInformation($"Retrying in {delayMilliseconds}ms...");
                await Task.Delay(delayMilliseconds);
            }
        }
    }

    /// <summary>
    /// Gets database health status
    /// </summary>
    public static async Task<bool> IsHealthyAsync(ApplicationDbContext dbContext)
    {
        try
        {
            return await dbContext.Database.CanConnectAsync();
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Gets applied migrations count
    /// </summary>
    public static async Task<int> GetAppliedMigrationsCountAsync(ApplicationDbContext dbContext)
    {
        try
        {
            var applied = await dbContext.Database.GetAppliedMigrationsAsync();
            return applied.Count();
        }
        catch
        {
            return 0;
        }
    }
}
