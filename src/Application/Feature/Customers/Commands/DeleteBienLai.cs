namespace CustomerApi.Application.Customers.Commands;

// ==========================================
// 1. THE COMMAND (The "What")
// ==========================================

// This record takes both the ID (from the URL) and the DTO (from the Body).
public record DeleteBienLaiCommand(int Id) : IRequest<int>;

// ==========================================
// 2. THE HANDLER (The "How")
// ==========================================

public class DeleteBienLaiHandler : IRequestHandler<DeleteBienLaiCommand, int>
{
    private readonly IApplicationDbContext _db;
    private readonly IDistributedCache _cache;

    // Injecting the database context. Remember to keep CustomerDb 'public'.
    public DeleteBienLaiHandler(IApplicationDbContext db, IDistributedCache cache)
    {
        _db = db;
        _cache = cache;
    }

    public async Task<int> Handle(DeleteBienLaiCommand request, CancellationToken ct)
    {
        if (await _db.BienLais.FindAsync(new object[] { request.Id }, ct) is BienLai bienLai)
        {
            _db.BienLais.Remove(bienLai);
            await _db.SaveChangesAsync(ct);
            await _cache.RemoveAsync($"bienlai_{request.Id}", ct);
            await _cache.RemoveAsync("all_bienlais", ct);
            return bienLai.Id;
        }

        return 0;
    }
}