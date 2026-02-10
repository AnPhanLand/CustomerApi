using MediatR;
using CustomerApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public record GetAllStudentsQuery() : IRequest<List<Student>>;

public class GetAllStudentsHandler : IRequestHandler<GetAllStudentsQuery, List<Student>>
{
    private readonly IApplicationDbContext _context;
    public GetAllStudentsHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<Student>> Handle(GetAllStudentsQuery request, CancellationToken ct)
    {
        return await _context.Students.ToListAsync(ct);
    }
}