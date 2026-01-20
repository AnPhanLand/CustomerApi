namespace CustomerApi.Application.Customers.DTOs;
public class CustomerReadDTO
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public CustomerStatus Status { get; set; } 
    public MembershipLevel MembershipLevel { get; set; } 
    public CustomerReadDTO() { }
    public CustomerReadDTO(Customer customer) =>
    (Id, FullName, Email, CountryCode, PhoneNumber, Status, MembershipLevel) = (customer.Id, customer.FullName, customer.Email.Value, customer.ContactNumber.CountryCode, customer.ContactNumber.Number, customer.Status, customer.MembershipLevel);
}