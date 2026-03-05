namespace CustomerApi.Application.Customers.Commands;

// ==========================================
// 1. THE COMMAND (The "What")
// ==========================================

// This record takes both the ID (from the URL) and the DTO (from the Body).
public record DeleteReportCommand(int Id) : IRequest<int>;

// ==========================================
// 2. THE HANDLER (The "How")
// ==========================================

public class DeleteReportHandler : IRequestHandler<DeleteReportCommand, int>
{
    private readonly IApplicationDbContext _db;
    private readonly IDistributedCache _cache;

    // Injecting the database context. Remember to keep CustomerDb 'public'.
    public DeleteReportHandler(IApplicationDbContext db, IDistributedCache cache)
    {
        _db = db;
        _cache = cache;
    }

    public async Task<int> Handle(DeleteReportCommand request, CancellationToken ct)
    {
        if (await _db.Reports.FindAsync(new object[] { request.Id }, ct) is Report report)
        {
            _db.Reports.Remove(report);
            await _db.SaveChangesAsync(ct);
            await _cache.RemoveAsync($"report_{request.Id}", ct);
            await _cache.RemoveAsync("all_reports", ct);
            return report.report_id;
        }

        return 0;
    }
}