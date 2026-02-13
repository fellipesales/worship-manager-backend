using Microsoft.EntityFrameworkCore;
using WorshipManager.Core.Entities;
using WorshipManager.Core.Interfaces;
using WorshipManager.Infrastructure.Data;

namespace WorshipManager.Infrastructure.Repositories;

public class MemberRepository : Repository<Member>, IMemberRepository
{
    private readonly ITenantService _tenantService;

    public MemberRepository(ApplicationDbContext context, ITenantService tenantService) : base(context)
    {
        _tenantService = tenantService;
    }

    private int? CurrentTenantId => _tenantService.GetCurrentTenantId();

    public override async Task<Member?> GetByIdAsync(int id)
    {
        if (!CurrentTenantId.HasValue) return null;
        return await _dbSet.Where(m => m.OrganizationId == CurrentTenantId && m.Id == id).FirstOrDefaultAsync();
    }

    public override async Task<IEnumerable<Member>> GetAllAsync()
    {
        if (!CurrentTenantId.HasValue) return Enumerable.Empty<Member>();
        return await _dbSet.Where(m => m.OrganizationId == CurrentTenantId).OrderBy(m => m.Name).ToListAsync();
    }

    public async Task<IEnumerable<Member>> GetActiveMembersAsync()
    {
        if (!CurrentTenantId.HasValue) return Enumerable.Empty<Member>();
        return await _dbSet.Where(m => m.IsActive && m.OrganizationId == CurrentTenantId).OrderBy(m => m.Name).ToListAsync();
    }

    public async Task<IEnumerable<Member>> GetMembersByInstrumentAsync(string instrument)
    {
        if (!CurrentTenantId.HasValue) return Enumerable.Empty<Member>();
        return await _dbSet.Where(m => m.IsActive && m.Instrument == instrument && m.OrganizationId == CurrentTenantId).OrderBy(m => m.Name).ToListAsync();
    }

    public async Task<IEnumerable<Member>> GetAvailableMembersForDateAsync(DateTime date)
    {
        if (!CurrentTenantId.HasValue) return Enumerable.Empty<Member>();
        var dateOnly = DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);
        var unavailableMemberIds = await _context.Availabilities
            .Where(a => a.Date.Date == dateOnly.Date && !a.IsAvailable && a.OrganizationId == CurrentTenantId)
            .Select(a => a.MemberId).Distinct().ToListAsync();
        return await _dbSet
            .Where(m => m.IsActive && !unavailableMemberIds.Contains(m.Id) && m.OrganizationId == CurrentTenantId)
            .OrderBy(m => m.ParticipationCount).ThenByDescending(m => m.LastParticipationDate ?? DateTime.MinValue).ToListAsync();
    }

    public async Task<Member?> GetMemberWithDetailsAsync(int id)
    {
        if (!CurrentTenantId.HasValue) return null;
        return await _dbSet
            .Include(m => m.Availabilities.Where(a => a.Date >= DateTime.UtcNow.Date))
            .Include(m => m.ScheduleMembers).ThenInclude(sm => sm.Schedule)
            .Include(m => m.MemberRoles).ThenInclude(mr => mr.Role)
            .Where(m => m.OrganizationId == CurrentTenantId).FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task UpdateParticipationCountAsync(int memberId)
    {
        var member = await _dbSet.FindAsync(memberId);
        if (member != null)
        {
            var count = await _context.ScheduleMembers.CountAsync(sm => sm.MemberId == memberId && sm.Schedule.Status == ScheduleStatus.Completed);
            var lastParticipation = await _context.ScheduleMembers
                .Where(sm => sm.MemberId == memberId && sm.Schedule.Status == ScheduleStatus.Completed)
                .OrderByDescending(sm => sm.Schedule.Date).Select(sm => sm.Schedule.Date).FirstOrDefaultAsync();
            member.ParticipationCount = count;
            member.LastParticipationDate = lastParticipation == default ? null : lastParticipation;
        }
    }
}
