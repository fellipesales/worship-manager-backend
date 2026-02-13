using WorshipManager.Core.Entities;

namespace WorshipManager.Core.Interfaces;

public interface IOrganizationRepository : IRepository<Organization>
{
    Task<Organization?> GetBySlugAsync(string slug);
    Task<Organization?> GetByInviteCodeAsync(string inviteCode);
    Task<bool> SlugExistsAsync(string slug);
    Task<bool> InviteCodeExistsAsync(string inviteCode);
    Task<IEnumerable<Organization>> GetUserOrganizationsAsync(string userId);
    Task<Organization?> GetWithSettingsAsync(int id);
}

public interface IOrganizationMemberRepository : IRepository<OrganizationMember>
{
    Task<OrganizationMember?> GetByUserAndOrganizationAsync(string userId, int organizationId);
    Task<IEnumerable<OrganizationMember>> GetOrganizationMembersAsync(int organizationId);
    Task<bool> UserBelongsToOrganizationAsync(string userId, int organizationId);
    Task<OrganizationRole?> GetUserRoleAsync(string userId, int organizationId);
}

public interface IRoleRepository : IRepository<Role>
{
    Task<IEnumerable<Role>> GetGlobalRolesAsync();
    Task<IEnumerable<Role>> GetOrganizationRolesAsync(int organizationId);
    Task<IEnumerable<Role>> GetByCategoryAsync(string category, int? organizationId = null);
}

public interface IMemberRoleRepository : IRepository<MemberRole>
{
    Task<IEnumerable<MemberRole>> GetMemberRolesAsync(int memberId);
    Task<IEnumerable<MemberRole>> GetMembersByRoleAsync(int roleId);
    Task SetPrimaryRoleAsync(int memberId, int roleId);
    Task DeleteByMemberIdAsync(int memberId);
}
