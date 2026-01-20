namespace CustomerApi.Application.Customers.DTOs;
public class CustomerReadDTO
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public CustomerReadDTO() { }
    public CustomerReadDTO(Customer customer) =>
    (Id, FullName, Email) = (customer.Id, customer.FullName, customer.Email);
}