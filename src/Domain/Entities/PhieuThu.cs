namespace CustomerApi.Domain.Entities;

public class PhieuThu : BaseEntity
{
    public string MaPhieu { get; set; } = string.Empty; 
    public DateTime HanThanhToan { get; set; } 
    public bool DaThanhToan { get; set; } 

    public Guid StudentId { get; set; } 
    public virtual Student Student { get; set; } = null!;
    public Guid PhuHuynhId { get; set; } 
    public virtual PhuHuynh PhuHuynh { get; set; } = null!;
    public Guid PaymentId { get; set; } 
    public virtual Payment Payment { get; set; } = null!;
}   