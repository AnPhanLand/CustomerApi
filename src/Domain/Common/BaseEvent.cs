namespace CustomerApi.Domain.Common;

public abstract record BaseEvent
{
    public List<BaseEvent> DomainEvents { get; } = new();
}