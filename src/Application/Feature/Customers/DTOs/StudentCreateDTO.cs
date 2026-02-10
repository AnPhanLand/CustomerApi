namespace CustomerApi.Application.Customers.DTOs;
public class StudentCreateDTO
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public StudentStatus Status { get; set; } = StudentStatus.DangHoc;
    public string Address { get; set; } = "Default";
    public StudentGender Gender { get; set; } = StudentGender.Default;
    public DateTime DateOfBirth { get; set; }
    public string ClassID { get; set; } = "Default";
    public string Grade { get; set; } = "Default";
    public Guid PhuHuynhId { get; set; }
}