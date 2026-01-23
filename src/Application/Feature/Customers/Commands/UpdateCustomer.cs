namespace CustomerApi.Application.Customers.Commands;

// ==========================================
// 1. THE COMMAND (The "What")
// ==========================================

// This record takes both the ID (from the URL) and the DTO (from the Body).
public record UpdateCustomerCommand(Guid Id, CustomerUpdateDTO CustomerDTO) : IRequest<Guid>;

// ==========================================
// 2. THE HANDLER (The "How")
// ==========================================

public class UpdateCustomerHandler : IRequestHandler<UpdateCustomerCommand, Guid>
{
    private readonly IApplicationDbContext _db;
    
    private readonly IDistributedCache _cache;
    private readonly IValidator<UpdateCustomerCommand> _validator;

    // Injecting the database context. Remember to keep CustomerDb 'public'.
    public UpdateCustomerHandler(IApplicationDbContext db, IDistributedCache cache, IValidator<UpdateCustomerCommand> validator)
    {
        _db = db;
        _cache = cache;
        _validator = validator;
    }

    public async Task<Guid> Handle(UpdateCustomerCommand request, CancellationToken ct)
    {
        var validationResult = await _validator.ValidateAsync(request, ct);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }


        // 1. Find the existing customer using the ID from the command.
        // We use the object array syntax to include the CancellationToken 'ct'.
        var customer = await _db.Customers.FindAsync(new object[] { request.Id }, ct);

        // 2. If the customer doesn't exist, stop and return 404.
        if (customer is null) 
        {
            return Guid.Empty;
        }

        // 3. Update the properties of the tracked entity.
        customer.FirstName = request.CustomerDTO.FirstName;
        customer.LastName = request.CustomerDTO.LastName;
        customer.Status = request.CustomerDTO.Status;
        customer.UpdateEmail(request.CustomerDTO.Email);
        customer.UpdatePhoneNumber(request.CustomerDTO.CountryCode, request.CustomerDTO.PhoneNumber);
        customer.UpgradeMembership(request.CustomerDTO.MembershipLevel);

        // 4. Save the changes. 
        // Since EF Core is "tracking" this customer, it knows exactly what changed.
        await _db.SaveChangesAsync(ct);

        await _cache.RemoveAsync($"customer_{request.Id}", ct);
        await _cache.RemoveAsync("all_customers", ct);

        // 5. Return 204 No Content (Standard for successful updates).
        return customer.Id;
    }
}