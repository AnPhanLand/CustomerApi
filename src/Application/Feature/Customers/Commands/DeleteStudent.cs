namespace CustomerApi.Application.Customers.Commands;

// ==========================================
// 1. THE COMMAND (The "What")
// ==========================================

// This record takes both the ID (from the URL) and the DTO (from the Body).
public record DeleteStudentCommand(Guid Id) : IRequest<Guid>;

// ==========================================
// 2. THE HANDLER (The "How")
// ==========================================

public class DeleteStudentHandler : IRequestHandler<DeleteStudentCommand, Guid>
{
    private readonly IApplicationDbContext _db;
    private readonly IDistributedCache _cache;

    // Injecting the database context. Remember to keep CustomerDb 'public'.
    public DeleteStudentHandler(IApplicationDbContext db, IDistributedCache cache)
    {
        _db = db;
        _cache = cache;
    }

    public async Task<Guid> Handle(DeleteStudentCommand request, CancellationToken ct)
    {
        if (await _db.Students.FindAsync(new object[] { request.Id }, ct) is Student student)
        {
            _db.Students.Remove(student);
            await _db.SaveChangesAsync(ct);
            await _cache.RemoveAsync($"student_{request.Id}", ct);
            await _cache.RemoveAsync("all_students", ct);
            return student.Id;
        }

        return Guid.Empty;
    }
}