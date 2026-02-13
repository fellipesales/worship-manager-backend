using Microsoft.EntityFrameworkCore;
using WorshipManager.Core.Entities;
using WorshipManager.Core.Interfaces;
using WorshipManager.Infrastructure.Data;

namespace WorshipManager.Infrastructure.Repositories;

public class ScheduleHistoryRepository : Repository<ScheduleHistory>, IScheduleHistoryRepository
{
    public ScheduleHistoryRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<ScheduleHistory>> GetByScheduleIdAsync(int scheduleId) =>
        await _dbSet.Where(h => h.ScheduleId == scheduleId).OrderByDescending(h => h.ChangedAt).ToListAsync();
}
