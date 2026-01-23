
using CustomerApi.Infrastructure;
using CustomerApi.Application;
using CustomerApi.Application.Common.Interfaces;
using CustomerApi.Infrastructure.Persistence.Mongo;
using CustomerApi.Infrastructure.Persistence.Postgres;
using CustomerApi.Domain.Entities;
using CustomerApi.Domain.ValueObjects;
using CustomerApi.Application.Customers.DTOs;
using CustomerApi.Application.Customers.Commands;

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
    builder.Host.UseSerilog();

    var app = builder.Build();

    app.UseCors("MyFrontendPolicy");
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseHangfireDashboard("/hangfire");

    var secretKey = builder.Configuration["Jwt:Key"];
    app.MapPost("/login", async (LoginRequest login, CustomerDb db) => 
    {
        // 1. DEFINE THE POLICY
        // We create a "Pipeline" that retries 3 times if a database exception occurs.
        var pipeline = new ResiliencePipelineBuilder<Customer?>()
            .AddRetry(new RetryStrategyOptions<Customer?>
            {
                ShouldHandle = new PredicateBuilder<Customer?>()
                    .Handle<DbUpdateException>()
                    .Handle<OperationCanceledException>(), // Handle timeouts
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromSeconds(1),
                BackoffType = DelayBackoffType.Constant // Wait exactly 1s between tries
            })
            .Build();

        // 1. Logic for a 'backdoor' account to allow testing without database entries.
        bool isTestAccount = (login.Email.Value == "admin@test.com" && login.Password == "password123");
        
        // Declares a variable to hold a Customer object, initializing it as 'null' (empty).
        // The '?' means this variable is "nullable"—it is allowed to be empty if no user is found.
        Customer? user = null;

        if (isTestAccount)
        {
            // Creates a temporary in-memory user object for the test account.
            user?.UpdateEmail("admin@test.com");
        }
        else
        {
            // 2. Database lookup: Finds a real customer matching the provided email.
            // 2. EXECUTE THE LOOKUP INSIDE THE PIPELINE
            // The code inside 'ExecuteAsync' is what Polly will watch over.
            user = await pipeline.ExecuteAsync(async ct => 
            {
                return await db.Customers.FirstOrDefaultAsync(u => u.Email.Value == login.Email.Value, ct);
            });
            
            // Security check: If user doesn't exist or password doesn't match, block access.
            if (user is null || user.Password != login.Password) 
            {
                return Results.Unauthorized(); // Returns HTTP 401.
            }
        }

        // 3. Claims: Key-value pairs describing the user that will be encoded into the JWT.
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()), // Unique ID of the user.
            new Claim(JwtRegisteredClaimNames.Email, user.Email.Value),       // User's email address.
            new Claim(ClaimTypes.Role, "Admin")                         // User's permission level.
        };

        // Prepares the mathematical key and algorithm used to sign the token.
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Creates the JWT object with metadata, expiration, and the user's claims.
        var token = new JwtSecurityToken(
            issuer: "your-api",
            audience: "your-client",
            claims: claims,
            expires: DateTime.Now.AddMinutes(30), // Token becomes invalid after 30 mins.
            signingCredentials: creds
        );

        // 4. Returns a JSON object containing the finalized string token to the client.
        return Results.Ok(new { 
            token = new JwtSecurityTokenHandler().WriteToken(token),
            email = user.Email 
        });
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
    Log.Information( "The application failed to start correctly.");
}
finally {
    // 4. Important: Ensures all log messages are written before the app closes
    Log.CloseAndFlush();
}