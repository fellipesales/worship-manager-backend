using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WorshipManager.Application.Interfaces;
using WorshipManager.Core.Entities;

namespace WorshipManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrganizationsController : ControllerBase
{
    private readonly IOrganizationService _organizationService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<OrganizationsController> _logger;

    public OrganizationsController(
        IOrganizationService organizationService,
        UserManager<ApplicationUser> userManager,
        ILogger<OrganizationsController> logger)
    {
        _organizationService = organizationService;
        _userManager = userManager;
        _logger = logger;
    }

    private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet("current")]
    public async Task<IActionResult> GetCurrent()
    {
        var org = await _organizationService.GetCurrentOrganizationAsync();
        if (org == null)
            return NotFound(new { message = "Nenhuma organização selecionada." });

        return Ok(new
        {
            org.Id,
            org.Name,
            org.Slug,
            org.Description,
            org.Address,
            org.Phone,
            org.Email,
            org.InviteCode,
            org.IsActive,
            org.CreatedAt
        });
    }

    [HttpGet("mine")]
    public async Task<IActionResult> GetMyOrganizations()
    {
        var orgs = await _organizationService.GetUserOrganizationsAsync(GetUserId());
        return Ok(orgs.Select(o => new { o.Id, o.Name, o.Slug, o.Description, o.IsActive }));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrganizationRequest request)
    {
        var org = await _organizationService.CreateOrganizationAsync(request.Name, GetUserId(), request.Description);
        return CreatedAtAction(nameof(GetCurrent), new
        {
            org.Id,
            org.Name,
            org.Slug,
            org.InviteCode,
            org.Description,
            org.CreatedAt
        });
    }

    [HttpPost("join")]
    public async Task<IActionResult> Join([FromBody] JoinOrganizationRequest request)
    {
        var membership = await _organizationService.JoinOrganizationAsync(request.InviteCode, GetUserId());
        return Ok(new { membership.OrganizationId, membership.Role, membership.IsActive });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateOrganizationRequest request)
    {
        var org = await _organizationService.UpdateAsync(id, request.Name, request.Description, request.Address, request.Phone, request.Email);
        return Ok(new { org.Id, org.Name, org.Description, org.Address, org.Phone, org.Email });
    }

    [HttpPost("{id}/regenerate-invite")]
    public async Task<IActionResult> RegenerateInviteCode(int id)
    {
        var newCode = await _organizationService.RegenerateInviteCodeAsync(id);
        return Ok(new { inviteCode = newCode });
    }

    [HttpPost("switch/{organizationId}")]
    public async Task<IActionResult> SwitchOrganization(int organizationId)
    {
        await _organizationService.SetCurrentOrganizationAsync(GetUserId(), organizationId);
        return Ok(new { message = "Organização alterada com sucesso." });
    }

    [HttpGet("{id}/members")]
    public async Task<IActionResult> GetMembers(int id)
    {
        var org = await _organizationService.GetByIdAsync(id);
        if (org == null)
            return NotFound();

        var members = org.Members.Where(m => m.IsActive).Select(m => new
        {
            m.UserId,
            m.User?.FullName,
            m.User?.Email,
            m.User?.PhotoUrl,
            Roles = m.MemberRoles.Select(mr => mr.Role?.Name),
            m.CreatedAt
        });

        return Ok(members);
    }

    [HttpPut("{id}/members/{userId}/role")]
    public async Task<IActionResult> UpdateMemberRole(int id, string userId, [FromBody] UpdateRoleRequest request)
    {
        if (!Enum.TryParse<OrganizationRole>(request.Role, out var role))
            return BadRequest(new { message = "Role inválida." });

        await _organizationService.UpdateMemberRoleAsync(id, userId, role);
        return Ok(new { message = "Role atualizada com sucesso." });
    }

    [HttpDelete("{id}/members/{userId}")]
    public async Task<IActionResult> RemoveMember(int id, string userId)
    {
        await _organizationService.RemoveMemberAsync(id, userId);
        return NoContent();
    }
}

public class CreateOrganizationRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class JoinOrganizationRequest
{
    public string InviteCode { get; set; } = string.Empty;
}

public class UpdateOrganizationRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
}

public class UpdateRoleRequest
{
    public string Role { get; set; } = string.Empty;
}
