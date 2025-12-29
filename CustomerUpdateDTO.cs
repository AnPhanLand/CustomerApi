namespace CustomerApp;
public class CustomerUpdateDTO
{
    // public Guid Id { get; set; } = Guid.NewGuid();
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public CustomerUpdateDTO() { }
    public CustomerUpdateDTO(Customer customer) =>
    (FirstName, LastName, Email) = (customer.FirstName, customer.LastName, customer.Email);
}