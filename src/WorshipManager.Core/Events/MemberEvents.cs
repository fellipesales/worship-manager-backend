using WorshipManager.Core.Entities;

namespace WorshipManager.Core.Events;

public class MemberCreatedEvent : DomainEventBase
{
    public Member Member { get; }
    public MemberCreatedEvent(Member member) => Member = member;
}

public class MemberUpdatedEvent : DomainEventBase
{
    public Member Member { get; }
    public MemberUpdatedEvent(Member member) => Member = member;
}

public class MemberDeletedEvent : DomainEventBase
{
    public int MemberId { get; }
    public MemberDeletedEvent(int memberId) => MemberId = memberId;
}
