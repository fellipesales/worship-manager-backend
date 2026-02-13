using Microsoft.EntityFrameworkCore;
using WorshipManager.Core.Entities;
using WorshipManager.Core.Interfaces;
using WorshipManager.Infrastructure.Data;

namespace WorshipManager.Infrastructure.Repositories;

public class MemberRoleRepository : Repository<MemberRole>, IMemberRoleRepository
{
    public MemberRoleRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<MemberRole>> GetMemberRolesAsync(int memberId) =>
        await _context.MemberRoles.Where(mr => mr.MemberId == memberId).Include(mr => mr.Role)
            .OrderByDescending(mr => mr.IsPrimary).ThenBy(mr => mr.Role.Name).ToListAsync();

    public async Task<IEnumerable<MemberRole>> GetMembersByRoleAsync(int roleId) =>
        await _context.MemberRoles.Where(mr => mr.RoleId == roleId).Include(mr => mr.Member).Where(mr => mr.Member.IsActive).ToListAsync();

    public async Task SetPrimaryRoleAsync(int memberId, int roleId)
    {
        var roles = await _context.MemberRoles.Where(mr => mr.MemberId == memberId).ToListAsync();
        foreach (var role in roles) role.IsPrimary = role.RoleId == roleId;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteByMemberIdAsync(int memberId)
    {
        var memberRoles = await _context.MemberRoles.Where(mr => mr.MemberId == memberId).ToListAsync();
        _context.MemberRoles.RemoveRange(memberRoles);
    }
}
