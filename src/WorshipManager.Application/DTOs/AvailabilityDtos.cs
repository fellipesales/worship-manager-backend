using System.ComponentModel.DataAnnotations;

namespace WorshipManager.Application.DTOs;

public class AvailabilityDto
{
    public int Id { get; set; }
    public int MemberId { get; set; }
    public string MemberName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public bool IsAvailable { get; set; }
    public string? Notes { get; set; }
}

public class CreateAvailabilityDto
{
    [Required(ErrorMessage = "MemberId é obrigatório")]
    public int MemberId { get; set; }

    [Required(ErrorMessage = "Data é obrigatória")]
    public DateTime Date { get; set; }

    public bool IsAvailable { get; set; } = true;

    [MaxLength(500, ErrorMessage = "Notas devem ter no máximo 500 caracteres")]
    public string? Notes { get; set; }
}

public class BulkAvailabilityDto
{
    [Required(ErrorMessage = "MemberId é obrigatório")]
    public int MemberId { get; set; }

    [Required(ErrorMessage = "Datas são obrigatórias")]
    public List<DateTime> Dates { get; set; } = new();

    public List<DateTime> AvailableDates { get; set; } = new();
    public List<DateTime> UnavailableDates { get; set; } = new();
    public bool IsAvailable { get; set; } = true;

    [MaxLength(500, ErrorMessage = "Notas devem ter no máximo 500 caracteres")]
    public string? Notes { get; set; }
}

public class AvailabilityCalendarDto
{
    public DateTime Date { get; set; }
    public List<MemberAvailabilityInfo> AvailableMembers { get; set; } = new();
    public List<MemberAvailabilityInfo> UnavailableMembers { get; set; } = new();
}

public class MemberAvailabilityInfo
{
    public int MemberId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Instrument { get; set; } = string.Empty;
    public string? Notes { get; set; }
}
