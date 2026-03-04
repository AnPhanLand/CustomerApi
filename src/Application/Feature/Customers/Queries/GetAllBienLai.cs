using Microsoft.EntityFrameworkCore;

public record GetAllBienLaiQuery() : IRequest<List<BienLai>>;

public class GetAllBienLaiHandler : IRequestHandler<GetAllBienLaiQuery, List<BienLai>>
{
    private readonly IApplicationDbContext _context;
    public GetAllBienLaiHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<BienLai>> Handle(GetAllBienLaiQuery request, CancellationToken ct)
    {
        return await _context.BienLais.ToListAsync(ct);
    }
}   