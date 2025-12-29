using Microsoft.EntityFrameworkCore;
using CustomerApp;

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
    config.Title = "CustomerAPI v1";
    config.Version = "v1";
});

var app = builder.Build();

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

RouteGroupBuilder customer = app.MapGroup("/Customer");

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
        Email = customerDTO.Email
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