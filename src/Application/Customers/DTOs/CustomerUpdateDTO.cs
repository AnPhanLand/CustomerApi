namespace CustomerApi.Application.Customers.DTOs;
public class CustomerUpdateDTO
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public CustomerStatus Status { get; set; }
    public MembershipLevel MembershipLevel { get; set; }
    public string CountryCode { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;

    public CustomerUpdateDTO() { }
    public CustomerUpdateDTO(Customer customer) =>
    (FirstName, LastName, Email, Status, MembershipLevel, CountryCode, PhoneNumber) = (customer.FirstName, customer.LastName, customer.Email.Value, customer.Status, customer.MembershipLevel, customer.ContactNumber.CountryCode, customer.ContactNumber.Number);
}