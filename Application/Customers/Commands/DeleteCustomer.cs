using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace CustomerApp;

// ==========================================
// 1. THE COMMAND (The "What")
// ==========================================

// This record takes both the ID (from the URL) and the DTO (from the Body).
public record DeleteCustomerCommand(Guid Id) : IRequest<IResult>;

// ==========================================
// 2. THE HANDLER (The "How")
// ==========================================

public class DeleteCustomerHandler : IRequestHandler<DeleteCustomerCommand, IResult>
{
    private readonly CustomerDb _db;
    private readonly IDistributedCache _cache;

    // Injecting the database context. Remember to keep CustomerDb 'public'.
    public DeleteCustomerHandler(CustomerDb db, IDistributedCache cache)
    {
        _db = db;
        _cache = cache;
    }

    public async Task<IResult> Handle(DeleteCustomerCommand request, CancellationToken ct)
    {
        if (await _db.Customers.FindAsync(new object[] { request.Id }, ct) is Customer customer)
        {
            _db.Customers.Remove(customer);
            await _db.SaveChangesAsync(ct);
            await _cache.RemoveAsync($"customer_{request.Id}", ct);
            await _cache.RemoveAsync("all_customers", ct);
            return TypedResults.NoContent();
        }

        return TypedResults.NotFound();
    }
}