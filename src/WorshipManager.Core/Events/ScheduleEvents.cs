using WorshipManager.Core.Entities;

namespace WorshipManager.Core.Events;

public class ScheduleCreatedEvent : DomainEventBase
{
    public Schedule Schedule { get; }
    public ScheduleCreatedEvent(Schedule schedule) => Schedule = schedule;
}

public class ScheduleConfirmedEvent : DomainEventBase
{
    public Schedule Schedule { get; }
    public ScheduleConfirmedEvent(Schedule schedule) => Schedule = schedule;
}

public class ScheduleCancelledEvent : DomainEventBase
{
    public Schedule Schedule { get; }
    public ScheduleCancelledEvent(Schedule schedule) => Schedule = schedule;
}
