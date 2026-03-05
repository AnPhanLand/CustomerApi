namespace CustomerApi.Application.Customers.DTOs;

public class AccountCreateDTO
{
    public string account_name { get; set; } = string.Empty;
    public string account_number { get; set; } = string.Empty;
    public int parent_account_id { get; set; }
}