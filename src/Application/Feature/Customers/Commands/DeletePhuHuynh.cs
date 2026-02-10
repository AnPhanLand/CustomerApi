namespace CustomerApi.Application.Customers.Commands;

// ==========================================
// 1. THE COMMAND (The "What")
// ==========================================

// This record takes both the ID (from the URL) and the DTO (from the Body).
public record DeletePhuHuynhCommand(Guid Id) : IRequest<Guid>;

// ==========================================
// 2. THE HANDLER (The "How")
// ==========================================

public class DeletePhuHuynhHandler : IRequestHandler<DeletePhuHuynhCommand, Guid>
{
    private readonly IApplicationDbContext _db;
    private readonly IDistributedCache _cache;

    // Injecting the database context. Remember to keep CustomerDb 'public'.
    public DeletePhuHuynhHandler(IApplicationDbContext db, IDistributedCache cache)
    {
        _db = db;
        _cache = cache;
    }

    public async Task<Guid> Handle(DeletePhuHuynhCommand request, CancellationToken ct)
    {
        if (await _db.PhuHuynhs.FindAsync(new object[] { request.Id }, ct) is PhuHuynh phuHuynh)
        {
            _db.PhuHuynhs.Remove(phuHuynh);
            await _db.SaveChangesAsync(ct);
            await _cache.RemoveAsync($"phuhuynh_{request.Id}", ct);
            await _cache.RemoveAsync("all_phuhuynhs", ct);
            return phuHuynh.Id;
        }

        return Guid.Empty;
    }
}