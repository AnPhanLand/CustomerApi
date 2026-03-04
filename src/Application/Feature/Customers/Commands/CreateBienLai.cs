public record CreateBienLaiCommand(BienLaiCreateDTO BienLaiDTO) : IRequest<int>;

public class CreateBienLaiHandler : IRequestHandler<CreateBienLaiCommand, int>
{
    private readonly IApplicationDbContext _context;
    public CreateBienLaiHandler(IApplicationDbContext context) => _context = context;

    public async Task<int> Handle(CreateBienLaiCommand request, CancellationToken ct)
    {
        var entity = new BienLai
        {
            StudentId = request.BienLaiDTO.StudentId,
            HoTen = request.BienLaiDTO.HoTen,
            TonAn = request.BienLaiDTO.TonAn,
            TienAn = request.BienLaiDTO.TienAn,
            ChamSocBanTru = request.BienLaiDTO.ChamSocBanTru,
            Dien = request.BienLaiDTO.Dien,
            NuocUongTK = request.BienLaiDTO.NuocUongTK,
            HocHe = request.BienLaiDTO.HocHe,
            TrangBiPVBanTru = request.BienLaiDTO.TrangBiPVBanTru,
            BaoHiemTT = request.BienLaiDTO.BaoHiemTT,
            NangKhieu = request.BienLaiDTO.NangKhieu,
            Mua = request.BienLaiDTO.Mua,
            Ve = request.BienLaiDTO.Ve,
            TiengAnh = request.BienLaiDTO.TiengAnh,
            GhiChu = request.BienLaiDTO.GhiChu
        };

        _context.BienLais.Add(entity);
        await _context.SaveChangesAsync(ct);
        
        return entity.Id;
    }
}