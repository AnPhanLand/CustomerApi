using Microsoft.EntityFrameworkCore;
using CustomerApp;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);
// builder.Services.AddDbContext<CustomerDb>(opt => opt.UseInMemoryDatabase("Customer"));

var connectionString = "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=mysecretpassword";

builder.Services.AddDbContext<CustomerDb>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
    config.DocumentName = "CustomerAPI";
    config.Title = "CustomerAPI";
    config.Version = "v1";
    ////////////////////////////////////
    config.AddSecurity("JWT", Enumerable.Empty<string>(), new NSwag.OpenApiSecurityScheme
    {
        Type = NSwag.OpenApiSecuritySchemeType.ApiKey,
        Name = "Authorization",
        In = NSwag.OpenApiSecurityApiKeyLocation.Header,
        Description = "Type into the textbox: Bearer {your JWT token}."
    });
});

var secretKey = builder.Configuration["Jwt:Key"];
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "your-api",
            ValidAudience = "your-client",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

//Login Test
app.MapPost("/login", async (LoginRequest login, CustomerDb db) => 
{
    // 1. Check for the hardcoded "Backdoor" Test Account first
    bool isTestAccount = (login.Email == "admin@test.com" && login.Password == "password123");
    
    Customer? user = null;

    if (isTestAccount)
    {
        // Create a fake user object for the test account so the rest of the code works
        user = new Customer { Id = Guid.Empty, Email = "admin@test.com" };
    }
    else
    {
        // 2. Otherwise, look for a real user in the database
        user = await db.Customers.FirstOrDefaultAsync(u => u.Email == login.Email);
        
        // Verify real user existence and password
        if (user is null || user.Password != login.Password) 
        {
            return Results.Unauthorized();
        }
    }

    // 3. Create claims using the user (either the real one or the test one)
    var claims = new[]
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        new Claim(JwtRegisteredClaimNames.Email, user.Email),
        new Claim(ClaimTypes.Role, "Admin")
    };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: "your-api",
        audience: "your-client",
        claims: claims,
        expires: DateTime.Now.AddMinutes(30),
        signingCredentials: creds
    );

    // 4. Return the token string from the response body
    return Results.Ok(new { 
        token = new JwtSecurityTokenHandler().WriteToken(token),
        email = user.Email 
    });
});

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi(config =>
    {
        config.DocumentTitle = "CustomerAPI";
        config.Path = "/swagger";
        config.DocumentPath = "/swagger/{documentName}/swagger.json";
        config.DocExpansion = "list";
    });
}

RouteGroupBuilder customer = app.MapGroup("/Customer").RequireAuthorization();

customer.MapGet("/", GetAllCustomers);
// todoItems.MapGet("/complete", GetCompleteTodos);
customer.MapGet("/{id}", GetCustomer);
customer.MapPost("/", CreateCustomer);
customer.MapPut("/{id}", UpdateCustomer);
customer.MapDelete("/{id}", DeleteCustomer);

app.Run();

static async Task<IResult> GetAllCustomers(CustomerDb db)
{
    return TypedResults.Ok(await db.Customers.Select(x => new CustomerReadDTO(x)).ToArrayAsync());
}

// static async Task<IResult> GetCompleteTodos(TodoDb db) {
//     return TypedResults.Ok(await db.Todos.Where(t => t.IsComplete).Select(x => new TodoItemDTO(x)).ToListAsync());
// }

static async Task<IResult> GetCustomer(Guid id, CustomerDb db)
{
    return await db.Customers.FindAsync(id)
        is Customer customer
            ? TypedResults.Ok(new CustomerReadDTO(customer))
            : TypedResults.NotFound();
}

static async Task<IResult> CreateCustomer(CustomerCreateDTO customerDTO, CustomerDb db)
{
    var customer = new Customer
    {
        FirstName = customerDTO.FirstName,
        LastName = customerDTO.LastName,
        Email = customerDTO.Email,
        Password = customerDTO.Password
    };

    db.Customers.Add(customer);
    await db.SaveChangesAsync();

    customerDTO = new CustomerCreateDTO(customer);

    return TypedResults.Created($"/Customer/{customer.Id}", customerDTO);
}

static async Task<IResult> UpdateCustomer(Guid id, CustomerUpdateDTO customerDTO, CustomerDb db)
{
    var customer = await db.Customers.FindAsync(id);

    if (customer is null) return TypedResults.NotFound();

    customer.FirstName = customerDTO.FirstName;
    customer.LastName = customerDTO.LastName;
    customer.Email = customerDTO.Email;

    await db.SaveChangesAsync();

    return TypedResults.NoContent();
}

static async Task<IResult> DeleteCustomer(Guid id, CustomerDb db)
{
    if (await db.Customers.FindAsync(id) is Customer customer)
    {
        db.Customers.Remove(customer);
        await db.SaveChangesAsync();
        return TypedResults.NoContent();
    }

    return TypedResults.NotFound();
}