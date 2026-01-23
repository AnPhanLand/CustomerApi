namespace CustomerApi.Application.Customers.Commands;

// ==========================================
// 1. THE COMMAND (The "What")
// ==========================================

// This record takes both the ID (from the URL) and the DTO (from the Body).
public record DeleteCustomerCommand(Guid Id) : IRequest<Guid>;

// ==========================================
// 2. THE HANDLER (The "How")
// ==========================================

public class DeleteCustomerHandler : IRequestHandler<DeleteCustomerCommand, Guid>
{
    private readonly IApplicationDbContext _db;
    private readonly IDistributedCache _cache;

    // Injecting the database context. Remember to keep CustomerDb 'public'.
    public DeleteCustomerHandler(IApplicationDbContext db, IDistributedCache cache)
    {
        _db = db;
        _cache = cache;
    }

    public async Task<Guid> Handle(DeleteCustomerCommand request, CancellationToken ct)
    {
        if (await _db.Customers.FindAsync(new object[] { request.Id }, ct) is Customer customer)
        {
            _db.Customers.Remove(customer);
            await _db.SaveChangesAsync(ct);
            await _cache.RemoveAsync($"customer_{request.Id}", ct);
            await _cache.RemoveAsync("all_customers", ct);
            return customer.Id;
        }

        return Guid.Empty;
    }
}