using MediatR;
using CustomerApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public record GetAllPaymentQuery() : IRequest<List<Payment>>;

public class GetAllPaymentHandler : IRequestHandler<GetAllPaymentQuery, List<Payment>>
{
    private readonly IApplicationDbContext _context;
    public GetAllPaymentHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<Payment>> Handle(GetAllPaymentQuery request, CancellationToken ct)
    {
        return await _context.Payments.ToListAsync(ct);
    }
}   