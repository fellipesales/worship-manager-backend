using AutoMapper;
using Microsoft.Extensions.Logging;
using WorshipManager.Application.DTOs;
using WorshipManager.Application.Interfaces;
using WorshipManager.Core.Interfaces;

namespace WorshipManager.Application.Services;

public class ReportService : IReportService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<ReportService> _logger;

    public ReportService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ReportService> logger)
    {
        _unitOfWork = unitOfWork; _mapper = mapper; _logger = logger;
    }

    public async Task<DashboardDto> GetDashboardDataAsync()
    {
        var members = await _unitOfWork.Members.GetActiveMembersAsync();
        var memberList = members.ToList();
        var upcoming = await _unitOfWork.Schedules.GetUpcomingSchedulesAsync(5);
        var pending = await _unitOfWork.ScheduleMembers.GetPendingConfirmationsAsync();

        return new DashboardDto
        {
            TotalMembers = memberList.Count,
            ActiveMembers = memberList.Count(m => m.IsActive),
            UpcomingSchedules = upcoming.Count(),
            PendingConfirmations = pending.Count(),
            NextSchedules = _mapper.Map<List<ScheduleCalendarDto>>(upcoming),
            TopParticipants = _mapper.Map<List<MemberStatisticsDto>>(memberList.OrderByDescending(m => m.ParticipationCount).Take(5)),
            InstrumentDistribution = memberList.Where(m => !string.IsNullOrEmpty(m.Instrument)).GroupBy(m => m.Instrument!)
                .Select(g => new InstrumentStatisticsDto { Instrument = g.Key, MemberCount = g.Count(), TotalParticipations = g.Sum(m => m.ParticipationCount) }).ToList()
        };
    }

    public async Task<ParticipationReportDto> GetParticipationReportAsync(ReportFilterDto filter)
    {
        var startDate = filter.StartDate ?? DateTime.UtcNow.AddMonths(-3);
        var endDate = filter.EndDate ?? DateTime.UtcNow;
        var schedules = await _unitOfWork.Schedules.GetSchedulesByDateRangeAsync(startDate, endDate);
        var scheduleList = schedules.ToList();

        return new ParticipationReportDto
        {
            StartDate = startDate, EndDate = endDate,
            TotalSchedules = scheduleList.Count,
            CompletedSchedules = scheduleList.Count(s => s.Status == Core.Entities.ScheduleStatus.Completed),
            TotalParticipations = scheduleList.Sum(s => s.ScheduleMembers.Count),
            AverageParticipantsPerSchedule = scheduleList.Any() ? scheduleList.Average(s => s.ScheduleMembers.Count) : 0,
            MemberStatistics = _mapper.Map<List<MemberStatisticsDto>>((await _unitOfWork.Members.GetActiveMembersAsync()).ToList())
        };
    }

    public async Task<IEnumerable<InstrumentStatisticsDto>> GetInstrumentStatisticsAsync(ReportFilterDto filter)
    {
        var members = await _unitOfWork.Members.GetActiveMembersAsync();
        return members.Where(m => !string.IsNullOrEmpty(m.Instrument)).GroupBy(m => m.Instrument!)
            .Select(g => new InstrumentStatisticsDto { Instrument = g.Key, MemberCount = g.Count(), TotalParticipations = g.Sum(m => m.ParticipationCount) });
    }

    public Task<byte[]> ExportScheduleToPdfAsync(int scheduleId) => Task.FromResult(Array.Empty<byte>());
    public Task<byte[]> ExportReportToExcelAsync(ReportFilterDto filter) => Task.FromResult(Array.Empty<byte>());
    public Task<byte[]> ExportParticipationReportToPdfAsync(ReportFilterDto filter) => Task.FromResult(Array.Empty<byte>());
    public Task<byte[]> ExportParticipationReportToExcelAsync(ReportFilterDto filter) => Task.FromResult(Array.Empty<byte>());
}
