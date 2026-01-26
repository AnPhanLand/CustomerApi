using Carter;
using MediatR;

public class AuthModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
     app.MapPost("/login", async (LoginRequest request, IMediator mediator) => 
    {
        try 
        {
            var result = await mediator.Send(new LoginCommand(request));
            return Results.Ok(result);
        }
        catch (UnauthorizedAccessException) 
        {
            return Results.Unauthorized();
        }
    });
    }
}