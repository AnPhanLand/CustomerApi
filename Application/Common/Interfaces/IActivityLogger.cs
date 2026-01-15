namespace CustomerApp.Application.Common.Interfaces;

public interface IActivityLogger
{
    Task LogActivityAsync(Guid customerId, string action);
}