// These 'using' statements allow us to use the MediatR tools 
// and the Entity Framework methods (like ToArrayAsync).
using MediatR;
using Microsoft.EntityFrameworkCore;

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

    // CONSTRUCTOR: We ask .NET to "inject" our Database (CustomerDb) here.
    // Note: CustomerDb must be 'public' for this to work without errors.
    public GetAllCustomersHandler(CustomerDb db)
    {
        _db = db;
    }

    // THE HANDLE METHOD: This is the actual engine room of the feature.
    // This code runs only when you call 'mediator.Send(new GetAllCustomersQuery())' in Program.cs.
    public async Task<IResult> Handle(GetAllCustomersQuery request, CancellationToken ct)
    {
        // 1. Go to the 'Customers' table.
        // 2. 'Select' transforms the raw database model into a 'CustomerReadDTO' for security/cleanliness.
        // 3. 'ToArrayAsync(ct)' executes the query and gets the data. 
        //    The 'ct' (CancellationToken) stops the database work if the user closes their browser.
        var customers = await _db.Customers
            .Select(x => new CustomerReadDTO(x))
            .ToArrayAsync(ct);

        // Return a '200 OK' status code along with the list of customers.
        return TypedResults.Ok(customers);
    }
}