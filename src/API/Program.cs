using CustomerApi.Infrastructure.Services;

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

    builder.Services.AddScoped<IReportService, ReportGeneratorService>();

    var app = builder.Build();

    app.UseCors("MyFrontendPolicy");
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseHangfireDashboard("/hangfire");
    app.MapCarter();

    using (var scope = app.Services.CreateScope())
    {
        var reportService = scope.ServiceProvider.GetRequiredService<IReportService>();
        
        // Cast it to the concrete class to access the Update method 
        // (since it's not part of the base IReportService interface)
        if (reportService is ReportGeneratorService generator)
        {
            generator.UpdateReportSchemaWithSql();
        }
    }

    Console.WriteLine("Done! Check your .repx file in VS Code.");

//     using (var scope = app.Services.CreateScope())
// {
//     var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
//     var conn = config.GetConnectionString("ReportsConnection");
//     new ReportGeneratorService(conn).UpdateReportSchemaWithSql();
// }

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