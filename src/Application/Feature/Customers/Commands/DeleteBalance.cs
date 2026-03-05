namespace CustomerApi.Application.Customers.Commands;

// ==========================================
// 1. THE COMMAND (The "What")
// ==========================================

// This record takes both the ID (from the URL) and the DTO (from the Body).
public record DeleteBalanceCommand(int Id) : IRequest<int>;

// ==========================================
// 2. THE HANDLER (The "How")
// ==========================================

public class DeleteBalanceHandler : IRequestHandler<DeleteBalanceCommand, int>
{
    private readonly IApplicationDbContext _db;
    private readonly IDistributedCache _cache;

    // Injecting the database context. Remember to keep CustomerDb 'public'.
    public DeleteBalanceHandler(IApplicationDbContext db, IDistributedCache cache)
    {
        _db = db;
        _cache = cache;
    }

    public async Task<int> Handle(DeleteBalanceCommand request, CancellationToken ct)
    {
        if (await _db.Balances.FindAsync(new object[] { request.Id }, ct) is Balance balance)
        {
            _db.Balances.Remove(balance);
            await _db.SaveChangesAsync(ct);
            await _cache.RemoveAsync($"balance_{request.Id}", ct);
            await _cache.RemoveAsync("all_balances", ct);
            return balance.Id;
        }

        return 0;
    }
}