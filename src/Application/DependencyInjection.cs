namespace CustomerApi.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // 1. Register MediatR
        // This scans the current assembly to find all Handlers (like UpdateCustomerHandler).
        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
        });

        // 2. Register FluentValidation
        // This scans the assembly for all classes that inherit from AbstractValidator.
        services.AddValidatorsFromAssemblyContaining(typeof(DependencyInjection));

        return services;
    }
}