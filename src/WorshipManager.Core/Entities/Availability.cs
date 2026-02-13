using System.ComponentModel.DataAnnotations;

namespace WorshipManager.Core.Entities;

public class Availability
{
    public int Id { get; set; }
    public int OrganizationId { get; set; }
    public int MemberId { get; set; }
    public DateTime Date { get; set; }
    public bool IsAvailable { get; set; } = true;

    [MaxLength(500)]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public virtual Organization Organization { get; set; } = null!;
    public virtual Member Member { get; set; } = null!;
}
