namespace CustomerApp;
public class Customer
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    // Using the Enum here
    public CustomerStatus Status { get; set; } = CustomerStatus.Pending;
}