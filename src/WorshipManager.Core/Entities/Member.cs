using System.ComponentModel.DataAnnotations;

namespace WorshipManager.Core.Entities;

public class Member
{
    public int Id { get; set; }
    public int OrganizationId { get; set; }
    public string? UserId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Instrument { get; set; }

    [MaxLength(20)]
    public string Phone { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Email { get; set; }

    [MaxLength(500)]
    public string? PhotoUrl { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public int ParticipationCount { get; set; }
    public DateTime? LastParticipationDate { get; set; }

    public virtual Organization Organization { get; set; } = null!;
    public virtual ApplicationUser? User { get; set; }
    public virtual ICollection<MemberRole> MemberRoles { get; set; } = new List<MemberRole>();
    public virtual ICollection<Availability> Availabilities { get; set; } = new List<Availability>();
    public virtual ICollection<ScheduleMember> ScheduleMembers { get; set; } = new List<ScheduleMember>();
    public virtual ICollection<OrganizationMember> OrganizationMembers { get; set; } = new List<OrganizationMember>();
}
