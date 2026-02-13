using System.ComponentModel.DataAnnotations;

namespace WorshipManager.Core.Entities;

public class Song
{
    public int Id { get; set; }
    public int OrganizationId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Artist { get; set; }

    [MaxLength(50)]
    public string? Key { get; set; }

    [MaxLength(20)]
    public string? Tempo { get; set; }

    [MaxLength(50)]
    public string? Category { get; set; }

    [MaxLength(500)]
    public string? SpotifyUrl { get; set; }

    [MaxLength(500)]
    public string? YouTubeUrl { get; set; }

    [MaxLength(500)]
    public string? ChordsUrl { get; set; }

    [MaxLength(2000)]
    public string? Notes { get; set; }

    [MaxLength(2000)]
    public string? Lyrics { get; set; }

    public int TimesUsed { get; set; } = 0;
    public DateTime? LastUsedDate { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public virtual Organization Organization { get; set; } = null!;
    public virtual ICollection<ScheduleSong> ScheduleSongs { get; set; } = new List<ScheduleSong>();
}
