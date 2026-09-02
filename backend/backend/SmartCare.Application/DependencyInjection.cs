using Microsoft.Extensions.DependencyInjection;
using MediatR;
using FluentValidation;
using SmartCare.Application.Common.Behaviors;

namespace SmartCare.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Add MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        // Add AutoMapper
        services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<SmartCare.Application.Common.Mappings.MappingProfile>();
        });

        // Add FluentValidation
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        // Add pipeline behaviors
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}
