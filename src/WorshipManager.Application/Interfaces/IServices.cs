using WorshipManager.Application.DTOs;

namespace WorshipManager.Application.Interfaces;

public interface IMemberService
{
    Task<IEnumerable<MemberDto>> GetAllMembersAsync();
    Task<MemberDto?> GetMemberByIdAsync(int id);
    Task<MemberDto?> GetMemberWithRolesAsync(int id);
    Task<MemberDto> CreateMemberAsync(CreateMemberDto dto);
    Task<MemberDto> UpdateMemberAsync(int id, UpdateMemberDto dto);
    Task DeleteMemberAsync(int id);
    Task<IEnumerable<MemberDto>> GetActiveMembersAsync();
    Task<IEnumerable<MemberDto>> GetMembersByInstrumentAsync(string instrument);
    Task<IEnumerable<MemberStatisticsDto>> GetMemberStatisticsAsync();
}

public interface IRoleService
{
    Task<IEnumerable<RoleDto>> GetAllRolesAsync();
    Task<IEnumerable<RoleDto>> GetRolesByCategoryAsync(string category);
}

public interface IAvailabilityService
{
    Task<IEnumerable<AvailabilityDto>> GetMemberAvailabilitiesAsync(int memberId);
    Task<IEnumerable<AvailabilityDto>> GetMemberAvailabilityAsync(int memberId, DateTime startDate, DateTime endDate);
    Task<AvailabilityCalendarDto> GetAvailabilityForDateAsync(DateTime date);
    Task<IEnumerable<AvailabilityCalendarDto>> GetAvailabilityForRangeAsync(DateTime startDate, DateTime endDate);
    Task<AvailabilityDto> SetAvailabilityAsync(CreateAvailabilityDto dto);
    Task<IEnumerable<AvailabilityDto>> SetBulkAvailabilityAsync(BulkAvailabilityDto dto);
    Task DeleteAvailabilityAsync(int id);
}

public interface IScheduleService
{
    Task<IEnumerable<ScheduleDto>> GetAllSchedulesAsync();
    Task<ScheduleDto?> GetScheduleByIdAsync(int id);
    Task<IEnumerable<ScheduleCalendarDto>> GetScheduleCalendarAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<ScheduleDto>> GetUpcomingSchedulesAsync(int count = 5);
    Task<ScheduleDto> CreateScheduleAsync(CreateScheduleDto dto);
    Task<ScheduleDto> UpdateScheduleAsync(int id, UpdateScheduleDto dto);
    Task DeleteScheduleAsync(int id);
    Task DeleteScheduleAsync(int id, bool forceDelete);
    Task<ScheduleDto> ConfirmScheduleAsync(int id);
    Task<ScheduleDto> CompleteScheduleAsync(int id);
    Task<ScheduleDto> CancelScheduleAsync(int id);
    Task<ScheduleMemberDto> ConfirmPresenceAsync(ConfirmPresenceDto dto);
}

public interface IScheduleGeneratorService
{
    Task<ScheduleDto> GenerateOptimalScheduleAsync(GenerateScheduleDto dto);
    Task<IEnumerable<ScheduleDto>> GenerateMultipleSchedulesAsync(GenerateScheduleDto dto);
    Task<IEnumerable<MemberDto>> GetSuggestedMembersAsync(DateTime date, int count = 5);
}

public interface ISongService
{
    Task<IEnumerable<SongDto>> GetAllSongsAsync();
    Task<IEnumerable<SongDto>> GetActiveSongsAsync();
    Task<SongDto?> GetSongByIdAsync(int id);
    Task<IEnumerable<SongDto>> GetSongsByCategoryAsync(string category);
    Task<IEnumerable<SongDto>> SearchSongsAsync(string searchTerm);
    Task<SongDto> CreateSongAsync(CreateSongDto dto);
    Task<SongDto> UpdateSongAsync(int id, UpdateSongDto dto);
    Task DeleteSongAsync(int id);
    Task<IEnumerable<ScheduleSongDto>> GetScheduleSongsAsync(int scheduleId);
    Task<ScheduleSongDto> AddSongToScheduleAsync(int scheduleId, AddScheduleSongDto dto);
    Task RemoveSongFromScheduleAsync(int scheduleId, int songId);
    Task ReorderScheduleSongsAsync(int scheduleId, List<int> songIdsInOrder);
    Task UpdateScheduleSongAsync(int scheduleSongId, AddScheduleSongDto dto);
}

public interface IReportService
{
    Task<DashboardDto> GetDashboardDataAsync();
    Task<ParticipationReportDto> GetParticipationReportAsync(ReportFilterDto filter);
    Task<IEnumerable<InstrumentStatisticsDto>> GetInstrumentStatisticsAsync(ReportFilterDto filter);
    Task<byte[]> ExportScheduleToPdfAsync(int scheduleId);
    Task<byte[]> ExportReportToExcelAsync(ReportFilterDto filter);
    Task<byte[]> ExportParticipationReportToPdfAsync(ReportFilterDto filter);
    Task<byte[]> ExportParticipationReportToExcelAsync(ReportFilterDto filter);
}

public interface IWhatsAppService
{
    Task<bool> SendScheduleNotificationAsync(int scheduleId);
    Task<bool> SendReminderNotificationAsync(int scheduleId, int daysBefore = 1);
    Task<bool> SendConfirmationRequestAsync(int scheduleMemberId);
    Task<bool> SendMessageAsync(string phoneNumber, string message);
}

public interface INotificationService
{
    Task SendScheduleNotificationsAsync(int scheduleId);
    Task SendRemindersAsync();
    Task SendScheduleChangedNotificationAsync(int scheduleId, string changeDescription);
}
