namespace CustomerApi.Application.Customers.DTOs;
public class CustomerCreateDTO
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    // New fields needed for the PhoneNumber Value Object
    public string CountryCode { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;


    public CustomerCreateDTO() { }
    public CustomerCreateDTO(Customer customer) =>
    (FirstName, LastName, Email, Password, CountryCode, PhoneNumber) 
    = (customer.FirstName, customer.LastName, customer.Email.Value, customer.Password, customer.ContactNumber.CountryCode, customer.ContactNumber.Number);
}