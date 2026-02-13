using Microsoft.EntityFrameworkCore;
using WorshipManager.Core.Entities;
using WorshipManager.Core.Interfaces;
using WorshipManager.Infrastructure.Data;

namespace WorshipManager.Infrastructure.Repositories;

public class ScheduleRepository : Repository<Schedule>, IScheduleRepository
{
    private readonly ITenantService _tenantService;

    public ScheduleRepository(ApplicationDbContext context, ITenantService tenantService) : base(context)
    {
        _tenantService = tenantService;
    }

    private int? CurrentTenantId => _tenantService.GetCurrentTenantId();

    public override async Task<Schedule?> GetByIdAsync(int id)
    {
        if (!CurrentTenantId.HasValue) return null;
        return await _dbSet.Where(s => s.OrganizationId == CurrentTenantId && s.Id == id).FirstOrDefaultAsync();
    }

    public override async Task<IEnumerable<Schedule>> GetAllAsync()
    {
        if (!CurrentTenantId.HasValue) return Enumerable.Empty<Schedule>();
        return await _dbSet.Include(s => s.ScheduleMembers).ThenInclude(sm => sm.Member)
            .Where(s => s.OrganizationId == CurrentTenantId).OrderBy(s => s.Date).ToListAsync();
    }

    public async Task<Schedule?> GetScheduleWithMembersAsync(int id)
    {
        if (!CurrentTenantId.HasValue) return null;
        return await _dbSet.Include(s => s.ScheduleMembers).ThenInclude(sm => sm.Member)
            .Where(s => s.OrganizationId == CurrentTenantId).FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<IEnumerable<Schedule>> GetSchedulesByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        if (!CurrentTenantId.HasValue) return Enumerable.Empty<Schedule>();
        var startDateOnly = DateTime.SpecifyKind(startDate.Date, DateTimeKind.Utc);
        var endDateOnly = DateTime.SpecifyKind(endDate.Date, DateTimeKind.Utc);
        return await _dbSet.Include(s => s.ScheduleMembers).ThenInclude(sm => sm.Member)
            .Where(s => s.Date.Date >= startDateOnly && s.Date.Date <= endDateOnly && s.OrganizationId == CurrentTenantId)
            .OrderBy(s => s.Date).ToListAsync();
    }

    public async Task<IEnumerable<Schedule>> GetUpcomingSchedulesAsync(int count = 5)
    {
        if (!CurrentTenantId.HasValue) return Enumerable.Empty<Schedule>();
        return await _dbSet.Include(s => s.ScheduleMembers).ThenInclude(sm => sm.Member)
            .Where(s => s.Date.Date >= DateTime.UtcNow.Date && s.Status != ScheduleStatus.Cancelled && s.OrganizationId == CurrentTenantId)
            .OrderBy(s => s.Date).Take(count).ToListAsync();
    }

    public async Task<IEnumerable<Schedule>> GetSchedulesByStatusAsync(ScheduleStatus status)
    {
        if (!CurrentTenantId.HasValue) return Enumerable.Empty<Schedule>();
        return await _dbSet.Include(s => s.ScheduleMembers).ThenInclude(sm => sm.Member)
            .Where(s => s.Status == status && s.OrganizationId == CurrentTenantId).OrderBy(s => s.Date).ToListAsync();
    }

    public async Task<Schedule?> GetScheduleByDateAndServiceTypeAsync(DateTime date, string serviceType)
    {
        if (!CurrentTenantId.HasValue) return null;
        var dateOnly = DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);
        return await _dbSet.Where(s => s.Date.Date == dateOnly && s.ServiceType == serviceType && s.OrganizationId == CurrentTenantId).FirstOrDefaultAsync();
    }
}
