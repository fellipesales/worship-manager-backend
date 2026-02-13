using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using WorshipManager.Application.Interfaces;
using WorshipManager.Application.Mappings;
using WorshipManager.Application.Services;
using WorshipManager.Application.Validators;

namespace WorshipManager.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(MappingProfile));
        services.AddValidatorsFromAssemblyContaining<CreateMemberDtoValidator>();

        services.AddScoped<IMemberService, MemberService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IAvailabilityService, AvailabilityService>();
        services.AddScoped<IScheduleService, ScheduleService>();
        services.AddScoped<IScheduleGeneratorService, ScheduleGeneratorService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<IWhatsAppService, WhatsAppService>();
        services.AddScoped<ISongService, SongService>();
        services.AddScoped<IOrganizationService, OrganizationService>();

        return services;
    }
}
