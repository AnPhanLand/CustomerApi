public record CreatePhieuThuCommand(PhieuThuCreateDTO PhieuThuDTO) : IRequest<Guid>;

public class CreatePhieuThuHandler : IRequestHandler<CreatePhieuThuCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    public CreatePhieuThuHandler(IApplicationDbContext context) => _context = context;

    public async Task<Guid> Handle(CreatePhieuThuCommand request, CancellationToken ct)
    {
        var entity = new PhieuThu
        {
            HanThanhToan = request.PhieuThuDTO.HanThanhToan,
            DaThanhToan = request.PhieuThuDTO.DaThanhToan,
            StudentId = request.PhieuThuDTO.StudentId,
            PhuHuynhId = request.PhieuThuDTO.PhuHuynhId,
            PaymentId = request.PhieuThuDTO.PaymentId
        };

        _context.PhieuThus.Add(entity);
        await _context.SaveChangesAsync(ct);
        
        return entity.Id;
    }
}