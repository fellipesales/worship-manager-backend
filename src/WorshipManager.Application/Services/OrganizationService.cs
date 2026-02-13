using Microsoft.AspNetCore.Identity;
using WorshipManager.Application.Interfaces;
using WorshipManager.Core.Entities;
using WorshipManager.Core.Interfaces;
using WorshipManager.Core.Utilities;

namespace WorshipManager.Application.Services;

public class OrganizationService : IOrganizationService
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IOrganizationMemberRepository _organizationMemberRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantService _tenantService;
    private readonly UserManager<ApplicationUser> _userManager;

    public OrganizationService(IOrganizationRepository organizationRepository, IOrganizationMemberRepository organizationMemberRepository, IUnitOfWork unitOfWork, ITenantService tenantService, UserManager<ApplicationUser> userManager)
    {
        _organizationRepository = organizationRepository; _organizationMemberRepository = organizationMemberRepository;
        _unitOfWork = unitOfWork; _tenantService = tenantService; _userManager = userManager;
    }

    public async Task<Organization> CreateOrganizationAsync(string name, string userId, string? description = null)
    {
        var baseSlug = SlugGenerator.Generate(name);
        var slug = baseSlug; var counter = 1;
        while (await _organizationRepository.SlugExistsAsync(slug)) { slug = $"{baseSlug}-{counter}"; counter++; }

        var inviteCode = await GenerateUniqueInviteCodeAsync();
        var organization = new Organization { Name = name, Slug = slug, InviteCode = inviteCode, Description = description, IsActive = true };
        await _organizationRepository.AddAsync(organization);
        await _unitOfWork.SaveChangesAsync();

        organization.Settings = new OrganizationSettings { OrganizationId = organization.Id };
        var membership = new OrganizationMember { OrganizationId = organization.Id, UserId = userId, Role = OrganizationRole.Admin, IsActive = true };
        await _organizationMemberRepository.AddAsync(membership);
        await _unitOfWork.SaveChangesAsync();

        var user = await _userManager.FindByIdAsync(userId);
        if (user != null) { user.CurrentOrganizationId = organization.Id; await _userManager.UpdateAsync(user); }
        return organization;
    }

    public async Task<Organization?> GetByIdAsync(int id) => await _organizationRepository.GetWithSettingsAsync(id);
    public async Task<Organization?> GetByInviteCodeAsync(string inviteCode) => await _organizationRepository.GetByInviteCodeAsync(inviteCode);

    public async Task<OrganizationMember> JoinOrganizationAsync(string inviteCode, string userId)
    {
        var organization = await _organizationRepository.GetByInviteCodeAsync(inviteCode) ?? throw new InvalidOperationException("Código de convite inválido.");
        var existing = await _organizationMemberRepository.GetByUserAndOrganizationAsync(userId, organization.Id);
        if (existing != null) { if (existing.IsActive) throw new InvalidOperationException("Você já é membro desta organização."); existing.IsActive = true; await _unitOfWork.SaveChangesAsync(); return existing; }

        var membership = new OrganizationMember { OrganizationId = organization.Id, UserId = userId, Role = OrganizationRole.Member, IsActive = true };
        await _organizationMemberRepository.AddAsync(membership);
        await _unitOfWork.SaveChangesAsync();
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null) { user.CurrentOrganizationId = organization.Id; await _userManager.UpdateAsync(user); }
        return membership;
    }

    public async Task<IEnumerable<Organization>> GetUserOrganizationsAsync(string userId) => await _organizationRepository.GetUserOrganizationsAsync(userId);

    public async Task<string> RegenerateInviteCodeAsync(int organizationId)
    {
        var org = await _organizationRepository.GetByIdAsync(organizationId) ?? throw new InvalidOperationException("Organização não encontrada.");
        org.InviteCode = await GenerateUniqueInviteCodeAsync();
        org.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync();
        return org.InviteCode;
    }

    public async Task<Organization> UpdateAsync(int id, string name, string? description, string? address, string? phone, string? email)
    {
        var org = await _organizationRepository.GetByIdAsync(id) ?? throw new InvalidOperationException("Organização não encontrada.");
        org.Name = name; org.Description = description; org.Address = address; org.Phone = phone; org.Email = email; org.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync();
        return org;
    }

    public async Task SetCurrentOrganizationAsync(string userId, int organizationId)
    {
        if (!await _organizationMemberRepository.UserBelongsToOrganizationAsync(userId, organizationId)) throw new InvalidOperationException("Usuário não pertence a esta organização.");
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null) { user.CurrentOrganizationId = organizationId; await _userManager.UpdateAsync(user); }
    }

    public async Task<OrganizationRole?> GetUserRoleAsync(string userId, int organizationId) => await _organizationMemberRepository.GetUserRoleAsync(userId, organizationId);

    public async Task UpdateMemberRoleAsync(int organizationId, string userId, OrganizationRole newRole)
    {
        var membership = await _organizationMemberRepository.GetByUserAndOrganizationAsync(userId, organizationId) ?? throw new InvalidOperationException("Membro não encontrado.");
        membership.Role = newRole;
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task RemoveMemberAsync(int organizationId, string userId)
    {
        var membership = await _organizationMemberRepository.GetByUserAndOrganizationAsync(userId, organizationId) ?? throw new InvalidOperationException("Membro não encontrado.");
        membership.IsActive = false;
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<Organization?> GetCurrentOrganizationAsync()
    {
        var tenantId = _tenantService.GetCurrentTenantId();
        return tenantId == null ? null : await _organizationRepository.GetByIdAsync(tenantId.Value);
    }

    public async Task<OrganizationRole?> GetCurrentUserRoleAsync()
    {
        var userId = _tenantService.GetCurrentUserId();
        var tenantId = _tenantService.GetCurrentTenantId();
        return string.IsNullOrEmpty(userId) || tenantId == null ? null : await _organizationMemberRepository.GetUserRoleAsync(userId, tenantId.Value);
    }

    public async Task<bool> IsCurrentUserAdminAsync() => await GetCurrentUserRoleAsync() == OrganizationRole.Admin;
    public async Task<bool> IsCurrentUserLeaderOrAdminAsync() { var role = await GetCurrentUserRoleAsync(); return role == OrganizationRole.Admin || role == OrganizationRole.Leader; }

    private async Task<string> GenerateUniqueInviteCodeAsync()
    {
        var code = InviteCodeGenerator.Generate();
        while (await _organizationRepository.InviteCodeExistsAsync(code)) code = InviteCodeGenerator.Generate();
        return code;
    }
}
