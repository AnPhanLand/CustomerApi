using Carter;

namespace CustomerApi.API;

public static class DependencyInjection
{
    public static IServiceCollection AddApi(this IServiceCollection services)
    {
        // CORS Policy
        services.AddCors(options =>
        {
            options.AddPolicy("MyFrontendPolicy", policy =>
            {
                policy.WithOrigins("http://localhost:5067") // Only allow your React/Angular app
                    .AllowAnyMethod()                    // Allow GET, POST, PUT, DELETE
                    .AllowAnyHeader();                   // Allow JWT headers
            });
        });

        // Provides helpful error pages during development if a database-related error occurs.
        services.AddDatabaseDeveloperPageExceptionFilter();

        // Required for Minimal APIs to discover your endpoints and generate metadata for Swagger/OpenAPI.
        services.AddEndpointsApiExplorer();

        // Configures NSwag to generate the OpenAPI (Swagger) documentation for your API.
        services.AddOpenApiDocument(config =>
        {
            config.DocumentName = "CustomerAPI"; // The internal name of the document.
            config.Title = "CustomerAPI";        // The title displayed at the top of the Swagger UI.
            config.Version = "v1";               // The version of your API.

            // Adds the "Authorize" button to the Swagger UI so you can test protected routes.
            config.AddSecurity("JWT", Enumerable.Empty<string>(), new NSwag.OpenApiSecurityScheme
            {
                // Defines the security type as an API Key (JWT falls under this in Swagger).
                Type = NSwag.OpenApiSecuritySchemeType.ApiKey, 
                
                // Tells Swagger to look for the "Authorization" field in the HTTP Header.
                Name = "Authorization", 
                In = NSwag.OpenApiSecurityApiKeyLocation.Header, 
                
                // The instruction shown to the user inside the Swagger UI.
                Description = "Type into the textbox: Bearer {your JWT token}." 
            });
        });

        // 4. Registers the Authorization service (checks if a user has specific permissions/roles).
        services.AddAuthorization();

        services.AddCarter();

        return services;
    }
}