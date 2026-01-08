namespace CustomerApp;
public class CustomerCreateDTO
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;


    public CustomerCreateDTO() { }
    public CustomerCreateDTO(Customer customer) =>
    (FirstName, LastName, Email) = (customer.FirstName, customer.LastName, customer.Email);
}