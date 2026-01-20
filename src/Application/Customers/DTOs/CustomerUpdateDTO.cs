namespace CustomerApi.Application.Customers.DTOs;
public class CustomerUpdateDTO
{
    // public Guid Id { get; set; } = Guid.NewGuid();
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public CustomerStatus Status { get; set; } = CustomerStatus.Pending;
    public MembershipLevel MembershipLevel { get; set; } = MembershipLevel.Standard;

    public CustomerUpdateDTO() { }
    public CustomerUpdateDTO(Customer customer) =>
    (FirstName, LastName, Email, Status, MembershipLevel) = (customer.FirstName, customer.LastName, customer.Email, customer.Status, customer.MembershipLevel);
}