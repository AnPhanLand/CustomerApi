namespace CustomerApp;
public class Customer : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    // Using the Enum here
    public CustomerStatus Status { get; set; } = CustomerStatus.Pending;
    public MembershipLevel MembershipLevel { get; set; } = MembershipLevel.Standard;
    public string Region { get; set; } = CustomerRules.DefaultRegion;
}