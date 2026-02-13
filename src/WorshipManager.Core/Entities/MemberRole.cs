namespace WorshipManager.Core.Entities;

public class MemberRole
{
    public int Id { get; set; }
    public int MemberId { get; set; }
    public int RoleId { get; set; }
    public bool IsPrimary { get; set; }
    public int SkillLevel { get; set; } = 3;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual Member Member { get; set; } = null!;
    public virtual Role Role { get; set; } = null!;
}
