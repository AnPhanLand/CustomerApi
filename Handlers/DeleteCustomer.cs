using MediatR;
using Microsoft.EntityFrameworkCore;

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

    // Injecting the database context. Remember to keep CustomerDb 'public'.
    public DeleteCustomerHandler(CustomerDb db)
    {
        _db = db;
    }

    public async Task<IResult> Handle(DeleteCustomerCommand request, CancellationToken ct)
    {
        if (await _db.Customers.FindAsync(new object[] { request.Id }, ct) is Customer customer)
        {
            _db.Customers.Remove(customer);
            await _db.SaveChangesAsync(ct);
            return TypedResults.NoContent();
        }

        return TypedResults.NotFound();
    }
}