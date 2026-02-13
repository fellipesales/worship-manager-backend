using WorshipManager.Core.Entities;

namespace WorshipManager.Core.Specifications;

public class UpcomingSchedulesSpec : BaseSpecification<Schedule>
{
    public UpcomingSchedulesSpec(int count)
        : base(s => s.Date >= DateTime.UtcNow.Date && s.Status != ScheduleStatus.Cancelled)
    {
        AddInclude(s => s.ScheduleMembers);
        AddInclude("ScheduleMembers.Member");
        ApplyOrderBy(s => s.Date);
        ApplyPaging(0, count);
    }
}

public class SchedulesByDateRangeSpec : BaseSpecification<Schedule>
{
    public SchedulesByDateRangeSpec(DateTime startDate, DateTime endDate)
        : base(s => s.Date >= startDate && s.Date <= endDate)
    {
        AddInclude(s => s.ScheduleMembers);
        AddInclude("ScheduleMembers.Member");
        ApplyOrderBy(s => s.Date);
    }
}

public class ScheduleWithMembersSpec : BaseSpecification<Schedule>
{
    public ScheduleWithMembersSpec(int scheduleId)
        : base(s => s.Id == scheduleId)
    {
        AddInclude(s => s.ScheduleMembers);
        AddInclude("ScheduleMembers.Member");
        AddInclude("ScheduleMembers.Member.MemberRoles");
        AddInclude("ScheduleMembers.Member.MemberRoles.Role");
        AddInclude(s => s.ScheduleSongs);
        AddInclude("ScheduleSongs.Song");
    }
}
