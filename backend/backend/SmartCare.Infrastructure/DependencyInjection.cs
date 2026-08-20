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
        // Add DbContext
        services.AddDbContext<SmartCareDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("SmartCare")));

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
        
        // Apply any pending migrations on startup
        await context.Database.MigrateAsync();
        
        // Seed data only if database is empty
        if (!await context.Patients.AnyAsync())
        {
            await DataSeed.SeedAsync(context);
        }
    }
}
