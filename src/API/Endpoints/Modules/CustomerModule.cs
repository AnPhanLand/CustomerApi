using Application.Features.Customers.Queries;
using CustomerApi.Application.Common.Interfaces;
using DevExpress.XtraReports.UI;

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
        customer.MapGet("/export-csv", async (IMediator mediator) => 
        {
            var result = await mediator.Send(new ExportCSVQuery());

            return Results.File(result.Content, result.ContentType, result.FileName);
        });

        // Import from CSV
        customer.MapPost("/import-csv", async (IFormFile file, IMediator mediator, ICsvService csvService) => 
        {
            if (file == null || file.Length == 0) 
                return Results.BadRequest("No file uploaded");

            // Open the stream and send the command
            using var stream = file.OpenReadStream();
            var count = await mediator.Send(new ImportCSVCommand(stream));
            
            return Results.Ok($"{count} customers imported from CSV successfully.");
        });

        customer.MapGet("/export-pdf", async (IMediator mediator) =>
        {
            // Send the query to the Application Layer
            var pdfBytes = await mediator.Send(new GetCustomerReportPdfQuery());

            // Return the file result
            return Results.File(pdfBytes, "application/pdf", "Customers.pdf");
        });

        RouteGroupBuilder students = app.MapGroup("/Student").RequireRateLimiting("fixed"); 

        // List all
        students.MapGet("/", async (IMediator mediator) => 
        {
            // We send the "Query" object, and MediatR handles the rest
            return await mediator.Send(new GetAllStudentsQuery());
        });

        // Create new
        students.MapPost("/", async (StudentCreateDTO StudentDTO, IMediator mediator) => 
        {
            return await mediator.Send(new CreateStudentCommand(StudentDTO));
        });

        // Remove
        students.MapDelete("/{id}", async (Guid id, IMediator mediator) => 
        {
            return await mediator.Send(new DeleteStudentCommand(id));
        });


        RouteGroupBuilder phuHuynhs = app.MapGroup("/PhuHuynh").RequireRateLimiting("fixed"); 

        // List all
        phuHuynhs.MapGet("/", async (IMediator mediator) => 
        {
            // We send the "Query" object, and MediatR handles the rest
            return await mediator.Send(new GetAllPhuHuynhQuery());
        });

        // Create new
        phuHuynhs.MapPost("/", async (PhuHuynhCreateDTO PhuHuynhDTO, IMediator mediator) => 
        {
            return await mediator.Send(new CreatePhuHuynhCommand(PhuHuynhDTO));
        });

        RouteGroupBuilder payments = app.MapGroup("/Payment").RequireRateLimiting("fixed"); 

        // List all
        payments.MapGet("/", async (IMediator mediator) => 
        {
            // We send the "Query" object, and MediatR handles the rest
            return await mediator.Send(new GetAllPaymentQuery());
        });

        // Create new
        payments.MapPost("/", async (PaymentCreateDTO PaymentDTO, IMediator mediator) => 
        {
            return await mediator.Send(new CreatePaymentCommand(PaymentDTO));
        });
    }
}