namespace CustomerApi.Domain.Entities;

public class PhuHuynh : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{LastName} {FirstName}";
    public string PhoneNumber { get; set; } = string.Empty;
}