using Microsoft.EntityFrameworkCore;

public record GetAllAccountQuery() : IRequest<List<Account>>;

public class GetAllAccountHandler : IRequestHandler<GetAllAccountQuery, List<Account>>
        {
    private readonly IApplicationDbContext _context;
    public GetAllAccountHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<Account>> Handle(GetAllAccountQuery request, CancellationToken ct)
    {
        return await _context.Accounts.ToListAsync(ct);
    }
}