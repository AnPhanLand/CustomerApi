namespace CustomerApi.Application.Common.Interfaces;

public interface ICsvService
{
    byte[] ExportToCsv<T>(IEnumerable<T> data);
    IEnumerable<T> ImportFromCsv<T>(Stream stream);
}