namespace CustomerApi.Infrastructure.Files;
// Implementation using ClosedXML
public class ExcelService : IExcelService
{
    public byte[] ExportCustomers(IEnumerable<CustomerReadDTO> customers)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Customers");

        // This one line replaces the manual headers and the loop!
        // It starts at row 1, column 1 and creates a formatted Excel table.
        worksheet.Cell(1, 1).InsertTable(customers);

        // Optional: Adjust column widths to fit the text automatically
        worksheet.Columns().AdjustToContents();

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