namespace CustomerApi.Infrastructure.Persistence.Mongo;

public class MongoActivityLogger : IActivityLogger
{
    private readonly IMongoCollection<CustomerActivity> _collection;

    public MongoActivityLogger(IConfiguration config)
    {
        var client = new MongoClient(config["MongoDbSettings:ConnectionString"]);
        var database = client.GetDatabase(config["MongoDbSettings:DatabaseName"]);
        _collection = database.GetCollection<CustomerActivity>("Activities");
    }

    public async Task LogActivityAsync(Guid customerId, string action)
    {
        var activity = new CustomerActivity 
        { 
            CustomerId = customerId, 
            Action = action, 
            Timestamp = DateTime.UtcNow 
        };
        await _collection.InsertOneAsync(activity);
    }
}