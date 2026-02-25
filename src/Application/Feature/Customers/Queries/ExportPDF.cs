using DevExpress.XtraReports.UI;

namespace Application.Features.Customers.Queries;

// We return a byte[] so the API can turn it into a file
public record GetCustomerReportPdfQuery() : IRequest<byte[]>;

public class GetCustomerReportPdfHandler : IRequestHandler<GetCustomerReportPdfQuery, byte[]>
{
    private readonly IReportService _reportService;

    public GetCustomerReportPdfHandler(IReportService reportService)
    {
        _reportService = reportService;
    }

    public async Task<byte[]> Handle(GetCustomerReportPdfQuery request, CancellationToken cancellationToken)
    {
        // No more JSON logic here!
        return await _reportService.GenerateReceiptPdfAsync(cancellationToken);
    }
}