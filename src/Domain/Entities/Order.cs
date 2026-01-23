namespace CustomerApi.Domain.Entities;

public class Order : BaseEntity
{
    public decimal TotalAmount { get; private set; }
    
    // Foreign Key: Links the order back to a specific customer
    public Guid CustomerId { get; private set; }

    // Navigation Property: Allows EF Core to load the Customer object
    public Customer Customer { get; private set; } = null!;

    private Order() { } // Required for EF Core

    public Order(Guid customerId, decimal totalAmount)
    {
        CustomerId = customerId;
        TotalAmount = totalAmount;
    }
}