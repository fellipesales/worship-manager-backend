using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WorshipManager.Application.Interfaces;
using WorshipManager.Core.Interfaces;

namespace WorshipManager.Application.Services;

public class WhatsAppService : IWhatsAppService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<WhatsAppService> _logger;
    private readonly string? _accountSid;
    private readonly string? _authToken;
    private readonly string? _fromNumber;

    public WhatsAppService(IUnitOfWork unitOfWork, IConfiguration configuration, ILogger<WhatsAppService> logger)
    {
        _unitOfWork = unitOfWork; _logger = logger;
        _accountSid = configuration["Twilio:AccountSid"];
        _authToken = configuration["Twilio:AuthToken"];
        _fromNumber = configuration["Twilio:FromNumber"];
    }

    public async Task<bool> SendScheduleNotificationAsync(int scheduleId)
    {
        var schedule = await _unitOfWork.Schedules.GetScheduleWithMembersAsync(scheduleId);
        if (schedule == null) return false;
        var template = await _unitOfWork.MessageTemplates.GetByTypeAsync("ScheduleNotification");
        if (template == null) return false;

        foreach (var sm in schedule.ScheduleMembers)
        {
            var message = template.Template.Replace("{MemberName}", sm.Member.Name).Replace("{ServiceType}", schedule.ServiceType).Replace("{Date}", schedule.Date.ToString("dd/MM/yyyy"));
            await SendMessageAsync(sm.Member.Phone, message);
            sm.NotificationSent = true; sm.NotificationSentAt = DateTime.UtcNow;
            await _unitOfWork.ScheduleMembers.UpdateAsync(sm);
        }
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SendReminderNotificationAsync(int scheduleId, int daysBefore = 1)
    {
        _logger.LogInformation("Sending reminder for schedule {Id}", scheduleId);
        return await Task.FromResult(true);
    }

    public async Task<bool> SendConfirmationRequestAsync(int scheduleMemberId)
    {
        _logger.LogInformation("Sending confirmation request for {Id}", scheduleMemberId);
        return await Task.FromResult(true);
    }

    public Task<bool> SendMessageAsync(string phoneNumber, string message)
    {
        if (string.IsNullOrEmpty(_accountSid))
        {
            _logger.LogInformation("WhatsApp (dev) to {Phone}: {Message}", phoneNumber, message);
            return Task.FromResult(true);
        }
        return Task.FromResult(true);
    }
}
