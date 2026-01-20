namespace CustomerApi.Domain.Events;

public record MembershipLevelUpgradedEvent : BaseEvent
{
    public MembershipLevelUpgradedEvent(Guid customerId, MembershipLevel oldLevel, MembershipLevel newLevel)
    {
        CustomerId = customerId;
        OldLevel = oldLevel;
        NewLevel = newLevel;
        DateOccurred = DateTime.UtcNow;
    }

    public Guid CustomerId { get; init; }
    public MembershipLevel OldLevel { get; init; }
    public MembershipLevel NewLevel { get; init; }
    public DateTime DateOccurred { get; init; }
}