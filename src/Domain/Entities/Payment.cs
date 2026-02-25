namespace CustomerApi.Domain.Entities;

public class Payment : BaseEntity
{
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal InterestRate { get; set; } 
    public decimal DiscountRate { get; set; } = 0;
    public decimal Tax => Price * InterestRate; 
    
    public decimal Total => Price + Tax - (Price * DiscountRate);
    
    public Guid StudentId { get; set; }
    public Guid PhieuThuId { get; set; }
}