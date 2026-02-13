namespace WorshipManager.Core.Entities;

public class ScheduleMember
{
    public int Id { get; set; }
    public int ScheduleId { get; set; }
    public int MemberId { get; set; }
    public bool NotificationSent { get; set; }
    public DateTime? NotificationSentAt { get; set; }
    public bool? ConfirmedPresence { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual Schedule Schedule { get; set; } = null!;
    public virtual Member Member { get; set; } = null!;
}
