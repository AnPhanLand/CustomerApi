using System.Reflection;
using CustomerApi.Infrastructure.Persistence.Mongo;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CustomerApi.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // 1. Register MediatR
        // This scans the current assembly to find all Handlers (like UpdateCustomerHandler).
        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
        });

        // Register the MongoDB Activity Logger
        services.AddScoped<IActivityLogger, MongoActivityLogger>();

        // 2. Register FluentValidation
        // This scans the assembly for all classes that inherit from AbstractValidator.
        services.AddValidatorsFromAssemblyContaining<Program>();

        return services;
    }
}