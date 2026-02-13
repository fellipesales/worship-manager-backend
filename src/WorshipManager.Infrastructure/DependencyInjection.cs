using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WorshipManager.Core.Interfaces;
using WorshipManager.Infrastructure.Data;
using WorshipManager.Infrastructure.Events;
using WorshipManager.Infrastructure.Repositories;
using WorshipManager.Infrastructure.Services;

namespace WorshipManager.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")),
            ServiceLifetime.Scoped);

        services.AddScoped<ITenantService, TenantService>();
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IMemberRepository, MemberRepository>();
        services.AddScoped<IAvailabilityRepository, AvailabilityRepository>();
        services.AddScoped<IScheduleRepository, ScheduleRepository>();
        services.AddScoped<IScheduleMemberRepository, ScheduleMemberRepository>();
        services.AddScoped<IScheduleHistoryRepository, ScheduleHistoryRepository>();
        services.AddScoped<IMessageTemplateRepository, MessageTemplateRepository>();
        services.AddScoped<ISongRepository, SongRepository>();
        services.AddScoped<IScheduleSongRepository, ScheduleSongRepository>();

        services.AddScoped<IOrganizationRepository, OrganizationRepository>();
        services.AddScoped<IOrganizationMemberRepository, OrganizationMemberRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IMemberRoleRepository, MemberRoleRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
