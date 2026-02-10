namespace CustomerApi.Domain.Entities;

public class Student : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{LastName} {FirstName}";
    
    public string ClassName { get; set; } = string.Empty;
    public StudentStatus Status { get; set; } = StudentStatus.Default;
    public string Address { get; set; } = "Default";
    public StudentGender Gender { get; set; } = StudentGender.Default;
    public DateTime DateOfBirth { get; set; }
    public string ClassID { get; set; } = "Default";
    public string Grade { get; set; } = "Default";

    public Guid PhuHuynhId { get; set; } 
}