namespace CustomerApi.Domain.Entities;

public class Payment : BaseEntity
{
    public string Description { get; set; } = string.Empty;
    public DateTime PaymentDeadline { get; set; }
    public decimal Price { get; set; }
    public decimal InterestRate { get; set; } 
    public decimal Tax => Price * InterestRate; 
    
    public decimal Total => Price + Tax;
    
    public Guid StudentId { get; set; }
}