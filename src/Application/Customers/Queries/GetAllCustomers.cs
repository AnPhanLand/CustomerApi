// These 'using' statements allow us to use the MediatR tools 
// and the Entity Framework methods (like ToArrayAsync).
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

// Namespaces organize your code. This should match your project name or folder.
namespace CustomerApp;

// ==========================================
// 1. THE REQUEST (The "What")
// ==========================================

// We use a 'record' here because it's a lightweight way to hold data.
// 'IRequest<IResult>' tells MediatR: "When this request is finished, 
// it will return a standard ASP.NET Result (like 200 OK or 404 Not Found)."
public record GetAllCustomersQuery() : IRequest<IResult>;


// ==========================================
// 2. THE HANDLER (The "How")
// ==========================================

// This class is the "Worker" that waits for the 'GetAllCustomersQuery' to be sent.
// It implements 'IRequestHandler', which links this specific class to that specific Request.
public class GetAllCustomersHandler : IRequestHandler<GetAllCustomersQuery, IResult>
{
    // We create a private variable to hold our Database connection.
    private readonly CustomerDb _db;
    private readonly IDistributedCache _cache;
    private const string CacheKey = "all_customers";

    // CONSTRUCTOR: We ask .NET to "inject" our Database (CustomerDb) here.
    // Note: CustomerDb must be 'public' for this to work without errors.
    public GetAllCustomersHandler(CustomerDb db, IDistributedCache cache)
    {
        _db = db;
        _cache = cache;
    }

    // THE HANDLE METHOD: This is the actual engine room of the feature.
    // This code runs only when you call 'mediator.Send(new GetAllCustomersQuery())' in Program.cs.
    public async Task<IResult> Handle(GetAllCustomersQuery request, CancellationToken ct)
    {
        // 1. Try to get the list from Redis
        var cachedData = await _cache.GetStringAsync(CacheKey, ct);

        if (!string.IsNullOrEmpty(cachedData))
        {
            // Deserialize the JSON string back into a List
            var customers = JsonSerializer.Deserialize<List<CustomerReadDTO>>(cachedData);
            return TypedResults.Ok(customers);
        }

        // 2. If not in cache, go to Postgres
        var customersFromDb = await _db.Customers
            .Select(c => new CustomerReadDTO(c))
            .ToListAsync(ct);

        // 3. Store the result in Redis for 5 minutes
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        };

        var serializedData = JsonSerializer.Serialize(customersFromDb);
        await _cache.SetStringAsync(CacheKey, serializedData, options, ct);

        return TypedResults.Ok(customersFromDb);
        
        // 1. Go to the 'Customers' table.
        // 2. 'Select' transforms the raw database model into a 'CustomerReadDTO' for security/cleanliness.
        // 3. 'ToArrayAsync(ct)' executes the query and gets the data. 
        //    The 'ct' (CancellationToken) stops the database work if the user closes their browser.
        // var customers = await _db.Customers
        //     .Select(x => new CustomerReadDTO(x))
        //     .ToArrayAsync(ct);

        // // Return a '200 OK' status code along with the list of customers.
        // return TypedResults.Ok(customers);
    }
}