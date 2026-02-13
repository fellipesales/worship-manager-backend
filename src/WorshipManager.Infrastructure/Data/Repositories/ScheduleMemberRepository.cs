using Microsoft.EntityFrameworkCore;
using WorshipManager.Core.Entities;
using WorshipManager.Core.Interfaces;
using WorshipManager.Infrastructure.Data;

namespace WorshipManager.Infrastructure.Repositories;

public class ScheduleMemberRepository : Repository<ScheduleMember>, IScheduleMemberRepository
{
    public ScheduleMemberRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<ScheduleMember>> GetByScheduleIdAsync(int scheduleId) =>
        await _dbSet.Include(sm => sm.Member).Where(sm => sm.ScheduleId == scheduleId).ToListAsync();

    public async Task<IEnumerable<ScheduleMember>> GetByMemberIdAsync(int memberId) =>
        await _dbSet.Include(sm => sm.Schedule).Where(sm => sm.MemberId == memberId).OrderByDescending(sm => sm.Schedule.Date).ToListAsync();

    public async Task<IEnumerable<ScheduleMember>> GetPendingConfirmationsAsync() =>
        await _dbSet.Include(sm => sm.Member).Include(sm => sm.Schedule)
            .Where(sm => sm.ConfirmedPresence == null && sm.Schedule.Status == ScheduleStatus.Confirmed && sm.Schedule.Date.Date >= DateTime.UtcNow.Date).ToListAsync();

    public async Task DeleteByScheduleIdAsync(int scheduleId)
    {
        var members = await _dbSet.Where(sm => sm.ScheduleId == scheduleId).ToListAsync();
        _dbSet.RemoveRange(members);
    }
}
