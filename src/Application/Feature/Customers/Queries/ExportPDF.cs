using DevExpress.XtraReports.UI;

namespace Application.Features.Customers.Queries;

// We return a byte[] so the API can turn it into a file
public record GetCustomerReportPdfQuery() : IRequest<byte[]>;

public class GetCustomerReportPdfHandler : IRequestHandler<GetCustomerReportPdfQuery, byte[]>
{
    public async Task<byte[]> Handle(GetCustomerReportPdfQuery request, CancellationToken cancellationToken)
    {
            var report = XtraReport.FromFile("C:\\Users\\phant\\Documents\\Intern\\BackEndIntership\\CustomerApi\\src\\API\\newReport.repx", true);

            // Create a data source manually using the file you exported from pgAdmin
            var jsonDataSource = new DevExpress.DataAccess.Json.JsonDataSource
            {
                JsonSource = new DevExpress.DataAccess.Json.UriJsonSource(
                    new Uri(Path.Combine(Directory.GetCurrentDirectory(), "data", "Customers.json"))
                )
            };

            // Bind it to the report
            report.DataSource = jsonDataSource;
            
            using var ms = new MemoryStream();
            await report.ExportToPdfAsync(ms);
            return ms.ToArray();
    }
}