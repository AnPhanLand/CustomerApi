using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace CustomerApp;

// ==========================================
// 1. THE COMMAND (The "What")
// ==========================================

// We use the DTO as the input for our Command.
// This tells MediatR: "I want to create a customer, and I expect an IResult back."
public record CreateCustomerCommand(CustomerCreateDTO CustomerDTO) : IRequest<IResult>;

// ==========================================
// 2. THE HANDLER (The "How")
// ==========================================

public class CreateCustomerHandler : IRequestHandler<CreateCustomerCommand, IResult>
{
    private readonly CustomerDb _db;
    private readonly IDistributedCache _cache;

    // Injecting the database context. Ensure it is 'public' to avoid CS0051.
    public CreateCustomerHandler(CustomerDb db, IDistributedCache cache)
    {
        _db = db;
        _cache = cache;
    }

    public async Task<IResult> Handle(CreateCustomerCommand request, CancellationToken ct)
    {
        // 1. Mapping: Convert the DTO inside the 'request' into a new 'Customer' Entity
        var customer = new Customer
        {
            FirstName = request.CustomerDTO.FirstName,
            LastName = request.CustomerDTO.LastName,
            Email = request.CustomerDTO.Email,
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
        BackgroundJob.Enqueue(() => Console.WriteLine($"Sending Welcome Email to: {customer.Email}"));

        // 4. Response: Return a 201 Created status and the location of the new resource
        return TypedResults.Created($"/customers/{customer.Id}", new CustomerCreateDTO(customer));
    }
}