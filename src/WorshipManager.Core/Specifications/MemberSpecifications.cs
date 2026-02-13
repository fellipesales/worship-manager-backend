using WorshipManager.Core.Entities;

namespace WorshipManager.Core.Specifications;

public class ActiveMembersSpec : BaseSpecification<Member>
{
    public ActiveMembersSpec() : base(m => m.IsActive)
    {
        AddInclude(m => m.MemberRoles);
        AddInclude("MemberRoles.Role");
        ApplyOrderBy(m => m.Name);
    }
}

public class MembersByInstrumentSpec : BaseSpecification<Member>
{
    public MembersByInstrumentSpec(string instrument)
        : base(m => m.IsActive && m.Instrument == instrument)
    {
        AddInclude(m => m.MemberRoles);
        AddInclude("MemberRoles.Role");
        ApplyOrderBy(m => m.Name);
    }
}

public class MemberWithDetailsSpec : BaseSpecification<Member>
{
    public MemberWithDetailsSpec(int memberId)
        : base(m => m.Id == memberId)
    {
        AddInclude(m => m.MemberRoles);
        AddInclude("MemberRoles.Role");
        AddInclude(m => m.Availabilities);
        AddInclude(m => m.ScheduleMembers);
    }
}

public class TopParticipantsSpec : BaseSpecification<Member>
{
    public TopParticipantsSpec(int count)
        : base(m => m.IsActive)
    {
        ApplyOrderByDescending(m => m.ParticipationCount);
        ApplyPaging(0, count);
    }
}
