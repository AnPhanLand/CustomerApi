namespace CustomerApi.Domain.Entities;

public class CustomerActivity
{
    public string? Id { get; set; }
    public Guid CustomerId { get; set; }
    public string Action { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    // Mongo is great for flexible data like this:
    public Dictionary<string, object>? Metadata { get; set; } 
}