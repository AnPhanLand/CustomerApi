namespace CustomerApi.Domain.Entities;

public class Report
{
    public int report_id { get; set; }
    public string report_name { get; set; } = string.Empty;
    public string currency { get; set; } = string.Empty;
    public string unit { get; set; } = string.Empty;
}