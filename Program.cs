
// Outline:

// The Configuration Zone (builder): How we tell the app to use Npgsql (Postgres), NSwag (Swagger), and JWT Authentication.

// The Middleware Pipeline (app.Use...): How a request travels through your security checks before reaching your logic.

// The Identity Zone (/login): How the API issues the digital "passport" (the token) to a user.

// The Protected Data Zone (/Customer): How EF Core interacts with the database while checking if the user is authorized.

// The Data Transfer Zone (DTOs): How we safely move data between the database and the user without exposing sensitive fields like passwords.


using Microsoft.EntityFrameworkCore;
using CustomerApp;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

// This section of your Program.cs is the Configuration Zone. It tells the web server which "tools" (services) it needs to have ready before the application actually starts running.

// Initializes the web application builder, which manages configuration, logging, and services.
var builder = WebApplication.CreateBuilder(args);

// The connection string containing the address, credentials, and database name for your PostgreSQL instance.
var connectionString = "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=mysecretpassword";

// Registers your Database Context (CustomerDb) to use the Npgsql provider for PostgreSQL.
builder.Services.AddDbContext<CustomerDb>(options =>
    options.UseNpgsql(connectionString));

// Provides helpful error pages during development if a database-related error occurs.
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Required for Minimal APIs to discover your endpoints and generate metadata for Swagger/OpenAPI.
builder.Services.AddEndpointsApiExplorer();

// Configures NSwag to generate the OpenAPI (Swagger) documentation for your API.
builder.Services.AddOpenApiDocument(config =>
{
    config.DocumentName = "CustomerAPI"; // The internal name of the document.
    config.Title = "CustomerAPI";        // The title displayed at the top of the Swagger UI.
    config.Version = "v1";               // The version of your API.

    // Adds the "Authorize" button to the Swagger UI so you can test protected routes.
    config.AddSecurity("JWT", Enumerable.Empty<string>(), new NSwag.OpenApiSecurityScheme
    {
        // Defines the security type as an API Key (JWT falls under this in Swagger).
        Type = NSwag.OpenApiSecuritySchemeType.ApiKey, 
        
        // Tells Swagger to look for the "Authorization" field in the HTTP Header.
        Name = "Authorization", 
        In = NSwag.OpenApiSecurityApiKeyLocation.Header, 
        
        // The instruction shown to the user inside the Swagger UI.
        Description = "Type into the textbox: Bearer {your JWT token}." 
    });
});

// So this part remains generally the same across all similar Api?

// Yes, for the most part, this configuration becomes a standard template you will reuse across almost all of your .NET Web API projects. While the specific names change, the "skeleton" remains consistent.


// This section is the Security Engine of your application. While the previous part told Swagger how to talk about security, this part actually builds the "Guard" that checks every request coming into your server.

// 1. Retrieves the secret string from appsettings.json to use for digital signatures.
var secretKey = builder.Configuration["Jwt:Key"];

// 2. Registers the Authentication service and sets JWT as the default scheme.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // 3. Defines the strict "checklist" the server uses to decide if a token is valid.
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,           // Ensure the token came from YOUR server.
            ValidateAudience = true,         // Ensure the token was meant for YOUR client app.
            ValidateLifetime = true,         // Ensure the token hasn't expired.
            ValidateIssuerSigningKey = true, // Ensure the digital "seal" hasn't been tampered with.
            
            ValidIssuer = "your-api",        // The expected name of your API.
            ValidAudience = "your-client",   // The expected name of your frontend.
            
            // Converts your secret string into a mathematical key for verification.
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!))
        };
    });

// 4. Registers the Authorization service (checks if a user has specific permissions/roles).
builder.Services.AddAuthorization();

// 5. Finalizes the builder and creates the 'app' instance ready for the pipeline.
var app = builder.Build();

// 6. Middleware: Inspects incoming headers for a JWT and identifies the user.
app.UseAuthentication();

// 7. Middleware: Decides if the identified user is allowed to access the specific route.
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