namespace WorshipManager.Application.DTOs;

public class DashboardDto
{
    public int TotalMembers { get; set; }
    public int ActiveMembers { get; set; }
    public int UpcomingSchedules { get; set; }
    public int PendingConfirmations { get; set; }
    public List<ScheduleCalendarDto> NextSchedules { get; set; } = new();
    public List<MemberStatisticsDto> TopParticipants { get; set; } = new();
    public List<InstrumentStatisticsDto> InstrumentDistribution { get; set; } = new();
}

public class InstrumentStatisticsDto
{
    public string Instrument { get; set; } = string.Empty;
    public int MemberCount { get; set; }
    public int TotalParticipations { get; set; }
}

public class ReportFilterDto
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Instrument { get; set; }
    public int? MemberId { get; set; }
}

public class ParticipationReportDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalSchedules { get; set; }
    public int CompletedSchedules { get; set; }
    public int TotalParticipations { get; set; }
    public double AverageParticipantsPerSchedule { get; set; }
    public List<MemberParticipationDto> MemberParticipations { get; set; } = new();
    public List<MemberStatisticsDto> MemberStatistics { get; set; } = new();
    public List<MonthlyParticipationDto> MonthlyData { get; set; } = new();
    public List<MonthlyParticipationDto> MonthlyParticipations { get; set; } = new();
}

public class MemberParticipationDto
{
    public int MemberId { get; set; }
    public string MemberName { get; set; } = string.Empty;
    public string Instrument { get; set; } = string.Empty;
    public int TotalParticipations { get; set; }
    public DateTime? LastParticipationDate { get; set; }
}

public class MonthlyParticipationDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public string MonthName { get; set; } = string.Empty;
    public int TotalSchedules { get; set; }
    public int TotalParticipations { get; set; }
}
