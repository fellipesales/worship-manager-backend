using System.ComponentModel.DataAnnotations;

namespace WorshipManager.Core.Entities;

public class ScheduleHistory
{
    public int Id { get; set; }
    public int ScheduleId { get; set; }

    [Required]
    [MaxLength(50)]
    public string Action { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    public string? OldValue { get; set; }
    public string? NewValue { get; set; }

    [MaxLength(100)]
    public string? ChangedBy { get; set; }

    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

    public virtual Schedule Schedule { get; set; } = null!;
}
