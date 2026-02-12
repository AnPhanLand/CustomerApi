namespace CustomerApi.Application.Customers.Commands;

// ==========================================
// 1. THE COMMAND (The "What")
// ==========================================

// This record takes both the ID (from the URL) and the DTO (from the Body).
public record DeletePaymentCommand(Guid Id) : IRequest<Guid>;

// ==========================================
// 2. THE HANDLER (The "How")
// ==========================================

public class DeletePaymentHandler : IRequestHandler<DeletePaymentCommand, Guid>
{
    private readonly IApplicationDbContext _db;
    private readonly IDistributedCache _cache;

    // Injecting the database context. Remember to keep CustomerDb 'public'.
    public DeletePaymentHandler(IApplicationDbContext db, IDistributedCache cache)
    {
        _db = db;
        _cache = cache;
    }

    public async Task<Guid> Handle(DeletePaymentCommand request, CancellationToken ct)
    {
        if (await _db.Payments.FindAsync(new object[] { request.Id }, ct) is Payment payment)
        {
            _db.Payments.Remove(payment);
            await _db.SaveChangesAsync(ct);
            await _cache.RemoveAsync($"payment_{request.Id}", ct);
            await _cache.RemoveAsync("all_payments", ct);
            return payment.Id;
        }

        return Guid.Empty;
    }
}