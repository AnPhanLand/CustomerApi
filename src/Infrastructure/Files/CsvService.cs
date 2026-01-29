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

    public IEnumerable<T> ImportFromCsv<T>(Stream stream)
    {
        using var reader = new StreamReader(stream);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        
        // GetRecords is "lazy"—it reads the file as you iterate
        return csv.GetRecords<T>().ToList(); 
    }
}