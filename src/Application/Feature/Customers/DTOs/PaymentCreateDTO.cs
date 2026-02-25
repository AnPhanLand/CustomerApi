namespace CustomerApi.Application.Customers.DTOs;
public class PaymentCreateDTO
{
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal InterestRate { get; set; } 
    public decimal DiscountRate { get; set; } = 0;
    public Guid StudentId { get; set; }
    public Guid PhieuThuId { get; set; }
}