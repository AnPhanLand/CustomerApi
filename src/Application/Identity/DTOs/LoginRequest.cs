namespace CustomerApi.Application.Identity.DTOs;
public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;


    public LoginRequest() { }
    public LoginRequest(Customer customer) =>
    (Email, Password) = (customer.Email, customer.Password);
}