
using CustomerApi.Infrastructure;
using CustomerApi.Application;
using CustomerApi.Application.Common.Interfaces;
using CustomerApi.Infrastructure.Persistence.Mongo;
using CustomerApi.Infrastructure.Persistence.Postgres;
using CustomerApi.Domain.Entities;
using CustomerApi.Domain.ValueObjects;
using CustomerApi.Application.Customers.DTOs;
using CustomerApi.Application.Customers.Commands;
using CustomerApi.API;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information)
    .WriteTo.File("logs/myapp.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
try {
    Log.Information("Starting the API...");

    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddApi();
    builder.Host.UseSerilog();

    var app = builder.Build();

    app.UseCors("MyFrontendPolicy");
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseHangfireDashboard("/hangfire");

    app.MapPost("/login", async (LoginRequest request, IMediator mediator) => 
    {
        try 
        {
            var result = await mediator.Send(new LoginCommand(request));
            return Results.Ok(result);
        }
        catch (UnauthorizedAccessException) 
        {
            return Results.Unauthorized();
        }
    });

    // Checks if the application is running in 'Development' mode (not production).
    if (app.Environment.IsDevelopment())
    {
        // Generates the OpenAPI specification (swagger.json).
        app.UseOpenApi();
        
        // Enables the visual Swagger UI website to test endpoints easily.
        app.UseSwaggerUi(config =>
        {
            config.DocumentTitle = "CustomerAPI";
            config.Path = "/swagger"; // The URL where you view the UI.
            config.DocumentPath = "/swagger/{documentName}/swagger.json";
            config.DocExpansion = "list"; // Keeps the endpoint list collapsed by default.
        });
    }


    RouteGroupBuilder customer = app.MapGroup("/Customer").RequireAuthorization().RequireRateLimiting("fixed");;

    // List all
    customer.MapGet("/", async (IMediator mediator) => 
    {
        // We send the "Query" object, and MediatR handles the rest
        return await mediator.Send(new GetAllCustomersQuery());
    });

    // Get one by ID
    customer.MapGet("/{id}", async (Guid id, IMediator mediator) => 
    {
        // Pass the id into the constructor of the Query
        return await mediator.Send(new GetCustomerQuery(id));
    });

    // Create new
    customer.MapPost("/", async (CustomerCreateDTO CustomerDTO, IMediator mediator) => 
    {
        return await mediator.Send(new CreateCustomerCommand(CustomerDTO));
    });

    // Update existing
    customer.MapPut("/{id}", async (Guid id, CustomerUpdateDTO CustomerDTO, IMediator mediator) => 
    {
        return await mediator.Send(new UpdateCustomerCommand(id, CustomerDTO));
    });

    // Remove
    customer.MapDelete("/{id}", async (Guid id, IMediator mediator) => 
    {
        return await mediator.Send(new DeleteCustomerCommand(id));
    });

    app.Run();


// Serilog exception catch
} catch (Exception ex) {
    // 3. This catches "Startup Crashes" (e.g., bad connection strings)
    Log.Fatal(ex, "The application failed to start correctly.");
}
finally {
    // 4. Important: Ensures all log messages are written before the app closes
    Log.CloseAndFlush();
}