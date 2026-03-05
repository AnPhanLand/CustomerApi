using Microsoft.EntityFrameworkCore;

public record GetAllBalanceQuery() : IRequest<List<Balance>>;

public class GetAllBalanceHandler : IRequestHandler<GetAllBalanceQuery, List<Balance>>
        {
    private readonly IApplicationDbContext _context;
    public GetAllBalanceHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<Balance>> Handle(GetAllBalanceQuery request, CancellationToken ct)
    {
        return await _context.Balances.ToListAsync(ct);
    }
}