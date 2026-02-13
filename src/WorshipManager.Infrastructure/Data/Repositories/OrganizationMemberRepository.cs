using Microsoft.EntityFrameworkCore;
using WorshipManager.Core.Entities;
using WorshipManager.Core.Interfaces;
using WorshipManager.Infrastructure.Data;

namespace WorshipManager.Infrastructure.Repositories;

public class OrganizationMemberRepository : Repository<OrganizationMember>, IOrganizationMemberRepository
{
    public OrganizationMemberRepository(ApplicationDbContext context) : base(context) { }

    public async Task<OrganizationMember?> GetByUserAndOrganizationAsync(string userId, int organizationId) =>
        await _context.OrganizationMembers.AsNoTracking().Include(om => om.Organization).Include(om => om.User)
            .FirstOrDefaultAsync(om => om.UserId == userId && om.OrganizationId == organizationId);

    public async Task<IEnumerable<OrganizationMember>> GetOrganizationMembersAsync(int organizationId) =>
        await _context.OrganizationMembers.AsNoTracking()
            .Where(om => om.OrganizationId == organizationId && om.IsActive)
            .Include(om => om.User).Include(om => om.Member).OrderBy(om => om.User.FullName).ToListAsync();

    public async Task<bool> UserBelongsToOrganizationAsync(string userId, int organizationId) =>
        await _context.OrganizationMembers.AnyAsync(om => om.UserId == userId && om.OrganizationId == organizationId && om.IsActive);

    public async Task<OrganizationRole?> GetUserRoleAsync(string userId, int organizationId)
    {
        var member = await _context.OrganizationMembers.AsNoTracking()
            .FirstOrDefaultAsync(om => om.UserId == userId && om.OrganizationId == organizationId && om.IsActive);
        return member?.Role;
    }
}
