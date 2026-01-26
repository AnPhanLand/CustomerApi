using Carter;
using MediatR;
using CustomerApi.Application.Customers.Queries;
using CustomerApi.Application.Customers.Commands;
using CustomerApi.Application.Customers.DTOs;

namespace CustomerApi.API.Endpoints.Modules;

public class CustomerModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder customer = app.MapGroup("/Customer").RequireAuthorization().RequireRateLimiting("fixed");
        
        // List all
        customer.MapGet("/", async (IMediator mediator) => 
        {
            // We send the "Query" object, and MediatR handles the rest
            return await mediator.Send(new GetAllCustomersQuery());
        });

        // Get one by ID
        customer.MapGet("/{id}", async (Guid id, IMediator mediator) => 
        {
            // Pass the id into the constructor of the Query
            return await mediator.Send(new GetCustomerQuery(id));
        });

        // Create new
        customer.MapPost("/", async (CustomerCreateDTO CustomerDTO, IMediator mediator) => 
        {
            return await mediator.Send(new CreateCustomerCommand(CustomerDTO));
        });

        // Update existing
        customer.MapPut("/{id}", async (Guid id, CustomerUpdateDTO CustomerDTO, IMediator mediator) => 
        {
            return await mediator.Send(new UpdateCustomerCommand(id, CustomerDTO));
        });

        // Remove
        customer.MapDelete("/{id}", async (Guid id, IMediator mediator) => 
        {
            return await mediator.Send(new DeleteCustomerCommand(id));
        });
    }
}