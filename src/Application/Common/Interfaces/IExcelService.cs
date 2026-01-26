namespace CustomerApi.Application.Common.Interfaces;
public interface IExcelService
{
    byte[] ExportCustomers(IEnumerable<CustomerReadDTO> customers);
    List<CustomerCreateDTO> ImportCustomers(Stream fileStream);
}