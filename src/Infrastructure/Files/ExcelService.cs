namespace CustomerApi.Infrastructure.Files;
// Implementation using ClosedXML
public class ExcelService : IExcelService
{
    public byte[] ExportCustomers(IEnumerable<CustomerReadDTO> customers)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Customers");
        worksheet.Cell(1, 1).Value = "First Name";
        worksheet.Cell(1, 2).Value = "Last Name";
        // ... fill data ...
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public List<CustomerCreateDTO> ImportCustomers(Stream fileStream)
    {
        var list = new List<CustomerCreateDTO>();
        using var workbook = new XLWorkbook(fileStream);
        var worksheet = workbook.Worksheet(1);
        var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // Skip header row

        foreach (var row in rows)
        {
            list.Add(new CustomerCreateDTO {
                FirstName = row.Cell(1).Value.ToString(),
                LastName = row.Cell(2).Value.ToString(),
                // ... map other cells ...
            });
        }
        return list;
    }
}