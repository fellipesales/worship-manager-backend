using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using WorshipManager.Core.Interfaces;

namespace WorshipManager.Infrastructure.Middleware;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;

    public TenantMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, ITenantService tenantService)
    {
        var organizationIdClaim = context.User?.FindFirst("OrganizationId");
        if (organizationIdClaim != null && int.TryParse(organizationIdClaim.Value, out var organizationId))
            tenantService.SetTenant(organizationId);
        await _next(context);
    }
}

public static class TenantMiddlewareExtensions
{
    public static IApplicationBuilder UseTenantMiddleware(this IApplicationBuilder builder) =>
        builder.UseMiddleware<TenantMiddleware>();
}
