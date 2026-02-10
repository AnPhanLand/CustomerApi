using MediatR;
using CustomerApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public record GetAllPhieuThuQuery() : IRequest<List<PhieuThu>>;

public class GetAllPhieuThuHandler : IRequestHandler<GetAllPhieuThuQuery, List<PhieuThu>>
{
    private readonly IApplicationDbContext _context;
    public GetAllPhieuThuHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<PhieuThu>> Handle(GetAllPhieuThuQuery request, CancellationToken ct)
    {
        return await _context.PhieuThus.ToListAsync(ct);
    }
}   