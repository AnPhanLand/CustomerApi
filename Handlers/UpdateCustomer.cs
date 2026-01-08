using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace CustomerApp;

// ==========================================
// 1. THE COMMAND (The "What")
// ==========================================

// This record takes both the ID (from the URL) and the DTO (from the Body).
public record UpdateCustomerCommand(Guid Id, CustomerUpdateDTO CustomerDTO) : IRequest<IResult>;

// ==========================================
// 2. THE HANDLER (The "How")
// ==========================================

public class UpdateCustomerHandler : IRequestHandler<UpdateCustomerCommand, IResult>
{
    private readonly CustomerDb _db;
    
    private readonly IDistributedCache _cache;

    // Injecting the database context. Remember to keep CustomerDb 'public'.
    public UpdateCustomerHandler(CustomerDb db, IDistributedCache cache)
    {
        _db = db;
        _cache = cache;
    }

    public async Task<IResult> Handle(UpdateCustomerCommand request, CancellationToken ct)
    {
        // 1. Find the existing customer using the ID from the command.
        // We use the object array syntax to include the CancellationToken 'ct'.
        var customer = await _db.Customers.FindAsync(new object[] { request.Id }, ct);

        // 2. If the customer doesn't exist, stop and return 404.
        if (customer is null) 
        {
            return TypedResults.NotFound();
        }

        // 3. Update the properties of the tracked entity.
        customer.FirstName = request.CustomerDTO.FirstName;
        customer.LastName = request.CustomerDTO.LastName;
        customer.Email = request.CustomerDTO.Email;

        // 4. Save the changes. 
        // Since EF Core is "tracking" this customer, it knows exactly what changed.
        await _db.SaveChangesAsync(ct);

        await _cache.RemoveAsync($"customer_{request.Id}", ct);

        // 5. Return 204 No Content (Standard for successful updates).
        return TypedResults.NoContent();
    }
}