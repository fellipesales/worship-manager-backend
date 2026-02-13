using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorshipManager.Application.Interfaces;
using WorshipManager.Core.Interfaces;

namespace WorshipManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SettingsController : ControllerBase
{
    private readonly IOrganizationService _organizationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SettingsController> _logger;

    public SettingsController(
        IOrganizationService organizationService,
        IUnitOfWork unitOfWork,
        ILogger<SettingsController> logger)
    {
        _organizationService = organizationService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetSettings()
    {
        var org = await _organizationService.GetCurrentOrganizationAsync();
        if (org?.Settings == null)
            return NotFound(new { message = "Configurações não encontradas." });

        var settings = org.Settings;
        return Ok(new
        {
            settings.MaximumMembersPerSchedule,
            settings.MinimumMembersPerSchedule,
            settings.MinimumDaysBetweenParticipation,
            settings.EnableWhatsAppNotifications,
            settings.ReminderDaysBefore,
            settings.ServiceTypesJson
        });
    }

    [HttpPut]
    public async Task<IActionResult> UpdateSettings([FromBody] UpdateSettingsRequest request)
    {
        var org = await _organizationService.GetCurrentOrganizationAsync();
        if (org?.Settings == null)
            return NotFound(new { message = "Configurações não encontradas." });

        var settings = org.Settings;
        settings.MaximumMembersPerSchedule = request.MaximumMembersPerSchedule ?? settings.MaximumMembersPerSchedule;
        settings.MinimumMembersPerSchedule = request.MinimumMembersPerSchedule ?? settings.MinimumMembersPerSchedule;
        settings.MinimumDaysBetweenParticipation = request.MinimumDaysBetweenParticipation ?? settings.MinimumDaysBetweenParticipation;
        settings.EnableWhatsAppNotifications = request.EnableWhatsAppNotifications ?? settings.EnableWhatsAppNotifications;
        settings.ReminderDaysBefore = request.ReminderDaysBefore ?? settings.ReminderDaysBefore;
        settings.ServiceTypesJson = request.ServiceTypesJson ?? settings.ServiceTypesJson;
        settings.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync();
        return Ok(new { message = "Configurações atualizadas com sucesso." });
    }

    [HttpGet("templates")]
    public async Task<IActionResult> GetTemplates()
    {
        var templates = await _unitOfWork.MessageTemplates.GetAllAsync();
        return Ok(templates.Select(t => new
        {
            t.Id,
            t.Name,
            t.Type,
            t.Template,
            t.IsActive
        }));
    }

    [HttpPut("templates/{id}")]
    public async Task<IActionResult> UpdateTemplate(int id, [FromBody] UpdateTemplateRequest request)
    {
        var template = await _unitOfWork.MessageTemplates.GetByIdAsync(id);
        if (template == null)
            return NotFound(new { message = "Template não encontrado." });

        template.Template = request.Template;
        template.IsActive = request.IsActive ?? template.IsActive;
        template.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync();
        return Ok(new { message = "Template atualizado com sucesso." });
    }
}

public class UpdateSettingsRequest
{
    public int? MaximumMembersPerSchedule { get; set; }
    public int? MinimumMembersPerSchedule { get; set; }
    public int? MinimumDaysBetweenParticipation { get; set; }
    public bool? EnableWhatsAppNotifications { get; set; }
    public int? ReminderDaysBefore { get; set; }
    public string? ServiceTypesJson { get; set; }
}

public class UpdateTemplateRequest
{
    public string Template { get; set; } = string.Empty;
    public bool? IsActive { get; set; }
}
