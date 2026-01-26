namespace CustomerApi.Infrastructure.Files;

public class CsvService : ICsvService
{
    public byte[] ExportToCsv<T>(IEnumerable<T> data)
    {
        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        
        csv.WriteRecords(data);
        writer.Flush();
        return stream.ToArray();
    }
}