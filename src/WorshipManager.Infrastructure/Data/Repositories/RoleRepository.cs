using Microsoft.EntityFrameworkCore;
using WorshipManager.Core.Entities;
using WorshipManager.Core.Interfaces;
using WorshipManager.Infrastructure.Data;

namespace WorshipManager.Infrastructure.Repositories;

public class RoleRepository : Repository<Role>, IRoleRepository
{
    public RoleRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Role>> GetGlobalRolesAsync() =>
        await _context.Roles.Where(r => r.OrganizationId == null && r.IsActive).OrderBy(r => r.DisplayOrder).ThenBy(r => r.Name).ToListAsync();

    public async Task<IEnumerable<Role>> GetOrganizationRolesAsync(int organizationId) =>
        await _context.Roles.Where(r => (r.OrganizationId == null || r.OrganizationId == organizationId) && r.IsActive)
            .OrderBy(r => r.DisplayOrder).ThenBy(r => r.Name).ToListAsync();

    public async Task<IEnumerable<Role>> GetByCategoryAsync(string category, int? organizationId = null) =>
        await _context.Roles.Where(r => r.Category == category && (r.OrganizationId == null || r.OrganizationId == organizationId) && r.IsActive)
            .OrderBy(r => r.DisplayOrder).ToListAsync();
}
