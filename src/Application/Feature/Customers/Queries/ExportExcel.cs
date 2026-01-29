using MediatR;
using CustomerApi.Application.Common.Interfaces;
using CustomerApi.Application.Common.Models;

namespace CustomerApi.Application.Customers.Queries;

public record ExportCustomersQuery() : IRequest<FileResponse>;

public class ExportCustomersQueryHandler : IRequestHandler<ExportCustomersQuery, FileResponse>
{
    private readonly IMediator _mediator;
    private readonly IExcelService _excelService;

    public ExportCustomersQueryHandler(IMediator mediator, IExcelService excelService)
    {
        _mediator = mediator;
        _excelService = excelService;
    }

    public async Task<FileResponse> Handle(ExportCustomersQuery request, CancellationToken cancellationToken)
    {
            // 1. Get data from DB via MediatR
            var customers = await _mediator.Send(new GetAllCustomersQuery());
            
            // 2. Generate the Excel bytes
            var fileContents = _excelService.ExportCustomers(customers);
            
            // 3. Return as a file download
            return new FileResponse(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Customers.xlsx");
    }
}