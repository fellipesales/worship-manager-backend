using System.ComponentModel.DataAnnotations;
using WorshipManager.Core.Entities;

namespace WorshipManager.Application.DTOs;

public class ScheduleDto
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string ServiceType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ScheduleStatus Status { get; set; }
    public string StatusName => Status.ToString();
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public List<ScheduleMemberDto> Members { get; set; } = new();
}

public class ScheduleMemberDto
{
    public int Id { get; set; }
    public int MemberId { get; set; }
    public string MemberName { get; set; } = string.Empty;
    public string Instrument { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public bool NotificationSent { get; set; }
    public DateTime? NotificationSentAt { get; set; }
    public bool? ConfirmedPresence { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public string? Notes { get; set; }
}

public class CreateScheduleDto
{
    [Required(ErrorMessage = "Data é obrigatória")]
    public DateTime Date { get; set; }

    [Required(ErrorMessage = "Tipo de culto é obrigatório")]
    [MaxLength(100, ErrorMessage = "Tipo de culto deve ter no máximo 100 caracteres")]
    public string ServiceType { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "Descrição deve ter no máximo 500 caracteres")]
    public string? Description { get; set; }

    public List<int> MemberIds { get; set; } = new();
}

public class UpdateScheduleDto
{
    [Required(ErrorMessage = "Data é obrigatória")]
    public DateTime Date { get; set; }

    [Required(ErrorMessage = "Tipo de culto é obrigatório")]
    [MaxLength(100, ErrorMessage = "Tipo de culto deve ter no máximo 100 caracteres")]
    public string ServiceType { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "Descrição deve ter no máximo 500 caracteres")]
    public string? Description { get; set; }

    public ScheduleStatus Status { get; set; }
    public List<int> MemberIds { get; set; } = new();
}

public class GenerateScheduleDto
{
    public DateTime? Date { get; set; }
    public List<DateTime> Dates { get; set; } = new();

    [Required(ErrorMessage = "Tipo de culto é obrigatório")]
    [MaxLength(100)]
    public string ServiceType { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public int MinimumMembers { get; set; } = 5;
    public List<string> RequiredInstruments { get; set; } = new();
    public bool UseBalancing { get; set; } = true;
    public int MinDaysBetweenParticipations { get; set; } = 7;

    public List<DateTime> GetAllDates()
    {
        if (Dates.Any())
            return Dates.OrderBy(d => d).ToList();
        if (Date.HasValue)
            return new List<DateTime> { Date.Value };
        return new List<DateTime>();
    }
}

public class ScheduleCalendarDto
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string ServiceType { get; set; } = string.Empty;
    public ScheduleStatus Status { get; set; }
    public string StatusName => Status.ToString();
    public int MemberCount { get; set; }
    public string StatusColor => Status switch
    {
        ScheduleStatus.Draft => "#ffc107",
        ScheduleStatus.Confirmed => "#28a745",
        ScheduleStatus.Completed => "#6c757d",
        ScheduleStatus.Cancelled => "#dc3545",
        _ => "#17a2b8"
    };
}

public class ConfirmPresenceDto
{
    public int ScheduleMemberId { get; set; }
    public bool Confirmed { get; set; }
    public string? Notes { get; set; }
}
