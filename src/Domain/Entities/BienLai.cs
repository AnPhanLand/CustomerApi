namespace CustomerApi.Domain.Entities;

public class BienLai
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string HoTen { get; set; } = string.Empty;
    public int TonAn { get; set; }
    public int TienAn { get; set; } = 0;
    public int ChamSocBanTru { get; set; } = 0;
    public int Dien { get; set; } = 0;
    public int NuocUongTK { get; set; } = 0;
    public int HocHe { get; set; } = 0;
    public int TrangBiPVBanTru { get; set; } = 0;
    public int BaoHiemTT { get; set; } = 0;
    public int NangKhieu { get; set; } = 0;
    public int Mua { get; set; } = 0;
    public int Ve  { get; set; } = 0;
    public int TiengAnh { get; set; } = 0;
    public string GhiChu { get; set; } = string.Empty;
}