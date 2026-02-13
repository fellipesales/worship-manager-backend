using AutoMapper;
using WorshipManager.Application.DTOs;
using WorshipManager.Application.Interfaces;
using WorshipManager.Core.Interfaces;

namespace WorshipManager.Application.Services;

public class RoleService : IRoleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ITenantService _tenantService;

    public RoleService(IUnitOfWork unitOfWork, IMapper mapper, ITenantService tenantService)
    {
        _unitOfWork = unitOfWork; _mapper = mapper; _tenantService = tenantService;
    }

    public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
    {
        var tenantId = _tenantService.GetCurrentTenantId();
        var roles = tenantId.HasValue
            ? await _unitOfWork.Roles.GetOrganizationRolesAsync(tenantId.Value)
            : await _unitOfWork.Roles.GetGlobalRolesAsync();
        return _mapper.Map<IEnumerable<RoleDto>>(roles);
    }

    public async Task<IEnumerable<RoleDto>> GetRolesByCategoryAsync(string category)
    {
        var tenantId = _tenantService.GetCurrentTenantId();
        var roles = await _unitOfWork.Roles.GetByCategoryAsync(category, tenantId);
        return _mapper.Map<IEnumerable<RoleDto>>(roles);
    }
}
