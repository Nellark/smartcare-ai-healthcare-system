using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartCare.Domain.Repositories;
using SmartCare.Domain.Services;
using SmartCare.Infrastructure.Persistence;
using SmartCare.Infrastructure.Persistence.Entities;
using SmartCare.Infrastructure.Persistence.Repositories;
using SmartCare.Infrastructure.Services;

namespace SmartCare.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("SmartCare");

        // Add DbContext (supports PostgreSQL and SQLite)
        services.AddDbContext<SmartCareDbContext>(options =>
        {
            if (!string.IsNullOrWhiteSpace(connectionString) && 
                (connectionString.Contains("Host=", StringComparison.OrdinalIgnoreCase) || 
                 connectionString.Contains("Server=", StringComparison.OrdinalIgnoreCase) ||
                 connectionString.Contains("Port=", StringComparison.OrdinalIgnoreCase)))
            {
                options.UseNpgsql(connectionString);
            }
            else
            {
                options.UseSqlite(connectionString);
            }
        });

        // Add repositories
        services.AddScoped<IPatientRepository, PatientRepository>();
        services.AddScoped<IDoctorRepository, DoctorRepository>();
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        services.AddScoped<IMedicalRecordRepository, MedicalRecordRepository>();
        services.AddScoped(typeof(IRepository<,>), typeof(GenericRepository<,>));

        // Add domain services
        services.AddScoped<IPatientDomainService, Services.PatientDomainService>();

        return services;
    }

    public static async Task InitializeDatabaseAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<SmartCareDbContext>();
        
        try
        {
            if (context.Database.IsNpgsql())
            {
                // Ensure tables exist in PostgreSQL
                await context.Database.EnsureCreatedAsync();
            }
            else
            {
                // Apply migrations for SQLite
                await context.Database.MigrateAsync();
            }
        }
        catch
        {
            // Table already exists or managed externally
        }
        
        // Seed data only if database is empty
        if (!await context.Patients.AnyAsync())
        {
            await DataSeed.SeedAsync(context);
        }
    }
}
