using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Polly;
using Polly.Retry;
using System.IdentityModel.Tokens.Jwt;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

public record LoginCommand(LoginRequest Request) : IRequest<LoginResponse>;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public LoginCommandHandler(IApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<LoginResponse> Handle(LoginCommand command, CancellationToken ct)
    {
        // 1. Resilience Pipeline (Moved from Program.cs)
        var pipeline = new ResiliencePipelineBuilder<Customer?>()
            .AddRetry(new RetryStrategyOptions<Customer?>
            {
                ShouldHandle = new PredicateBuilder<Customer?>()
                    .Handle<DbUpdateException>()
                    .Handle<OperationCanceledException>(),
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromSeconds(1),
                BackoffType = DelayBackoffType.Constant
            })
            .Build();

        // 2. Logic & Backdoor check
        Customer? user;
        if (command.Request.Email.Value == "admin@test.com" && command.Request.Password == "password123")
        {
            // Note: Don't use user?.UpdateEmail on a null object. Create a dummy instance or throw/return early.
            user = new Customer(command.Request.Email, PhoneNumber.Create("+1", "0000000")); 
        }
        else
        {
            user = await pipeline.ExecuteAsync(async token => 
                await _context.Customers.FirstOrDefaultAsync(u => u.Email.Value == command.Request.Email.Value, token), ct);

            if (user is null || user.Password != command.Request.Password)
            {
                throw new UnauthorizedAccessException("Invalid credentials."); // Application exception
            }
        }

        // 3. Generate Token (This could also be moved to an IIdentityService in Infrastructure)
        var token = GenerateJwtToken(user);

        return new LoginResponse(token, user.Email);
    }

    private string GenerateJwtToken(Customer user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email.Value),
            new Claim(ClaimTypes.Role, "Admin")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var jwt = new JwtSecurityToken(
            issuer: "your-api",
            audience: "your-client",
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}