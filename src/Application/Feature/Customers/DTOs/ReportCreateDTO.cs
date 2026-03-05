namespace CustomerApi.Application.Customers.DTOs;

public class ReportCreateDTO
{
    public string report_name { get; set; } = string.Empty;
    public string currency { get; set; } = string.Empty;
    public string unit { get; set; } = string.Empty;
}