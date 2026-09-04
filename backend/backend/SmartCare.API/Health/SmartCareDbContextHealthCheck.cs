using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SmartCare.Infrastructure.Persistence;

namespace SmartCare.API.Health;

public sealed class SmartCareDbContextHealthCheck : IHealthCheck
{
    private readonly SmartCareDbContext _dbContext;

    public SmartCareDbContextHealthCheck(SmartCareDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var canConnect = await _dbContext.Database.CanConnectAsync(cancellationToken);

            return canConnect
                ? HealthCheckResult.Healthy("Database connection is available.")
                : HealthCheckResult.Unhealthy("Database connection could not be established.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(
                "An error occurred while checking database connectivity.",
                ex);
        }
    }
}
