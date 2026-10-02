using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SmartHealthcare.Application.Common.Behaviors;

namespace SmartHealthcare.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddAutoMapper(assembly);
        services.AddValidatorsFromAssembly(assembly);

        // Register MediatR and Pipeline Behaviors
        services.AddMediatR(cfg =>
        {
            // Register MediatR handlers from the Application assembly
            cfg.RegisterServicesFromAssembly(assembly);

            // Add validation behavior to the MediatR pipeline
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            // Add transaction behavior to automatically wrap all commands in atomic database transactions
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
        });

        return services;
    }
}
