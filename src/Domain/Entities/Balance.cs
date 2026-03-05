namespace CustomerApi.Domain.Entities;

public class Balance
{
    public int Id { get; set; }
    public int report_id { get; set; }
    public int account_id { get; set; }
    public int report_year { get; set; }
    public decimal amount { get; set; }
}