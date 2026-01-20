namespace CustomerApi.Domain.Exceptions;

public class CustomerNotFoundException : Exception
{
    public CustomerNotFoundException(Guid id) 
        : base($"Customer with ID {id} was not found in our records.") 
    { }
}