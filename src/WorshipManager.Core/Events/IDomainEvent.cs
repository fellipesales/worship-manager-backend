namespace WorshipManager.Core.Events;

public interface IDomainEvent
{
    DateTime OccurredAt { get; }
}
