namespace WorshipManager.Core.Entities;

public class OrganizationMember
{
    public int Id { get; set; }
    public int OrganizationId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public OrganizationRole Role { get; set; } = OrganizationRole.Member;
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
    public int? MemberId { get; set; }

    public virtual Organization Organization { get; set; } = null!;
    public virtual ApplicationUser User { get; set; } = null!;
    public virtual Member? Member { get; set; }
}

public enum OrganizationRole
{
    Member = 1,
    Leader = 2,
    Admin = 3
}
