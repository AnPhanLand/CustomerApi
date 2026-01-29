using CustomerApi.Application.Common.Interfaces;

namespace CustomerApi.API.Endpoints.Modules;

public class CustomerModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder customer = app.MapGroup("/Customer").RequireAuthorization().RequireRateLimiting("fixed");
        
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

        // Export to Excel
        customer.MapGet("/export-excel", async (IMediator mediator) => 
        {
            var result = await mediator.Send(new ExportCustomersQuery());

            return Results.File(result.Content, result.ContentType, result.FileName);
        });

        // Import from Excel
        customer.MapPost("/import-excel", async (IFormFile file, IMediator mediator, IExcelService excelService) => 
        {
            // Basic HTTP Validation
            if (file == null || file.Length == 0) 
                return Results.BadRequest("No file uploaded");

            // Open the stream and send it to the Application layer
            using var stream = file.OpenReadStream();
            var count = await mediator.Send(new ImportCustomersCommand(stream));
            
            return Results.Ok($"{count} customers imported successfully.");
        });

        // Export to CSV
        customer.MapGet("/export-csv", async (IMediator mediator, ICsvService csvService) => 
        {
            // 1. Fetch data from the database
            var customers = await mediator.Send(new GetAllCustomersQuery());
            
            // 2. Generate the CSV bytes using the generic service
            var fileContents = csvService.ExportToCsv(customers);
            
            // 3. Return the file with the "text/csv" content type
            return Results.File(
                fileContents, 
                "text/csv", 
                "Customers.csv");
        });

        // Import from CSV
        customer.MapPost("/import-csv", async (IFormFile file, IMediator mediator, ICsvService csvService) => 
        {
            if (file == null || file.Length == 0) return Results.BadRequest("No file uploaded");

            using var stream = file.OpenReadStream();
            using var reader = new StreamReader(stream);

            // Logic here would call a CSV parsing method in your service
            // var customerDtos = csvService.ImportFromCsv<CustomerCreateDTO>(reader);
            
            return Results.Ok("CSV data received for processing.");
        });
    }
}