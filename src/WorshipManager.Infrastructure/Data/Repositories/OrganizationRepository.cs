using Microsoft.EntityFrameworkCore;
using WorshipManager.Core.Entities;
using WorshipManager.Core.Interfaces;
using WorshipManager.Infrastructure.Data;

namespace WorshipManager.Infrastructure.Repositories;

public class OrganizationRepository : Repository<Organization>, IOrganizationRepository
{
    public OrganizationRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Organization?> GetBySlugAsync(string slug) =>
        await _context.Organizations.AsNoTracking().FirstOrDefaultAsync(o => o.Slug == slug.ToLowerInvariant() && o.IsActive);

    public async Task<Organization?> GetByInviteCodeAsync(string inviteCode)
    {
        var normalizedCode = inviteCode?.Trim().ToUpperInvariant() ?? string.Empty;
        return await _context.Organizations.AsNoTracking().FirstOrDefaultAsync(o => o.InviteCode == normalizedCode && o.IsActive);
    }

    public async Task<bool> SlugExistsAsync(string slug) =>
        await _context.Organizations.AnyAsync(o => o.Slug == slug.ToLowerInvariant());

    public async Task<bool> InviteCodeExistsAsync(string inviteCode)
    {
        var normalizedCode = inviteCode?.Trim().ToUpperInvariant() ?? string.Empty;
        return await _context.Organizations.AnyAsync(o => o.InviteCode == normalizedCode);
    }

    public async Task<IEnumerable<Organization>> GetUserOrganizationsAsync(string userId) =>
        await _context.OrganizationMembers.AsNoTracking()
            .Where(om => om.UserId == userId && om.IsActive)
            .Join(_context.Organizations, om => om.OrganizationId, o => o.Id, (om, o) => o)
            .Where(o => o.IsActive).Distinct().ToListAsync();

    public async Task<Organization?> GetWithSettingsAsync(int id) =>
        await _context.Organizations.AsNoTracking().Include(o => o.Settings).Include(o => o.Members).ThenInclude(m => m.User).FirstOrDefaultAsync(o => o.Id == id);
}
