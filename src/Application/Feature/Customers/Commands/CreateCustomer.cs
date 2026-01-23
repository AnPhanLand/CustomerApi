namespace CustomerApi.Application.Customers.Commands;

// ==========================================
// 1. THE COMMAND (The "What")
// ==========================================

// We use the DTO as the input for our Command.
// This tells MediatR: "I want to create a customer, and I expect an IResult back."
public record CreateCustomerCommand(CustomerCreateDTO CustomerDTO) : IRequest<Guid>;

// ==========================================
// 2. THE HANDLER (The "How")
// ==========================================

public class CreateCustomerHandler : IRequestHandler<CreateCustomerCommand, Guid>
{
    private readonly IApplicationDbContext _db;
    private readonly IDistributedCache _cache;
    private readonly IValidator<CreateCustomerCommand> _validator;

    // Injecting the database context. Ensure it is 'public' to avoid CS0051.
    public CreateCustomerHandler(IApplicationDbContext db, IDistributedCache cache, IValidator<CreateCustomerCommand> validator)
    {
        _db = db;
        _cache = cache;
        _validator = validator;
    }

    public async Task<Guid> Handle(CreateCustomerCommand request, CancellationToken ct)
    {
        var validationResult = await _validator.ValidateAsync(request, ct);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        // 1. VALUE OBJECT CREATION
        // We create the Value Objects first. This ensures data integrity 
        // before the Entity even exists.
        var email = EmailAddress.Create(request.CustomerDTO.Email);
        
        // Assuming your DTO has CountryCode and PhoneNumber fields
        var phone = PhoneNumber.Create(
            request.CustomerDTO.CountryCode ?? "+1", 
            request.CustomerDTO.PhoneNumber);

        // 1. Mapping: Convert the DTO inside the 'request' into a new 'Customer' Entity
        var customer = new Customer(email, phone)
        {
            FirstName = request.CustomerDTO.FirstName,
            LastName = request.CustomerDTO.LastName,
            Password = request.CustomerDTO.Password 
        };

        // 2. Staging: Add the new object to the tracking list
        _db.Customers.Add(customer);

        // 3. Execution: Save changes to the database (PostgreSQL in Docker)
        // We pass 'ct' so the database stops if the user disconnects.
        await _db.SaveChangesAsync(ct);

        await _cache.RemoveAsync("all_customers", ct);

        // --- HANGFIRE JOB ---
        // This doesn't run the code NOW. It saves the "Plan" into Postgres
        // and returns immediately.
        // BackgroundJob.Enqueue(() => Console.WriteLine($"Sending Welcome Email to: {customer.Email}"));

        // 4. Response: Return a 201 Created status and the location of the new resource
        return customer.Id; 
    }
}