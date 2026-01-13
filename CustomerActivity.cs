using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace CustomerApp;

public class CustomerActivity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public Guid CustomerId { get; set; }
    public string Action { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    // Mongo is great for flexible data like this:
    public Dictionary<string, object>? Metadata { get; set; } 
}