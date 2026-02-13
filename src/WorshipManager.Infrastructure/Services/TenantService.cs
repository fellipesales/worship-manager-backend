using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using WorshipManager.Core.Interfaces;

namespace WorshipManager.Infrastructure.Services;

public class TenantService : ITenantService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private int? _currentTenantId;

    public TenantService(IHttpContextAccessor httpContextAccessor) => _httpContextAccessor = httpContextAccessor;

    public int? GetCurrentTenantId()
    {
        if (_currentTenantId.HasValue) return _currentTenantId;
        var tenantClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("OrganizationId");
        if (tenantClaim != null && int.TryParse(tenantClaim.Value, out var tenantId)) return tenantId;
        return null;
    }

    public void SetTenant(int tenantId) => _currentTenantId = tenantId;

    public string? GetCurrentUserId() =>
        _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
}
