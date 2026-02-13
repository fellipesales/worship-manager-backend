using Microsoft.EntityFrameworkCore;
using WorshipManager.Core.Entities;
using WorshipManager.Core.Interfaces;
using WorshipManager.Infrastructure.Data;

namespace WorshipManager.Infrastructure.Repositories;

public class AvailabilityRepository : Repository<Availability>, IAvailabilityRepository
{
    private readonly ITenantService _tenantService;

    public AvailabilityRepository(ApplicationDbContext context, ITenantService tenantService) : base(context)
    {
        _tenantService = tenantService;
    }

    private int? CurrentTenantId => _tenantService.GetCurrentTenantId();

    public override async Task<Availability?> GetByIdAsync(int id)
    {
        if (!CurrentTenantId.HasValue) return null;
        return await _dbSet.Where(a => a.OrganizationId == CurrentTenantId && a.Id == id).FirstOrDefaultAsync();
    }

    public override async Task<IEnumerable<Availability>> GetAllAsync()
    {
        if (!CurrentTenantId.HasValue) return Enumerable.Empty<Availability>();
        return await _dbSet.Where(a => a.OrganizationId == CurrentTenantId).OrderBy(a => a.Date).ToListAsync();
    }

    public async Task<IEnumerable<Availability>> GetByMemberIdAsync(int memberId)
    {
        return await _dbSet.Where(a => a.MemberId == memberId).OrderBy(a => a.Date).ToListAsync();
    }

    public async Task<IEnumerable<Availability>> GetByDateAsync(DateTime date)
    {
        if (!CurrentTenantId.HasValue) return Enumerable.Empty<Availability>();
        var dateOnly = DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);
        return await _dbSet.Include(a => a.Member).Where(a => a.Date.Date == dateOnly && a.OrganizationId == CurrentTenantId).ToListAsync();
    }

    public async Task<IEnumerable<Availability>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        if (!CurrentTenantId.HasValue) return Enumerable.Empty<Availability>();
        var startDateOnly = DateTime.SpecifyKind(startDate.Date, DateTimeKind.Utc);
        var endDateOnly = DateTime.SpecifyKind(endDate.Date, DateTimeKind.Utc);
        return await _dbSet.Include(a => a.Member)
            .Where(a => a.Date.Date >= startDateOnly && a.Date.Date <= endDateOnly && a.OrganizationId == CurrentTenantId)
            .OrderBy(a => a.Date).ToListAsync();
    }

    public async Task<Availability?> GetByMemberAndDateAsync(int memberId, DateTime date)
    {
        var dateOnly = DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);
        return await _dbSet.FirstOrDefaultAsync(a => a.MemberId == memberId && a.Date.Date == dateOnly);
    }

    public async Task DeleteByMemberIdAsync(int memberId)
    {
        var availabilities = await _dbSet.Where(a => a.MemberId == memberId).ToListAsync();
        _dbSet.RemoveRange(availabilities);
    }
}
