using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorshipManager.Application.DTOs;
using WorshipManager.Application.Interfaces;

namespace WorshipManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RolesController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoleDto>>> GetAll()
    {
        var roles = await _roleService.GetAllRolesAsync();
        return Ok(roles);
    }

    [HttpGet("category/{category}")]
    public async Task<ActionResult<IEnumerable<RoleDto>>> GetByCategory(string category)
    {
        var roles = await _roleService.GetRolesByCategoryAsync(category);
        return Ok(roles);
    }
}
