namespace WorshipManager.Core.Entities;

public class ScheduleSong
{
    public int Id { get; set; }
    public int ScheduleId { get; set; }
    public int SongId { get; set; }
    public int Order { get; set; }
    public string? CustomKey { get; set; }
    public string? Notes { get; set; }
    public int? DurationMinutes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual Schedule Schedule { get; set; } = null!;
    public virtual Song Song { get; set; } = null!;
}
