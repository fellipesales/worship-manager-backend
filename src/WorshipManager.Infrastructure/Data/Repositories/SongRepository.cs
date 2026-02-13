using Microsoft.EntityFrameworkCore;
using WorshipManager.Core.Entities;
using WorshipManager.Core.Interfaces;
using WorshipManager.Infrastructure.Data;

namespace WorshipManager.Infrastructure.Repositories;

public class SongRepository : Repository<Song>, ISongRepository
{
    private readonly ITenantService _tenantService;

    public SongRepository(ApplicationDbContext context, ITenantService tenantService) : base(context)
    {
        _tenantService = tenantService;
    }

    private int? CurrentTenantId => _tenantService.GetCurrentTenantId();

    public override async Task<Song?> GetByIdAsync(int id)
    {
        if (!CurrentTenantId.HasValue) return null;
        return await _dbSet.Where(s => s.OrganizationId == CurrentTenantId && s.Id == id).FirstOrDefaultAsync();
    }

    public override async Task<IEnumerable<Song>> GetAllAsync()
    {
        if (!CurrentTenantId.HasValue) return Enumerable.Empty<Song>();
        return await _dbSet.Where(s => s.OrganizationId == CurrentTenantId).OrderBy(s => s.Title).ToListAsync();
    }

    public async Task<IEnumerable<Song>> GetAllByOrganizationAsync(int organizationId)
    {
        return await _dbSet.Where(s => s.OrganizationId == organizationId).OrderBy(s => s.Title).ToListAsync();
    }

    public async Task<IEnumerable<Song>> GetActiveSongsAsync(int organizationId)
    {
        return await _dbSet.Where(s => s.OrganizationId == organizationId && s.IsActive).OrderBy(s => s.Title).ToListAsync();
    }

    public async Task<IEnumerable<Song>> GetByCategoryAsync(int organizationId, string category)
    {
        return await _dbSet.Where(s => s.OrganizationId == organizationId && s.IsActive && s.Category == category).OrderBy(s => s.Title).ToListAsync();
    }

    public async Task<IEnumerable<Song>> SearchSongsAsync(int organizationId, string searchTerm)
    {
        var lowerSearch = searchTerm.ToLower();
        return await _dbSet.Where(s => s.OrganizationId == organizationId && s.IsActive &&
            (s.Title.ToLower().Contains(lowerSearch) ||
             (s.Artist != null && s.Artist.ToLower().Contains(lowerSearch)) ||
             (s.Category != null && s.Category.ToLower().Contains(lowerSearch))))
            .OrderBy(s => s.Title).ToListAsync();
    }

    public async Task<Song?> GetSongWithDetailsAsync(int id)
    {
        return await _dbSet.Include(s => s.ScheduleSongs).ThenInclude(ss => ss.Schedule).FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task IncrementUsageCountAsync(int songId, DateTime usedDate)
    {
        var song = await _dbSet.FindAsync(songId);
        if (song != null) { song.TimesUsed++; song.LastUsedDate = usedDate; song.UpdatedAt = DateTime.UtcNow; }
    }
}
