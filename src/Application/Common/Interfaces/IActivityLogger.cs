namespace CustomerApi.Application.Common.Interfaces;

public interface IActivityLogger
{
    Task LogActivityAsync(Guid customerId, string action);
}