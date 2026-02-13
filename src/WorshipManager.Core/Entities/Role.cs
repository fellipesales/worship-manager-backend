using System.ComponentModel.DataAnnotations;

namespace WorshipManager.Core.Entities;

public class Role
{
    public int Id { get; set; }
    public int? OrganizationId { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Category { get; set; } = "Instrumento";

    [MaxLength(50)]
    public string? Icon { get; set; }

    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;

    public virtual Organization? Organization { get; set; }
    public virtual ICollection<MemberRole> MemberRoles { get; set; } = new List<MemberRole>();
}
