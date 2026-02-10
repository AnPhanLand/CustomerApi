public record CreatePhuHuynhCommand(PhuHuynhCreateDTO PhuHuynhDTO) : IRequest<Guid>;

public class CreatePhuHuynhHandler : IRequestHandler<CreatePhuHuynhCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    public CreatePhuHuynhHandler(IApplicationDbContext context) => _context = context;

    public async Task<Guid> Handle(CreatePhuHuynhCommand request, CancellationToken ct)
    {
        var entity = new PhuHuynh
        {
            FirstName = request.PhuHuynhDTO.FirstName,
            LastName = request.PhuHuynhDTO.LastName,
            PhoneNumber = request.PhuHuynhDTO.PhoneNumber
        };

        _context.PhuHuynhs.Add(entity);
        await _context.SaveChangesAsync(ct);
        
        return entity.Id;
    }
}