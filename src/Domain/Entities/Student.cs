namespace CustomerApi.Domain.Entities;

public class Student : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{LastName} {FirstName}";
    
    public string ClassName { get; set; } = string.Empty;

    public Guid PhuHuynhId { get; set; } 
}