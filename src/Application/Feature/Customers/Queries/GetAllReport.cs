using Microsoft.EntityFrameworkCore;

public record GetAllReportQuery() : IRequest<List<Report>>;

public class GetAllReportHandler : IRequestHandler<GetAllReportQuery, List<Report>>
        {
    private readonly IApplicationDbContext _context;
    public GetAllReportHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<Report>> Handle(GetAllReportQuery request, CancellationToken ct)
    {
        return await _context.Reports.ToListAsync(ct);
    }
}