namespace WorshipManager.Core.Interfaces;

public interface ITenantService
{
    int? GetCurrentTenantId();
    void SetTenant(int tenantId);
    string? GetCurrentUserId();
}
