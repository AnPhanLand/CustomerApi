namespace CustomerApi.Application.Customers.DTOs;
public class PhieuThuCreateDTO
{
    public DateTime HanThanhToan { get; set; } 
    public bool DaThanhToan { get; set; } 

    public Guid StudentId { get; set; } 
    public Guid PhuHuynhId { get; set; } 
}