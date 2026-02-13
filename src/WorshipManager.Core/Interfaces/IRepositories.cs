using WorshipManager.Core.Entities;

namespace WorshipManager.Core.Interfaces;

public interface IMemberRepository : IRepository<Member>
{
    Task<IEnumerable<Member>> GetActiveMembersAsync();
    Task<IEnumerable<Member>> GetMembersByInstrumentAsync(string instrument);
    Task<IEnumerable<Member>> GetAvailableMembersForDateAsync(DateTime date);
    Task<Member?> GetMemberWithDetailsAsync(int id);
    Task UpdateParticipationCountAsync(int memberId);
}

public interface IAvailabilityRepository : IRepository<Availability>
{
    Task<IEnumerable<Availability>> GetByMemberIdAsync(int memberId);
    Task<IEnumerable<Availability>> GetByDateAsync(DateTime date);
    Task<IEnumerable<Availability>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<Availability?> GetByMemberAndDateAsync(int memberId, DateTime date);
    Task DeleteByMemberIdAsync(int memberId);
}

public interface IScheduleRepository : IRepository<Schedule>
{
    Task<Schedule?> GetScheduleWithMembersAsync(int id);
    Task<IEnumerable<Schedule>> GetSchedulesByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<Schedule>> GetUpcomingSchedulesAsync(int count = 5);
    Task<IEnumerable<Schedule>> GetSchedulesByStatusAsync(ScheduleStatus status);
    Task<Schedule?> GetScheduleByDateAndServiceTypeAsync(DateTime date, string serviceType);
}

public interface IScheduleMemberRepository : IRepository<ScheduleMember>
{
    Task<IEnumerable<ScheduleMember>> GetByScheduleIdAsync(int scheduleId);
    Task<IEnumerable<ScheduleMember>> GetByMemberIdAsync(int memberId);
    Task<IEnumerable<ScheduleMember>> GetPendingConfirmationsAsync();
    Task DeleteByScheduleIdAsync(int scheduleId);
}

public interface IScheduleHistoryRepository : IRepository<ScheduleHistory>
{
    Task<IEnumerable<ScheduleHistory>> GetByScheduleIdAsync(int scheduleId);
}

public interface IMessageTemplateRepository : IRepository<MessageTemplate>
{
    Task<MessageTemplate?> GetByTypeAsync(string type);
    Task<IEnumerable<MessageTemplate>> GetActiveTemplatesAsync();
}

public interface ISongRepository : IRepository<Song>
{
    Task<IEnumerable<Song>> GetAllByOrganizationAsync(int organizationId);
    Task<IEnumerable<Song>> GetActiveSongsAsync(int organizationId);
    Task<IEnumerable<Song>> GetByCategoryAsync(int organizationId, string category);
    Task<IEnumerable<Song>> SearchSongsAsync(int organizationId, string searchTerm);
    Task<Song?> GetSongWithDetailsAsync(int id);
    Task IncrementUsageCountAsync(int songId, DateTime usedDate);
}

public interface IScheduleSongRepository : IRepository<ScheduleSong>
{
    Task<IEnumerable<ScheduleSong>> GetByScheduleIdAsync(int scheduleId);
    Task<IEnumerable<ScheduleSong>> GetBySongIdAsync(int songId);
    Task DeleteByScheduleIdAsync(int scheduleId);
    Task UpdateOrderAsync(int scheduleSongId, int newOrder);
}
