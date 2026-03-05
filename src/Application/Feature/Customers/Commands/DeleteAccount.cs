namespace CustomerApi.Application.Customers.Commands;

// ==========================================
// 1. THE COMMAND (The "What")
// ==========================================

// This record takes both the ID (from the URL) and the DTO (from the Body).
public record DeleteAccountCommand(int Id) : IRequest<int>;

// ==========================================
// 2. THE HANDLER (The "How")
// ==========================================

public class DeleteAccountHandler : IRequestHandler<DeleteAccountCommand, int>
{
    private readonly IApplicationDbContext _db;
    private readonly IDistributedCache _cache;

    // Injecting the database context. Remember to keep CustomerDb 'public'.
    public DeleteAccountHandler(IApplicationDbContext db, IDistributedCache cache)
    {
        _db = db;
        _cache = cache;
    }

    public async Task<int> Handle(DeleteAccountCommand request, CancellationToken ct)
    {
        if (await _db.Accounts.FindAsync(new object[] { request.Id }, ct) is Account account)
        {
            _db.Accounts.Remove(account);
            await _db.SaveChangesAsync(ct);
            await _cache.RemoveAsync($"account_{request.Id}", ct);
            await _cache.RemoveAsync("all_accounts", ct);
            return account.Id;
        }

        return 0;
    }
}