using MediatR;
using CustomerApi.Application.Common.Interfaces;
using CustomerApi.Application.Common.Models;

namespace CustomerApi.Application.Customers.Queries;

public record ExportCSVQuery() : IRequest<FileResponse>;

public class ExportCSVQueryHandler : IRequestHandler<ExportCSVQuery, FileResponse>
{
    private readonly IMediator _mediator;
    private readonly ICsvService _csvService;

    public ExportCSVQueryHandler(IMediator mediator, ICsvService csvService)
    {
        _mediator = mediator;
        _csvService = csvService;
    }

    public async Task<FileResponse> Handle(ExportCSVQuery request, CancellationToken cancellationToken)
    {
            // 1. Fetch data from the database
            var customers = await _mediator.Send(new GetAllCustomersQuery());
            
            // 2. Generate the CSV bytes using the generic service
            var fileContents = _csvService.ExportToCsv(customers);
            
            // 3. Return the file with the "text/csv" content type
            return new FileResponse(
                fileContents, 
                "text/csv", 
                "Customers.csv");
    }
}