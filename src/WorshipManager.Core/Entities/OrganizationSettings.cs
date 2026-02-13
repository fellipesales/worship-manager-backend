using System.ComponentModel.DataAnnotations;

namespace WorshipManager.Core.Entities;

public class OrganizationSettings
{
    public int Id { get; set; }
    public int OrganizationId { get; set; }
    public bool EnableWhatsAppNotifications { get; set; } = true;
    public int ReminderDaysBefore { get; set; } = 2;
    public int MinimumMembersPerSchedule { get; set; } = 4;
    public int MaximumMembersPerSchedule { get; set; } = 8;
    public int MinimumDaysBetweenParticipation { get; set; } = 7;

    [MaxLength(2000)]
    public string ServiceTypesJson { get; set; } = "[\"Culto Domingo Manhã\", \"Culto Domingo Noite\", \"Quarta-feira\"]";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public virtual Organization Organization { get; set; } = null!;
}
