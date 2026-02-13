namespace WorshipManager.Core.Events;

public abstract class DomainEventBase : IDomainEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}
