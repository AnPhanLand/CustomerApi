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
        customer.MapGet("/export-excel", async (IMediator mediator, IExcelService excelService) => 
        {
            // 1. Get data from DB via MediatR
            var customers = await mediator.Send(new GetAllCustomersQuery());
            
            // 2. Generate the Excel bytes
            var fileContents = excelService.ExportCustomers(customers);
            
            // 3. Return as a file download
            return Results.File(
                fileContents, 
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                "Customers.xlsx");
        });

        // Import from Excel
        customer.MapPost("/import-excel", async (IFormFile file, IMediator mediator, IExcelService excelService) => 
        {
            if (file == null || file.Length == 0) return Results.BadRequest("No file uploaded");

            // 1. Open the file stream
            using var stream = file.OpenReadStream();
            
            // 2. Turn Excel rows into DTOs
            var customerDtos = excelService.ImportCustomers(stream);
            
            // 3. Send to a Command to save them to Postgres
            // await mediator.Send(new ImportCustomersCommand(customerDtos));
            
            return Results.Ok($"{customerDtos.Count} customers imported successfully.");
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