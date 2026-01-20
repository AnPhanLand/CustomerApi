namespace CustomerApi.Application.Identity.DTOs;
public class LoginRequest
{
    public EmailAddress Email { get; private set; } 
    public string Password { get; set; } = string.Empty;


    public LoginRequest() { }
    public LoginRequest(Customer customer) =>
    (Email, Password) = (customer.Email, customer.Password);
}