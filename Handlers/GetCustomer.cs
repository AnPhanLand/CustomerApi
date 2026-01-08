// MediatR allows the "Mediator" pattern to work (sending messages between classes).
using MediatR;
// EntityFrameworkCore provides the database methods like 'FindAsync'.
using Microsoft.EntityFrameworkCore;

namespace CustomerApp;

// ==========================================
// 1. THE REQUEST (The "What")
// ==========================================

// We use a 'record' to define the input needed for this specific task.
// It accepts a 'Guid Id' as a parameter.
// 'IRequest<IResult>' confirms that the person sending this request expects an HTTP Result back.
public record GetCustomerQuery(Guid Id) : IRequest<IResult>;


// ==========================================
// 2. THE HANDLER (The "How")
// ==========================================

// This class is the "Worker" that performs the database search.
// It is linked to the 'GetCustomerQuery' and returns an 'IResult'.
public class GetCustomerHandler : IRequestHandler<GetCustomerQuery, IResult>
{
    // A private field to hold our database context instance.
    private readonly CustomerDb _db;

    // CONSTRUCTOR: We inject the database here so the handler can use it.
    // Ensure 'CustomerDb' is marked as 'public' in your data folder.
    public GetCustomerHandler(CustomerDb db)
    {
        _db = db;
    }

    // THE HANDLE METHOD: The logic triggered by 'mediator.Send()'.
    public async Task<IResult> Handle(GetCustomerQuery request, CancellationToken ct)
    {
        // 'request.Id' comes from the Query record we defined above.
        // 'FindAsync' is the most efficient way to look up a primary key.
        // We pass 'ct' (CancellationToken) to stop the DB query if the user cancels the request.
        var customer = await _db.Customers.FindAsync(new object[] { request.Id }, ct);

        // TERNARY OPERATOR: 
        // If 'customer' is found (not null), return '200 OK' with the DTO data.
        // If 'customer' is null, return '404 Not Found'.
        return customer is not null 
            ? TypedResults.Ok(new CustomerReadDTO(customer)) 
            : TypedResults.NotFound();
    }
}