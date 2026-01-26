public record LoginRequest(EmailAddress Email, string Password);
public record LoginResponse(string Token, EmailAddress Email);