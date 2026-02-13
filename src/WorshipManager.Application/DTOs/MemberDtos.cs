using System.ComponentModel.DataAnnotations;

namespace WorshipManager.Application.DTOs;

public class MemberDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Instrument { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public bool IsActive { get; set; }
    public int ParticipationCount { get; set; }
    public DateTime? LastParticipationDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<MemberRoleDto> Roles { get; set; } = new();
}

public class MemberRoleDto
{
    public int RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public bool IsPrimary { get; set; }
    public int SkillLevel { get; set; } = 3;
}

public class RoleDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public int DisplayOrder { get; set; }
}

public class CreateMemberDto
{
    [Required(ErrorMessage = "Nome é obrigatório")]
    [MaxLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(50, ErrorMessage = "Instrumento deve ter no máximo 50 caracteres")]
    public string Instrument { get; set; } = string.Empty;

    [MaxLength(20, ErrorMessage = "Telefone deve ter no máximo 20 caracteres")]
    [Phone(ErrorMessage = "Telefone inválido")]
    public string Phone { get; set; } = string.Empty;

    [MaxLength(100, ErrorMessage = "Email deve ter no máximo 100 caracteres")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    public string? Email { get; set; }

    public List<int> RoleIds { get; set; } = new();
    public int? PrimaryRoleId { get; set; }
}

public class UpdateMemberDto
{
    [Required(ErrorMessage = "Nome é obrigatório")]
    [MaxLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(50, ErrorMessage = "Instrumento deve ter no máximo 50 caracteres")]
    public string Instrument { get; set; } = string.Empty;

    [MaxLength(20, ErrorMessage = "Telefone deve ter no máximo 20 caracteres")]
    [Phone(ErrorMessage = "Telefone inválido")]
    public string Phone { get; set; } = string.Empty;

    [MaxLength(100, ErrorMessage = "Email deve ter no máximo 100 caracteres")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    public string? Email { get; set; }

    public bool IsActive { get; set; }
    public List<int> RoleIds { get; set; } = new();
    public int? PrimaryRoleId { get; set; }
}

public class MemberStatisticsDto
{
    public int MemberId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Instrument { get; set; } = string.Empty;
    public int TotalParticipations { get; set; }
    public DateTime? LastParticipation { get; set; }
    public int DaysSinceLastParticipation { get; set; }
    public double ParticipationRate { get; set; }
}
