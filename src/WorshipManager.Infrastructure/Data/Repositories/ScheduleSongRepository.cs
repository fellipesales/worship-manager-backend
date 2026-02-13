using Microsoft.EntityFrameworkCore;
using WorshipManager.Core.Entities;
using WorshipManager.Core.Interfaces;
using WorshipManager.Infrastructure.Data;

namespace WorshipManager.Infrastructure.Repositories;

public class ScheduleSongRepository : Repository<ScheduleSong>, IScheduleSongRepository
{
    public ScheduleSongRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<ScheduleSong>> GetByScheduleIdAsync(int scheduleId) =>
        await _dbSet.Include(ss => ss.Song).Where(ss => ss.ScheduleId == scheduleId).OrderBy(ss => ss.Order).ToListAsync();

    public async Task<IEnumerable<ScheduleSong>> GetBySongIdAsync(int songId) =>
        await _dbSet.Include(ss => ss.Schedule).Where(ss => ss.SongId == songId).OrderByDescending(ss => ss.Schedule.Date).ToListAsync();

    public async Task DeleteByScheduleIdAsync(int scheduleId)
    {
        var songs = await _dbSet.Where(ss => ss.ScheduleId == scheduleId).ToListAsync();
        _dbSet.RemoveRange(songs);
    }

    public async Task UpdateOrderAsync(int scheduleSongId, int newOrder)
    {
        var scheduleSong = await _dbSet.FindAsync(scheduleSongId);
        if (scheduleSong != null) scheduleSong.Order = newOrder;
    }
}
