using WorshipManager.Core.Entities;

namespace WorshipManager.Application.Interfaces;

public interface IOrganizationService
{
    Task<Organization> CreateOrganizationAsync(string name, string userId, string? description = null);
    Task<Organization?> GetByIdAsync(int id);
    Task<Organization?> GetByInviteCodeAsync(string inviteCode);
    Task<OrganizationMember> JoinOrganizationAsync(string inviteCode, string userId);
    Task<IEnumerable<Organization>> GetUserOrganizationsAsync(string userId);
    Task<string> RegenerateInviteCodeAsync(int organizationId);
    Task<Organization> UpdateAsync(int id, string name, string? description, string? address, string? phone, string? email);
    Task SetCurrentOrganizationAsync(string userId, int organizationId);
    Task<OrganizationRole?> GetUserRoleAsync(string userId, int organizationId);
    Task UpdateMemberRoleAsync(int organizationId, string userId, OrganizationRole newRole);
    Task RemoveMemberAsync(int organizationId, string userId);
    Task<Organization?> GetCurrentOrganizationAsync();
    Task<OrganizationRole?> GetCurrentUserRoleAsync();
    Task<bool> IsCurrentUserAdminAsync();
    Task<bool> IsCurrentUserLeaderOrAdminAsync();
}
