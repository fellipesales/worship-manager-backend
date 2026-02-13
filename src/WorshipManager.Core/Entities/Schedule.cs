using System.ComponentModel.DataAnnotations;

namespace WorshipManager.Core.Entities;

public class Schedule
{
    public int Id { get; set; }
    public int OrganizationId { get; set; }
    public DateTime Date { get; set; }

    [Required]
    [MaxLength(100)]
    public string ServiceType { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public ScheduleStatus Status { get; set; } = ScheduleStatus.Draft;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    [MaxLength(100)]
    public string? CreatedBy { get; set; }

    public virtual Organization Organization { get; set; } = null!;
    public virtual ICollection<ScheduleMember> ScheduleMembers { get; set; } = new List<ScheduleMember>();
    public virtual ICollection<ScheduleSong> ScheduleSongs { get; set; } = new List<ScheduleSong>();
}
