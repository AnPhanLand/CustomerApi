using MediatR;
using CustomerApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public record GetAllPhuHuynhQuery() : IRequest<List<PhuHuynh>>;

public class GetAllPhuHuynhHandler : IRequestHandler<GetAllPhuHuynhQuery, List<PhuHuynh>>
{
    private readonly IApplicationDbContext _context;
    public GetAllPhuHuynhHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<PhuHuynh>> Handle(GetAllPhuHuynhQuery request, CancellationToken ct)
    {
        return await _context.PhuHuynhs.ToListAsync(ct);
    }
}